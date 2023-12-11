// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel.AI.ChatCompletion;
using System;
using System.Threading.Tasks;

namespace SemanticKernel.Assistants.RoomThread;

/// <summary>
/// Interface representing a room thread.
/// </summary>
public interface IRoomThread
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
