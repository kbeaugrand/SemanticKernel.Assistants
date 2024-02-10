// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using System.Reflection;

namespace SemanticKernel.Assistants.AutoGen;

public class AssistantAgentBuilder
{
    public static AssistantBuilder CreateBuilder()
    {
        return AssistantBuilder.FromTemplateStream(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"{typeof(AssistantAgentBuilder).Namespace}.Assistants.AssistantAgent.yaml")!);
    }
}
