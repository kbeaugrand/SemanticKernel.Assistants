// Copyright (c) Kevin BEAUGRAND. All rights reserved.

namespace SemanticKernel.Assistants.AutoGen.Exceptions;

internal class CodeInterpreterException : Exception
{
    internal CodeInterpreterException(string message, params string[] warnings)
        : base(message)
    {
        this.Warnings = warnings;
    }

    public string[] Warnings { get; }
}
