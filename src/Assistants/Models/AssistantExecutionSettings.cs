// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models;

internal class AssistantExecutionSettings
{
    [YamlMember(Alias = "planner")]
    public string? Planner { get; set; }

    [YamlMember(Alias = "model")]
    public string? Model { get; set; }

    [YamlMember(Alias = "deployment_name")]
    public string? DeploymentName { get; set; }
}
