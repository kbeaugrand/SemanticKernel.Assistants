// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using System;
using System.Diagnostics;
using System.Linq;

namespace SemanticKernel.Assistants.Extensions;

/// <summary>
/// Extensions for <see cref="Kernel"/>.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// Imports the agent's plugin into the kernel.
    /// </summary>
    /// <param name="kernel">The Kernel instance.</param>
    /// <param name="assistant">The <see cref="IAssistant"/> instance.</param>
    public static void ImportPluginFromAssistant(this Kernel kernel, IAssistant assistant)
    {
        if (assistant is null)
        {
            throw new ArgumentNullException(nameof(assistant));
        }

        var agentConversationPlugin = KernelPluginFactory.CreateFromFunctions(assistant.Name!, assistant.Description!, functions: new[]
        {
            KernelFunctionFactory.CreateFromMethod(async (string input, KernelArguments args) =>
            {
                var thread = assistant.CreateThread(args.ToDictionary());

                return await thread.InvokeAsync(input).ConfigureAwait(false);
            },
            functionName: "Ask",
            description: assistant.Description,
            parameters: assistant.AssistantModel.Inputs.Select(c => c.ToKernelParameterMetadata()),
            returnParameter: new()
            {
                ParameterType = typeof(string),
                Description = "The response from the assistant."
            },
            loggerFactory: kernel.LoggerFactory)
        });

        kernel.Plugins.Add(agentConversationPlugin);
    }
}
