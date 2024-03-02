// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planning.Handlebars;
using SemanticKernel.Assistants.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticKernel.Assistants;

/// <summary>
/// Represents a thread of conversation with an agent.
/// </summary>
public class Thread : IThread
{
    /// <summary>
    /// The agent.
    /// </summary>
    private readonly IAssistant _agent;

    /// <summary>
    /// The chat history of this thread.
    /// </summary>
    private readonly ChatHistory _chatHistory;

    /// <summary>
    /// The settings for the OpenAI prompt execution.
    /// </summary>
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;

    /// <summary>
    /// The prompt to use for extracting the user intent.
    /// </summary>
    private const string SystemIntentExtractionPrompt =
        "Rewrite the last messages to reflect the user's intents, taking into consideration the provided chat history. " +
        "The output should be rewritten sentences that describes the user's intent and is understandable outside of the context of the chat history, in a way that will be useful for executing an action. " +
        "Do not try to find an answer, just extract the user intent and all needed information to execute the goal.";

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// The arguments to pass to the agent.
    /// </summary>
    private Dictionary<string, object?> _arguments;

    /// <summary>
    /// The name of the caller.
    /// </summary>
    private readonly string _callerName;

    /// <summary>
    /// Gets the chat messages.
    /// </summary>
    public IReadOnlyList<ChatMessageContent> ChatMessages => this._chatHistory;

    /// <summary>
    /// Initializes a new instance of the <see cref="Thread"/> class.
    /// </summary>
    /// <param name="agent">The agent.</param>
    /// <param name="arguments">The arguments to pass.</param>
    internal Thread(IAssistant agent,
        Dictionary<string, object?> arguments = null)
    {
        this._logger = agent.Kernel.LoggerFactory.CreateLogger<Thread>();
        this._agent = agent;
        this._callerName = "User";
        this._arguments = arguments ?? new Dictionary<string, object?>();
        this._chatHistory = new ChatHistory(this._agent.Description!);

        this._chatHistory.AddSystemMessage(this._agent.Instructions);

        this._openAIPromptExecutionSettings = new()
        {
            Temperature = this._agent.AssistantModel.ExecutionSettings.PromptExecutionSettings.Temperature,
            TopP = this._agent.AssistantModel.ExecutionSettings.PromptExecutionSettings.TopP,
            FrequencyPenalty = this._agent.AssistantModel.ExecutionSettings.PromptExecutionSettings.FrequencyPenalty,
            PresencePenalty = this._agent.AssistantModel.ExecutionSettings.PromptExecutionSettings.PresencePenalty,
            MaxTokens = this._agent.AssistantModel.ExecutionSettings.PromptExecutionSettings.MaxTokens,
            StopSequences = this._agent.AssistantModel.ExecutionSettings.PromptExecutionSettings.StopSequences
        };
    }

    /// <summary>
    /// Invoke the agent completion.
    /// </summary>
    /// <returns></returns>
    public async Task<ChatMessageContent> InvokeAsync(string userMessage)
    {
        this._logger.LogInformation($"{this._callerName} > {this._agent.Name}:\n{userMessage}");

        ChatMessageContent assistantAnswer;

        if (!string.IsNullOrEmpty(this._agent.Planner))
        {
            assistantAnswer = new ChatMessageContent(AuthorRole.Assistant, await this.GetPlannerAnswer(userMessage).ConfigureAwait(false));
        }
        else
        {
            assistantAnswer = new ChatMessageContent(AuthorRole.Assistant, await this.GetChatAnswer(userMessage).ConfigureAwait(false));
        }

        this._chatHistory.AddUserMessage(userMessage);
        this._chatHistory.Add(assistantAnswer);

        this._logger.LogInformation(message: $"{this._agent.Name!} > {this._callerName}:\n{assistantAnswer.Content}");

        return assistantAnswer;
    }

    /// <summary>
    /// Adds the system message to the chat history.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void AddSystemMessage(string message)
    {
        this._chatHistory.AddSystemMessage(message);
    }

    /// <summary>
    /// Adds the user message to the chat history.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void AddUserMessage(string message)
    {
        this._chatHistory.AddUserMessage(message);
    }

    /// <summary>
    /// Adds the assistant message to the chat history.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void AddAssistantMessage(string message)
    {
        this._chatHistory.AddAssistantMessage(message);
    }

    /// <summary>
    /// Returns the answer from the planner based on the user intent.
    /// </summary>
    /// <param name="userMessage">The latest user message.</param>
    /// <returns></returns>
    private async Task<string> GetPlannerAnswer(string userMessage)
    {
        var userIntent = await this.ExtractUserIntentAsync(userMessage)
                                    .ConfigureAwait(false);

        var plannerResult = await this.ExecutePlannerAsync(userIntent).ConfigureAwait(false);

        this._logger.LogTrace($"Planner result: {plannerResult}");

        return plannerResult;
    }

    /// <summary>
    /// Returns the answer from the assistant based on the chat history and assistant persona.
    /// </summary>
    /// <param name="userMessage">The latest user message.</param>
    /// <returns></returns>
    private async Task<string> GetChatAnswer(string userMessage)
    {
        var chatHistory = this.GetPastMessagesHistory();

        chatHistory.AddUserMessage(userMessage);

        var agentAnswer = await this._agent.ChatCompletion
                                .GetChatMessageContentAsync(chatHistory, executionSettings: this._openAIPromptExecutionSettings)
                                .ConfigureAwait(false);

        return agentAnswer.Content!;
    }

    /// <summary>
    /// Extracts the user intent from the chat history.
    /// </summary>
    /// <param name="userMessage">The user message.</param>
    /// <returns></returns>
    private async Task<string> ExtractUserIntentAsync(string userMessage)
    {
        var chat = new ChatHistory(SystemIntentExtractionPrompt);

        this._chatHistory.OrderByDescending(c => this._chatHistory.IndexOf(c))
            .Where(c => c.Role == AuthorRole.User || c.Role == AuthorRole.Assistant)
            .Take(this._agent.AssistantModel.ExecutionSettings.PastMessagesIncluded)
            .OrderBy(c => this._chatHistory.IndexOf(c))
            .ToList()
            .ForEach(chat.Add);

        chat.AddUserMessage(userMessage);

        var chatResult = await this._agent.ChatCompletion.GetChatMessageContentAsync(chat).ConfigureAwait(false);

        return chatResult.Content!;
    }

    /// <summary>
    /// Takes all the system messages from the chat history. Add this._agent.AssistantModel.ExecutionSettings.PastMessagesIncluded messages and return the corresponding history.
    /// </summary>
    /// <returns></returns>
    private ChatHistory GetPastMessagesHistory()
    {
        var result = new ChatHistory();

        this._chatHistory.OrderByDescending(c => this._chatHistory.IndexOf(c))
            .Where(c => c.Role == AuthorRole.User || c.Role == AuthorRole.Assistant || c.Role == AuthorRole.System)
            .Take(this._agent.AssistantModel.ExecutionSettings.PastMessagesIncluded)
            .OrderBy(c => this._chatHistory.IndexOf(c))
            .ToList()
            .ForEach(result.Add);

        return result;
    }

    private async Task<string> ExecutePlannerAsync(string userIntent)
    {
        var goal = $"{this._agent.Instructions}\n" +
                            $"Given the following context, accomplish the user intent.\n" +
                            $"## User intent\n" +
                            $"{userIntent}\n" +
                            $"## Current parameters\n" +
                            $"{string.Join(Environment.NewLine, this._arguments.Select(c => $"{c.Key}:\n{c.Value}"))}";

        switch (this._agent.Planner.ToLower())
        {
            case "handlebars":
                return await this.ExecuteHandleBarsPlannerAsync(goal).ConfigureAwait(false);
            case "stepwise":
                return await this.ExecuteStepwisePlannerAsync(goal).ConfigureAwait(false);
            default:
                throw new NotImplementedException($"Planner {this._agent.Planner} is not implemented.");
        }
    }

    private async Task<string> ExecuteHandleBarsPlannerAsync(string goal, int maxTries = 3)
    {
        HandlebarsPlan? lastPlan = null;
        Exception? lastError = null;

        while (maxTries > 0)
        {
            try
            {
                var planner = new HandlebarsPlanner(new()
                {
                    LastPlan = lastPlan, // Pass in the last plan in case we want to try again
                    LastError = lastError?.Message // Pass in the last error to avoid trying the same thing again
                });

                HandlebarsPlan plan = null!;

                if (!string.IsNullOrEmpty(this._agent.AssistantModel.ExecutionSettings.HandleBarsPlannerSettings.FixedPlan))
                {
                    plan = new HandlebarsPlan(this._agent.AssistantModel.ExecutionSettings.HandleBarsPlannerSettings.FixedPlan!);
                }
                else
                {
                    plan = await planner.CreatePlanAsync(this._agent.Kernel, goal).ConfigureAwait(false);
                    lastPlan = plan;
                }

                this._logger.LogDebug($"Plan: {plan}");

                var result = await plan.InvokeAsync(this._agent.Kernel, new KernelArguments(this._arguments)).ConfigureAwait(false);

                return result!.Trim();
            }
            catch (Exception e)
            {
                // If we get an error, try again
                lastError = e;

                this._logger.LogWarning(e.Message);
            }
            maxTries--;
        }

        this._logger.LogError(lastError!, lastError!.Message);
        this._logger.LogError(lastPlan?.ToString());

        throw lastError;
    }

    private async Task<string> ExecuteStepwisePlannerAsync(string goal)
    {
        var config = new FunctionCallingStepwisePlannerOptions
        {
            MaxIterations = this._agent.AssistantModel.ExecutionSettings.SetpwisePlannerSettings.MaxIterations,
            MaxTokens = this._agent.AssistantModel.ExecutionSettings.SetpwisePlannerSettings.MaxTokens,
            ExecutionSettings = this._openAIPromptExecutionSettings
        };

        var planner = new FunctionCallingStepwisePlanner(config);

        var result = await planner.ExecuteAsync(this._agent.Kernel, goal).ConfigureAwait(false);

        return result.FinalAnswer!.Trim();
    }

    void IThread.UpdateKernelArguments(KernelArguments arguments)
    {
        this._arguments = arguments.ToDictionary(); ;
    }
}
