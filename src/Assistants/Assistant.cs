// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernel.Assistants.Models;
using SemanticKernel.Assistants.RoomThread;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants;

/// <summary>
/// Represents an agent.
/// </summary>
public sealed class Assistant : IAssistant
{
    /// <summary>
    /// The agent model.
    /// </summary>
    private readonly AssistantModel _model;

    /// <summary>
    /// The kernel.
    /// </summary>
    private readonly Kernel _kernel;

    /// <summary>
    /// Gets the agent's name.
    /// </summary>
    public string? Name => this._model.Name;

    /// <summary>
    /// Gets the agent's description.
    /// </summary>
    public string? Description => this._model.Description;

    /// <summary>
    /// Gets the agent's instructions.
    /// </summary>
    public string Instructions => this._model.Instructions;

    /// <summary>
    /// Gets the tools defined for run execution.
    /// </summary>
    public KernelPluginCollection Plugins => this._kernel.Plugins;

    /// <summary>
    /// Gets the kernel.
    /// </summary>
    Kernel IAssistant.Kernel => this._kernel;

    /// <summary>
    /// Gets the chat completion service.
    /// </summary>
    IChatCompletionService IAssistant.ChatCompletion => this._kernel.Services.GetService<IChatCompletionService>()!;

    /// <summary>
    /// Gets the assistant threads.
    /// </summary>
    Dictionary<IAssistant, IThread> IAssistant.AssistantThreads { get; } = new Dictionary<IAssistant, IThread>();

    /// <summary>
    /// Gets the assistant model.
    /// </summary>
    AssistantModel IAssistant.AssistantModel => this._model;

    /// <summary>
    /// Gets the planner.
    /// </summary>
    public string Planner => this._model.ExecutionSettings.Planner!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Assistant"/> class.
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="kernel">The kernel</param>
    internal Assistant(AssistantModel model,
        Kernel kernel)
    {
        this._model = model;
        this._kernel = kernel;
    }

    /// <summary>
    /// Create a new assistant builder.
    /// </summary>
    /// <returns>The assistant builder.</returns>
    public static AssistantBuilder CreateBuilder()
    {
        return new();
    }

    /// <summary>
    /// Creates a new agent from a yaml template.
    /// </summary>
    /// <param name="definitionPath">The yaml definition file path.</param>
    /// <param name="azureOpenAIEndpoint">The Azure OpenAI endpoint.</param>
    /// <param name="azureOpenAIKey">The Azure OpenAI key.</param>
    /// <param name="plugins">The plugins.</param>
    /// <param name="assistants">The assistants.</param>
    /// <param name="loggerFactory">The logger factory instance.</param>
    /// <returns></returns>
    public static IAssistant FromTemplate(
        string definitionPath,
        string azureOpenAIEndpoint,
        string azureOpenAIKey,
        IEnumerable<KernelPlugin>? plugins = null,
        ILoggerFactory? loggerFactory = null,
        params IAssistant[] assistants)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlContent = File.ReadAllText(definitionPath);

        var agentModel = deserializer.Deserialize<AssistantModel>(yamlContent);

        var agentBuilder = new AssistantBuilder()
            .WithName(agentModel.Name!.Trim())
            .WithDescription(agentModel.Description!.Trim())
            .WithInstructions(agentModel.Instructions.Trim())
            .WithPlanner(agentModel.ExecutionSettings.Planner?.Trim())
            .WithExecutionSettings(agentModel.ExecutionSettings.PromptExecutionSettings)
            .WithInputParameter(agentModel.Input.Description?.Trim()!, agentModel.Input.DefaultValue?.Trim()!)
            .WithAzureOpenAIChatCompletion(agentModel.ExecutionSettings.ServiceId!, agentModel.ExecutionSettings.Model!, azureOpenAIEndpoint, azureOpenAIKey);

        if (plugins is not null)
        {
            foreach (var plugin in plugins)
            {
                agentBuilder.WithPlugin(plugin);
            }
        }

        if (assistants is not null)
        {
            foreach (var assistant in assistants)
            {
                agentBuilder.WithAssistant(assistant);
            }
        }

        if (loggerFactory is not null)
        {
            agentBuilder.WithLoggerFactory(loggerFactory);
        }

        return agentBuilder.Build();
    }

    /// <summary>
    /// Creates a new room thread for collaborative agents.
    /// </summary>
    /// <param name="agents">The collaborative agents.</param>
    /// <returns></returns>
    public static IRoomThread CreateRoomThread(params IAssistant[] agents)
    {
        return new RoomThread.RoomThread(agents);
    }

    /// <summary>
    /// Create a new conversable thread.
    /// </summary>
    /// <returns></returns>
    public IThread CreateThread()
    {
        return new Thread(this);
    }

    /// <summary>
    /// Create a new conversable thread using actual kernel arguments.
    /// </summary>
    /// <param name="initatedAgent">The agent that is creating a thread with this agent.</param>
    /// <param name="arguments">The actual kernel parameters.</param>
    /// <returns></returns>
    IThread IAssistant.CreateThread(IAssistant initatedAgent, Dictionary<string, object?> arguments)
    {
        return new Thread(this, initatedAgent.Name!, arguments);
    }
}
