// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using _02_autogen.Exceptions;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace _02_autogen.Plugins
{
    internal class CodeInterpretionPlugin
    {
        private readonly DockerClient _dockerClient;

        private readonly CodeInterpretionPluginOptions _options;

        private readonly ILogger<CodeInterpretionPlugin> _logger;

        private const string CodeFilePath = "/var/app/code.py";
        private const string RequirementsFilePath = "/var/app/requirements.txt";

        public CodeInterpretionPlugin(CodeInterpretionPluginOptions options, ILoggerFactory loggerFactory)
        {
            this._options = options;
            this._dockerClient = new DockerClientConfiguration(new Uri(options.DockerEndpoint)).CreateClient();

            this._logger = loggerFactory.CreateLogger<CodeInterpretionPlugin>();
        }

        [KernelFunction]
        [Description("Executes the specified python code in a sandbox.")]
        [return: Description("The result of the program execution.")]
        public async Task<string> ExecutePythonCode(KernelArguments arguments = null!)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            await PullRequiredImageAsync().ConfigureAwait(false);

            var instanceId = string.Empty;

            var codeFilePath = Path.GetTempFileName();
            var requirementsFilePath = Path.GetTempFileName();

            try
            {
                if (arguments.TryGetValue("input", out var pythonCode))
                {
                    File.WriteAllText(codeFilePath, pythonCode!.ToString());
                }
                else
                {
                    throw new CodeInterpreterException("The input code is not correctly provided.");
                }

                if (arguments.TryGetValue("requirements", out object? requirements))
                {
                    File.WriteAllText(requirementsFilePath, requirements?.ToString());
                }

                instanceId = await this.StartNewSandbox(@requirementsFilePath, codeFilePath).ConfigureAwait(false);

                this._logger.LogTrace($"Preparing Sandbox ({instanceId}:{Environment.NewLine}requirements.txt:{Environment.NewLine}{requirements}{Environment.NewLine}code.py:{Environment.NewLine}{pythonCode}");

                await this.InstallRequirementsAsync(instanceId).ConfigureAwait(false);

                return await this.ExecuteCodeAsync(instanceId).ConfigureAwait(false);
            }
            finally
            {
                if (!string.IsNullOrEmpty(instanceId))
                {
                    await this._dockerClient.Containers.RemoveContainerAsync(instanceId, new ContainerRemoveParameters
                    {
                        Force = true
                    }).ConfigureAwait(false);
                }

                if (File.Exists(codeFilePath))
                {
                    File.Delete(codeFilePath);
                }
                if (File.Exists(requirementsFilePath))
                {
                    File.Delete(requirementsFilePath);
                }
            }
        }

        private async Task<string> StartNewSandbox(string requirementFilePath, string codeFilePath)
        {
            var config = new Config()
            {
                Hostname = "localhost",
            };

            var containerCreateOptions = new CreateContainerParameters(config)
            {
                Image = this._options.DockerImage,
                Entrypoint = new[] { "/bin/sh" },
                Tty = true,
                NetworkDisabled = false,
                HostConfig = new HostConfig()
                {
                    Binds = new[]
                    {
                        $"{codeFilePath}:{CodeFilePath}",
                        $"{requirementFilePath}:{RequirementsFilePath}"
                    }
                }
            };

            this._logger.LogDebug("Creating container.");
            var response = await _dockerClient.Containers.CreateContainerAsync(containerCreateOptions).ConfigureAwait(false);

            this._logger.LogDebug($"Starting the container (id: {response.ID}).");
            await _dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters()).ConfigureAwait(false);

            return response.ID;
        }

        private async Task InstallRequirementsAsync(string containerId)
        {
            _ = await this.ExecuteInContainer(containerId, $"pip install -r {RequirementsFilePath}");
        }

        private async Task<string> ExecuteCodeAsync(string containerId)
        {
            return await this.ExecuteInContainer(containerId, $"python {CodeFilePath}").ConfigureAwait(false);
        }

        private async Task<string> ExecuteInContainer(string containerId, string command)
        {
            this._logger.LogDebug($"({containerId})# {command}");

            var execContainer = await this._dockerClient.Exec.ExecCreateContainerAsync(containerId, new ContainerExecCreateParameters
            {
                AttachStderr = true,
                AttachStdout = true,
                AttachStdin = true,
                Cmd = command.Split(' ', StringSplitOptions.RemoveEmptyEntries),
                Tty = true
            }).ConfigureAwait(false);

            var multiplexedStream = await _dockerClient.Exec.StartAndAttachContainerExecAsync(execContainer.ID, true);

            var output = await multiplexedStream.ReadOutputToEndAsync(CancellationToken.None);

            if (!string.IsNullOrWhiteSpace(output.stderr))
            {
                this._logger.LogError($"({containerId}): {output.stderr}");
                throw new CodeInterpreterException(output.stderr);
            }

            this._logger.LogDebug($"({containerId}): {output.stdout}");

            return output.stdout;
        }

        private async Task PullRequiredImageAsync()
        {
            try
            {
                _ = await _dockerClient.Images.InspectImageAsync(this._options.DockerImage);
            }
            catch (DockerImageNotFoundException)
            {
                await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters() { FromImage = this._options.DockerImage }, new AuthConfig(), new Progress<JSONMessage>());
            }
        }
    }
}
