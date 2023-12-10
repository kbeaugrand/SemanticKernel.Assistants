// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models;

internal class AssistantInputParameter
{
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "default_value")]
    public string? DefaultValue { get; set; }
}
