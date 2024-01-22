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
    /// The agent's plugins.
    /// </summary>
    private readonly List<KernelPlugin> _plugins;

    /// <summary>
    /// The logger factory.
    /// </summary>
    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    /// <summary>
    /// The kernel builder.
    /// </summary>
    internal IKernelBuilder KernelBuilder { get; }

    /// <summary>
    /// The agent model.
    /// </summary>
    internal AssistantModel Model { get; private set; }

    /// <summary>
    /// The AI services configuration.
    /// </summary>
    internal Action<IKernelBuilder> ConfigureAIServices { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantBuilder"/> class.
    /// </summary>
    public AssistantBuilder()
    {
        this.Model = new AssistantModel();
        this._assistants = new List<IAssistant>();
        this.KernelBuilder = Kernel.CreateBuilder();
        this._plugins = new List<KernelPlugin>();
    }

    /// <summary>
    /// Builds the agent.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="KernelException"></exception>
    public IAssistant Build()
    {
        if (this.Model.ExecutionSettings.Model is null)
        {
            throw new KernelException("The agent's model is not defined.");
        }

        if (this.ConfigureAIServices is null)
        {
            throw new KernelException("The AI services are not configured.");
        }

        this.ConfigureAIServices!(this.KernelBuilder);

        var kernel = this.KernelBuilder.Build();

        var agent = new Assistant(this.Model, kernel);

        foreach (var item in this._plugins)
        {
            kernel.Plugins.Add(item);
        }

        foreach (var item in this._assistants)
        {
            kernel.ImportPluginFromAgent(agent, item);
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
    /// Define the Azure OpenAI chat completion service (required).
    /// </summary>
    /// <returns><see cref="AssistantBuilder"/> instance for fluid expression.</returns>
    public AssistantBuilder WithAzureOpenAIChatCompletion(string deploymentName, string model, string endpoint, string apiKey)
    {
        this.Model.ExecutionSettings.ServiceId = deploymentName;
        this.Model.ExecutionSettings.Model = model;

        return this.WithAzureOpenAIChatCompletion(endpoint, apiKey);
    }

    /// <summary>
    /// Define the Azure OpenAI chat completion service (required).
    /// </summary>
    /// <returns><see cref="AssistantBuilder"/> instance for fluid expression.</returns>
    public AssistantBuilder WithAzureOpenAIChatCompletion(string endpoint, string apiKey)
    {
        this.ConfigureAIServices = (builder) =>
        {
            builder.AddAzureOpenAIChatCompletion(endpoint: endpoint, apiKey: apiKey, modelId: this.Model.ExecutionSettings.Model!, deploymentName: this.Model.ExecutionSettings.ServiceId!);
            builder.AddAzureOpenAITextGeneration(endpoint: endpoint, apiKey: apiKey, modelId: this.Model.ExecutionSettings.Model!, deploymentName: this.Model.ExecutionSettings.ServiceId!);
        };

        return this;
    }

    /// <summary>
    /// Adds a plugin to the agent.
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public AssistantBuilder WithPlugin(KernelPlugin plugin)
    {
        this._plugins.Add(plugin);
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
    /// Sets the logger factory to use.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public AssistantBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        this.KernelBuilder.Services.AddSingleton(loggerFactory);

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
    /// Creates a new agent builder from a yaml template.
    /// </summary>
    /// <param name="definitionPath">The yaml definition file path.</param>
    /// <param name="plugins">The plugins.</param>
    /// <param name="assistants">The assistants.</param>
    /// <param name="loggerFactory">The logger factory instance.</param>
    /// <returns></returns>
    public static AssistantBuilder FromTemplate(
        string definitionPath,
        IEnumerable<KernelPlugin>? plugins = null,
        ILoggerFactory? loggerFactory = null,
        params IAssistant[] assistants)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlContent = File.ReadAllText(definitionPath);

        var agentModel = deserializer.Deserialize<AssistantModel>(yamlContent);

        var agentBuilder = new AssistantBuilder();
        agentBuilder.Model = agentModel;

        if (plugins is not null)
        {
            foreach (var plugin in plugins)
            {
                agentBuilder.WithPlugin(plugin);
            }
        }

        if (assistants is not null)
        {
            foreach (var assistant in assistants)
            {
                agentBuilder.WithAssistant(assistant);
            }
        }

        if (loggerFactory is not null)
        {
            agentBuilder.WithLoggerFactory(loggerFactory);
        }

        return agentBuilder;
    }
}
