// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel.AI.ChatCompletion;
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
    /// Gets the chat messages.
    /// </summary>
    IReadOnlyList<ChatMessageContent> ChatMessages { get; }
}
