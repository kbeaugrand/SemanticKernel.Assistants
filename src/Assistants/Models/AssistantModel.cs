// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using SemanticKernel.Assistants.Models;
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

    [YamlMember(Alias = "input_parameter")]
    public AssistantInputParameter Input { get; set; } = new();
}
