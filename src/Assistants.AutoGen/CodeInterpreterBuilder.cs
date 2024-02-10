// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using System.Reflection;

namespace SemanticKernel.Assistants.AutoGen;

public class CodeInterpreterBuilder
{
    public static AssistantBuilder CreateBuilder()
    {
        return AssistantBuilder.FromTemplateStream(
            Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"{typeof(AssistantAgentBuilder).Namespace}.Assistants.CodeInterpreter.yaml")!);
    }
}
