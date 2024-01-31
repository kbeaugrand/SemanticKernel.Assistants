// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using System;
using System.Threading.Tasks;

namespace SemanticKernel.Assistants.GroupThread;

/// <summary>
/// Interface representing a room thread.
/// </summary>
public interface IGroupThread
{
    /// <summary>
    /// Adds the user message to the discussion.
    /// </summary>
    /// <param name="message">The user message.</param>
    /// <returns></returns>
    Task AddUserMessageAsync(string message);

    /// <summary>
    /// Event produced when an agent sends a message.
    /// </summary>
    event EventHandler<ChatMessageContent>? OnMessageReceived;
}
