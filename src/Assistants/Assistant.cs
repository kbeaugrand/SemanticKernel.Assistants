// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using SemanticKernel.Assistants.Models;
using System.Collections.Generic;

namespace SemanticKernel.Assistants;

/// <summary>
/// Represents an agent.
/// </summary>
internal class Assistant : IAssistant
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
    public Assistant(AssistantModel model,
        Kernel kernel)
    {
        this._model = model;
        this._kernel = kernel;
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
