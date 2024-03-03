// Copyright (c) Kevin BEAUGRAND. All rights reserved.

namespace SemanticKernel.Assistants.AutoGen.Plugins;

public class CodeInterpretionPluginOptions
{
    public string DockerEndpoint { get; set; } = string.Empty;

    public string DockerImage { get; set; } = "python:3-alpine";

    public string OutputFilePath { get; set; } = ".";

    public string GoogleSearchAPIKey { get; set; } = string.Empty;

    public string GoogleSearchEngineId { get; set; } = string.Empty;
}
