// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using SemanticKernel.Assistants.Models;
using System;
using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models;

internal class AssistantModel
{
    [YamlMember(Alias = "name")]
    public string? Name { get; set; }

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "instructions")]
    public string Instructions { get; set; } = string.Empty;

    [YamlMember(Alias = "execution_settings")]
    public AssistantExecutionSettings ExecutionSettings { get; set; } = new();

    [YamlMember(Alias = "input_parameters")]
    public AssistantInputParameter[] Inputs { get; set; } = Array.Empty<AssistantInputParameter>();
}
