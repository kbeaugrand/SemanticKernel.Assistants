﻿// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models;

internal class AssistantExecutionSettings
{
    [YamlMember(Alias = "planner")]
    public string? Planner { get; set; }

    [YamlMember(Alias = "model")]
    public string? Model { get; set; }

    [YamlMember(Alias = "service_id")]
    public string? ServiceId { get; set; }

    [YamlMember(Alias = "past_messages_included")]
    public int PastMessagesIncluded { get; set; } = 10;

    [YamlMember(Alias = "prompt_settings")]
    public AssistantPromptExecutionSettings PromptExecutionSettings { get; set; } = new();
}
