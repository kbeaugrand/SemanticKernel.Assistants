// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using System;
using System.Linq;

namespace SemanticKernel.Assistants.Extensions;

/// <summary>
/// Extensions for <see cref="Kernel"/>.
/// </summary>
internal static class KernelExtensions
{
    /// <summary>
    /// Imports the agent's plugin into the kernel.
    /// </summary>
    /// <param name="kernel">The Kernel instance.</param>
    /// <param name="agent">The Agent to import.</param>
    /// <param name="model">The <see cref="AgentAssistantModel"/> instance.</param>
    public static void ImportPluginFromAgent(this Kernel kernel, IAssistant agent, IAssistant otherAssistant)
    {
        var agentConversationPlugin = KernelPluginFactory.CreateFromFunctions(otherAssistant.Name!, otherAssistant.Description!);

        KernelFunctionFactory.CreateFromMethod(async (string input, KernelArguments args) =>
        {
            if (!agent.AssistantThreads.TryGetValue(otherAssistant, out var thread))
            {
                thread = otherAssistant.CreateThread();
                agent.AssistantThreads.Add(otherAssistant, thread);
            }

            return await thread.InvokeAsync(input).ConfigureAwait(false);
        },
        functionName: "Ask",
        description: otherAssistant.Description,
        parameters: new[]
        {
            new KernelParameterMetadata("input")

            {
                IsRequired = true,
                ParameterType = typeof(string),
                DefaultValue = otherAssistant.AssistantModel.Input.DefaultValue,
                Description = otherAssistant.AssistantModel.Input.Description
            }
        }, returnParameter: new()
        {
            ParameterType = typeof(string),
            Description = "The response from the assistant."
        },
        loggerFactory: kernel.LoggerFactory);

        kernel.Plugins.Add(agentConversationPlugin);
    }
}
