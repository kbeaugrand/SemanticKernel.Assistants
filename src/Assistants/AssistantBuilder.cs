// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using SemanticKernel.Assistants.Extensions;
using SemanticKernel.Assistants.Models;
using SemanticKernel.Assistants.RoomThread;
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
    /// The agent model.
    /// </summary>
    private readonly AssistantModel _model;

    /// <summary>
    /// The agent's assistants.
    /// </summary>
    private readonly List<IAssistant> _assistants;

    /// <summary>
    /// The agent's plugins.
    /// </summary>
    private readonly List<IKernelPlugin> _plugins;

    /// <summary>
    /// The logger factory.
    /// </summary>
    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;

    /// <summary>
    /// The kernel builder.
    /// </summary>
    private readonly KernelBuilder _kernelBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssistantBuilder"/> class.
    /// </summary>
    public AssistantBuilder()
    {
        this._model = new AssistantModel();
        this._assistants = new List<IAssistant>();
        this._kernelBuilder = new KernelBuilder();
        this._plugins = new List<IKernelPlugin>();
    }

    /// <summary>
    /// Builds the agent.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="KernelException"></exception>
    public IAssistant Build()
    {
        var kernel = this._kernelBuilder.Build();

        var agent = new Assistant(this._model, kernel);

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
        this._model.Name = name;
        return this;
    }

    /// <summary>
    /// Defines the agent's description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    public AssistantBuilder WithDescription(string description)
    {
        this._model.Description = description;
        return this;
    }

    /// <summary>
    /// Defines the agent's instructions.
    /// </summary>
    /// <param name="instructions">The instructions.</param>
    /// <returns></returns>
    public AssistantBuilder WithInstructions(string instructions)
    {
        this._model.Instructions = instructions;
        return this;
    }

    /// <summary>
    /// Define the Azure OpenAI chat completion service (required).
    /// </summary>
    /// <returns><see cref="AssistantBuilder"/> instance for fluid expression.</returns>
    public AssistantBuilder WithAzureOpenAIChatCompletion(string deploymentName, string model, string endpoint, string apiKey)
    {
        this._model.ExecutionSettings.DeploymentName = deploymentName;
        this._model.ExecutionSettings.Model = model;

        this._kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName, model, endpoint, apiKey);
        this._kernelBuilder.AddAzureOpenAITextGeneration(deploymentName, model, endpoint, apiKey);
        return this;
    }

    /// <summary>
    /// Adds a plugin to the agent.
    /// </summary>
    /// <param name="plugin"></param>
    /// <returns></returns>
    public AssistantBuilder WithPlugin(IKernelPlugin plugin)
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
        this._model.ExecutionSettings.Planner = plannerName;
        return this;
    }

    /// <summary>
    /// Sets the logger factory to use.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public AssistantBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        this._kernelBuilder.Services.AddSingleton(loggerFactory);

        return this;
    }

    /// <summary>
    /// Defines the agent's input parameter.
    /// </summary>
    /// <param name="inputParameter">The input parameter.</param>
    /// <returns></returns>
    public AssistantBuilder WithInputParameter(string description, string defaultValue = "")
    {
        this._model.Input = new AssistantInputParameter
        {
            DefaultValue = defaultValue,
            Description = description,
        };
        return this;
    }

    /// <summary>
    /// Creates a new agent from a yaml template.
    /// </summary>
    /// <param name="definitionPath">The yaml definition file path.</param>
    /// <param name="azureOpenAIEndpoint">The Azure OpenAI endpoint.</param>
    /// <param name="azureOpenAIKey">The Azure OpenAI key.</param>
    /// <param name="plugins">The plugins.</param>
    /// <param name="assistants">The assistants.</param>
    /// <param name="loggerFactory">The logger factory instance.</param>
    /// <returns></returns>
    public static IAssistant FromTemplate(
        string definitionPath,
        string azureOpenAIEndpoint,
        string azureOpenAIKey,
        IEnumerable<IKernelPlugin>? plugins = null,
        ILoggerFactory? loggerFactory = null,
        params IAssistant[] assistants)
    {
        var deserializer = new DeserializerBuilder().Build();
        var yamlContent = File.ReadAllText(definitionPath);

        var agentModel = deserializer.Deserialize<AssistantModel>(yamlContent);

        var agentBuilder = new AssistantBuilder()
            .WithName(agentModel.Name!.Trim())
            .WithDescription(agentModel.Description!.Trim())
            .WithInstructions(agentModel.Instructions.Trim())
            .WithPlanner(agentModel.ExecutionSettings.Planner?.Trim())
            .WithInputParameter(agentModel.Input.Description?.Trim()!, agentModel.Input.DefaultValue?.Trim()!)
            .WithAzureOpenAIChatCompletion(agentModel.ExecutionSettings.DeploymentName!, agentModel.ExecutionSettings.Model!, azureOpenAIEndpoint, azureOpenAIKey);

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

        return agentBuilder.Build();
    }

    /// <summary>
    /// Creates a new room thread for collaborative agents.
    /// </summary>
    /// <param name="agents">The collaborative agents.</param>
    /// <returns></returns>
    public static IRoomThread CreateRoomThread(params IAssistant[] agents)
    {
        return new RoomThread.RoomThread(agents);
    }
}
