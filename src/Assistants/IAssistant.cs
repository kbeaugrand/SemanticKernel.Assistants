// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernel.Assistants.Models;
using System.Collections.Generic;

namespace SemanticKernel.Assistants;

/// <summary>
/// Interface for an agent that can call the model and use tools.
/// </summary>
public interface IAssistant
{
    /// <summary>
    /// Gets the agent's name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets the agent's description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the agent's instructions.
    /// </summary>
    public string Instructions { get; }

    /// <summary>
    /// Gets the planner.
    /// </summary>
    public string Planner { get; }

    /// <summary>
    /// Tools defined for run execution.
    /// </summary>
    public KernelPluginCollection Plugins { get; }

    /// <summary>
    /// A semantic-kernel <see cref="Kernel"/> instance associated with the assistant.
    /// </summary>
    internal Kernel Kernel { get; }

    /// <summary>
    /// The chat completion service.
    /// </summary>
    internal IChatCompletionService ChatCompletion { get; }

    /// <summary>
    /// Gets the agent threads.
    /// </summary>
    internal Dictionary<IAssistant, IThread> AssistantThreads { get; }

    /// <summary>
    /// Gets the assistant model.
    /// </summary>
    internal AssistantModel AssistantModel { get; }

    /// <summary>
    /// Create a new conversable thread.
    /// </summary>
    public IThread CreateThread();

    /// <summary>
    /// Create a new conversable thread using actual kernel arguments.
    /// </summary>
    internal IThread CreateThread(IAssistant initatedAgent, Dictionary<string, object?> arguments);
}
