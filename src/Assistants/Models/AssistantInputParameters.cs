// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.SemanticKernel;
using System;
using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants.Models;

/// <summary>
/// Class representing the assistant input parameter.
/// </summary>
public class AssistantInputParameter
{
    /// <summary>
    /// Gets or sets the input name.
    /// </summary>
    [YamlMember(Alias = "name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the parameter description.
    /// </summary>
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "default_value")]
    public string? DefaultValue { get; set; }

    /// <summary>Gets whether the parameter is required.</summary>
    [YamlMember(Alias = "is_required")]
    public bool IsRequired { get; set; }

    /// <summary>Gets the .NET type of the parameter.</summary>
    [YamlMember(Alias = "parameter_type")]
    public Type? ParameterType { get; set; }

    /// <summary>Gets a JSON Schema describing the parameter's type.</summary>
    [YamlMember(Alias = "schema")]
    public KernelJsonSchema? Schema { get; set; }

    internal KernelParameterMetadata ToKernelParameterMetadata()
    {
        return new KernelParameterMetadata(this.Name!)
        {
            DefaultValue = this.DefaultValue,
            Description = this.Description,
            IsRequired = this.IsRequired,
            ParameterType = this.ParameterType,
            Schema = this.Schema
        };
    }
}
