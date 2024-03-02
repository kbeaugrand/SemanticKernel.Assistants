// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SemanticKernel.Assistants;

/// <summary>
/// Interface for a conversable thread.
/// </summary>
public interface IThread
{
    /// <summary>
    /// Invokes the thread.
    /// </summary>
    /// <returns></returns>
    Task<ChatMessageContent> InvokeAsync(string userMessage);

    /// <summary>
    /// Adds a system message.
    /// </summary>
    /// <param name="message">The message to add.</param>
    void AddSystemMessage(string message);


    /// <summary>
    /// Adds the user message to the chat history.
    /// </summary>
    /// <param name="message">The message to add.</param>
    void AddUserMessage(string message);

    /// <summary>
    /// Adds the assistant message to the chat history.
    /// </summary>
    /// <param name="message">The message to add.</param>
    void AddAssistantMessage(string message);

    /// <summary>
    /// Gets the chat messages.
    /// </summary>
    IReadOnlyList<ChatMessageContent> ChatMessages { get; }

    /// <summary>
    /// Updates the kernel arguments.
    /// </summary>
    /// <param name="arguments">The new kernel arguments.</param>
    internal void UpdateKernelArguments(KernelArguments arguments);
}
