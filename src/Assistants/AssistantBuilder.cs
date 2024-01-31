// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using SemanticKernel.Assistants.Extensions;
using SemanticKernel.Assistants.Models;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace SemanticKernel.Assistants;

/// <summary>
/// Fluent builder for initializing an <see cref="IAssistant"/> instance.
/// </summary>
public partial class AssistantBuilder
{
    /// <summary>
    /// The agent's assistants.
    /// </summary>
    private readonly List<IAssistant> _assistants;

    /// <summary>
    /// The kernel builder.
    /// </summary>
    internal Kernel? Kernel { get; private set; }

    /// <summary>
    /// The agent model.
    /// </summary>
    internal AssistantModel Model { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantBuilder"/> class.
    /// </summary>
    public AssistantBuilder()
    {
        this.Model = new AssistantModel();
        this._assistants = new List<IAssistant>();
    }

    /// <summary>
    /// Builds the agent.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="KernelException"></exception>
    public IAssistant Build()
    {
        if (this.Kernel is null)
        {
            throw new KernelException("The Kernel is not configured.");
        }

        var agent = new Assistant(this.Model, this.Kernel);

        foreach (var item in this._assistants)
        {
            this.Kernel.ImportPluginFromAgent(agent, item);
        }

        return agent;
    }

    /// <summary>
    /// Defines the agent's name.
    /// </summary>
    /// <param name="name">The agent's name.</param>
    /// <returns></returns>
    public AssistantBuilder WithName(string name)
    {
        this.Model.Name = name;
        return this;
    }

    /// <summary>
    /// Defines the agent's description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public AssistantBuilder WithDescription(string description)
    {
        this.Model.Description = description;
        return this;
    }

    /// <summary>
    /// Defines the agent's instructions.
    /// </summary>
    /// <param name="instructions">The instructions.</param>
    /// <returns></returns>
    public AssistantBuilder WithInstructions(string instructions)
    {
        this.Model.Instructions = instructions;
        return this;
    }

    /// <summary>
    /// Adds the agent's collaborative assistant.
    /// </summary>
    /// <param name="assistant">The assistant.</param>
    /// <returns></returns>
    public AssistantBuilder WithAssistant(IAssistant assistant)
    {
        this._assistants.Add(assistant);

        return this;
    }

    /// <summary>
    /// Defines the agent's planner.
    /// </summary>
    /// <param name="plannerName">The agent's planner name.</param>
    /// <returns></returns>
    public AssistantBuilder WithPlanner(string plannerName)
    {
        this.Model.ExecutionSettings.Planner = plannerName;
        return this;
    }

    /// <summary>
    /// Configures the agent's execution settings.
    /// </summary>
    /// <param name="executionSettings"></param>
    /// <returns></returns>
    public AssistantBuilder WithExecutionSettings(AssistantPromptExecutionSettings executionSettings)
    {
        this.Model.ExecutionSettings.PromptExecutionSettings = executionSettings;

        return this;
    }

    /// <summary>
    /// Defines the agent's input parameter.
    /// </summary>
    /// <param name="inputParameter">The input parameter.</param>
    /// <returns></returns>
    public AssistantBuilder WithInputParameter(string description, string defaultValue = "")
    {
        this.Model.Input = new AssistantInputParameter
        {
            DefaultValue = defaultValue,
            Description = description,
        };
        return this;
    }

    /// <summary>
    /// Defines the agent's kernel.
    /// </summary>
    /// <param name="kernel">The kernel.</param>
    /// <returns></returns>
    public AssistantBuilder WithKernel(Kernel kernel)
    {
        this.Kernel = kernel;
        return this;
    }

    /// <summary>
    /// Creates a new agent builder from a yaml template.
    /// </summary>
    /// <param name="definitionPath">The yaml definition file path.</param>
    /// <param name="assistants">The assistants.</param>
    /// <returns></returns>
    public static AssistantBuilder FromTemplate(
        string definitionPath,
        params IAssistant[] assistants)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlContent = File.ReadAllText(definitionPath);

        var agentModel = deserializer.Deserialize<AssistantModel>(yamlContent);

        var agentBuilder = new AssistantBuilder();
        agentBuilder.Model = agentModel;

        if (assistants is not null)
        {
            foreach (var assistant in assistants)
            {
                agentBuilder.WithAssistant(assistant);
            }
        }

        return agentBuilder;
    }
}
