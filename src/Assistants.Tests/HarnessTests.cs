﻿// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernel.Assistants;
using SemanticKernel.Assistants.Extensions;
using SemanticKernel.Assistants.Tests.Plugins;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SemanticKernel.Experimental.Agents.Tests;

public class HarnessTests
{
#if DISABLEHOST
    private const string SkipReason = "Harness only for local/dev environment";
#else
    private const string SkipReason = null;
#endif

    private readonly ITestOutputHelper _output;

    private readonly ILoggerFactory _loggerFactory;

    public HarnessTests(ITestOutputHelper output)
    {
        this._output = output;

        this._loggerFactory = LoggerFactory.Create(logging =>
        {
            logging
                .AddProvider(new XunitLoggerProvider(output))
                .AddConfiguration(TestConfig.Configuration.GetSection("Logging"));
        });
    }

    [Fact(Skip = SkipReason)]
    public async Task PoetTestAsync()
    {
        string azureOpenAIKey = TestConfig.AzureOpenAIAPIKey;
        string azureOpenAIEndpoint = TestConfig.AzureOpenAIEndpoint;
        string azureOpenAIChatCompletionDeployment = TestConfig.AzureOpenAIDeploymentName;

        var assistant = new AssistantBuilder()
                            .WithName("poet")
                            .WithDescription("An assistant that create sonnet poems.")
                            .WithInstructions("You are a poet that composes poems based on user input.\nCompose a sonnet inspired by the user input.")
                            .WithKernel(Kernel.CreateBuilder()
                                        .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                                        .Build())
                            .Build();

        var thread = assistant.CreateThread();

        var response = await thread.InvokeAsync("Eggs are yummy and beautiful geometric gems.").ConfigureAwait(true);
    }

    [Theory(Skip = SkipReason)]
    [InlineData("What is the square root of 4?")]
    [InlineData("If you start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, how much would you have?")]
    public async Task MathCalculationTestsAsync(string prompt)
    {
        string azureOpenAIKey = TestConfig.AzureOpenAIAPIKey;
        string azureOpenAIEndpoint = TestConfig.AzureOpenAIEndpoint;
        string azureOpenAIChatCompletionDeployment = TestConfig.AzureOpenAIDeploymentName;

        var mathKernel = Kernel.CreateBuilder()
                                        .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                                        .Build();

        mathKernel.ImportPluginFromObject(new MathPlugin());

        var mathematician = new AssistantBuilder()
                            .WithName("mathematician")
                            .WithDescription("An assistant that answers math problems.")
                            .WithInstructions("You are a mathematician.\n" +
                                              "No need to show your work, just give the answer to the math problem.\n" +
                                              "Use calculation results.")
                            .WithKernel(mathKernel)
                            .Build();

        var thread = mathematician.CreateThread();

        var response = await thread.InvokeAsync(prompt).ConfigureAwait(true);
    }

    [Fact(Skip = SkipReason)]
    public async Task ButlerTestAsync()
    {
        string azureOpenAIKey = TestConfig.AzureOpenAIAPIKey;
        string azureOpenAIEndpoint = TestConfig.AzureOpenAIEndpoint;
        string azureOpenAIChatCompletionDeployment = TestConfig.AzureOpenAIDeploymentName;

        var mathKernel = Kernel.CreateBuilder()
                                       .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                                       .Build();

        mathKernel.ImportPluginFromObject(new MathPlugin());

        var mathematician = new AssistantBuilder()
                            .WithName("mathematician")
                            .WithDescription("An assistant that answers math problems with a given user prompt.")
                            .WithInstructions("You are a mathematician.\n" +
                                              "No need to show your work, just give the answer to the math problem.\n" +
                                              "Use calculation results.")
                            .WithKernel(mathKernel)
                            .WithInputParameters(new Assistants.Models.AssistantInputParameter
                            {
                                Name = "input",
                                Description = "The word mathematics problem to solve in 2-3 sentences.\n" +
                                              "Make sure to include all the input variables needed along with their values and units otherwise the math function will not be able to solve it."
                            })
                            .Build();

        var butlerKernel = Kernel.CreateBuilder()
                                        .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                                        .Build();

        butlerKernel.ImportPluginFromAssistant(mathematician);

        var butler = new AssistantBuilder()
                    .WithName("alfred")
                    .WithDescription("An AI butler that helps humans.")
                    .WithInstructions("Act as a butler.\nNo need to explain further the internal process.\nBe concise when answering.")
                    .WithKernel(butlerKernel)
                    .Build();

        var thread = butler.CreateThread();

        var response = await thread.InvokeAsync("If I start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, how much would I have?").ConfigureAwait(true);
    }

    [Fact(Skip = SkipReason)]
    public async Task FinancialAdvisorFromTemplateTestsAsync()
    {
        string azureOpenAIKey = TestConfig.AzureOpenAIAPIKey;
        string azureOpenAIEndpoint = TestConfig.AzureOpenAIEndpoint;
        string azureOpenAIChatCompletionDeployment = TestConfig.AzureOpenAIDeploymentName;

        var financialKernel = Kernel.CreateBuilder()
                                       .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                                       .Build();

        financialKernel.ImportPluginFromObject(new FinancialPlugin());

        var mathematician = AssistantBuilder.FromTemplate("./Assistants/Mathematician.yaml")
            .WithKernel(financialKernel)
            .Build();

        var financial = AssistantBuilder.FromTemplate("./Assistants/FinancialCalculator.yaml")
            .WithKernel(financialKernel)
            .Build();

        var butlerKernel = Kernel.CreateBuilder()
                              .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                              .Build();

        butlerKernel.ImportPluginFromAssistant(mathematician);
        butlerKernel.ImportPluginFromAssistant(financial);

        var butler = AssistantBuilder.FromTemplate("./Assistants/Butler.yaml")
            .WithKernel(butlerKernel)
            .Build();

        var thread = butler.CreateThread();
        var question = "If I start with $25,000 in the stock market and leave it to grow for 20 years at a 5% annual interest rate, how much would I have?";

        var result = await thread.InvokeAsync(question)
            .ConfigureAwait(true);

        await this.AuditorTestsAsync(question, result.Content!, "If you start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, the future value of the investment would be approximately $66,332.44.", true).ConfigureAwait(true);

        result = await thread.InvokeAsync("What if the rate is about 3.6%?").ConfigureAwait(true);
        await this.AuditorTestsAsync(question + "\nWhat if the rate is about 3.6%?", result.Content!, "If you start with $25,000 in the stock market and leave it to grow for 20 years at a 3.6% interest rate, the future value of the investment would be approximately $50,714.85.", true).ConfigureAwait(true);
    }

    [Theory(Skip = SkipReason)]
    [InlineData(
        "If I start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, how much would I have?", 
        "The future value of $25,000 invested at a 5% interest rate for 20 years would be approximately $66,332.44.", 
        "If you start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, the future value of the investment would be approximately $66,332.44.", 
        true)]
    [InlineData(
        "If I start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, how much would I have?", 
        "If the interest rate is 3.6%, the future value of the $25,000 investment over 20 years would be approximately $47,688.04.", 
        "If you start with $25,000 in the stock market and leave it to grow for 20 years at a 5% interest rate, the future value of the investment would be approximately $66,332.44.", 
        false)]
    public async Task AuditorTestsAsync(
        string question,
        string answer1,
        string answer2,
        bool equality)
    {
        string azureOpenAIKey = TestConfig.AzureOpenAIAPIKey;
        string azureOpenAIEndpoint = TestConfig.AzureOpenAIEndpoint;
        string azureOpenAIChatCompletionDeployment = TestConfig.AzureOpenAIDeploymentName;

        var verifier = AssistantBuilder.FromTemplate("./Assistants/Auditor.yaml")
            .WithKernel(Kernel.CreateBuilder()
                              .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                              .Build())
            .Build();

        var result = await verifier.CreateThread()
            .InvokeAsync(
            $"Question: {question}\n" +
            $"Answer 1: {answer1}\n" +
            $"Answer 2: {answer2}")
                .ConfigureAwait(true);

        Assert.Equal(equality, bool.Parse(result.Content!));
    }


    [Theory(Skip = SkipReason)]
    [InlineData("What is the square of 16?")]
    [InlineData("What is the square root of 16?")]
    [InlineData("Help me to find the how I will have in my account in 2 years.")]


    public async Task RoomMeetingSampleTestAsync(string prompt)
    {
        string azureOpenAIKey = TestConfig.AzureOpenAIAPIKey;
        string azureOpenAIEndpoint = TestConfig.AzureOpenAIEndpoint;
        string azureOpenAIChatCompletionDeployment = TestConfig.AzureOpenAIDeploymentName;

        var mathematicianKernel = Kernel.CreateBuilder()
                               .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                               .Build();

        mathematicianKernel.ImportPluginFromObject(new MathPlugin());

        var mathematician = AssistantBuilder.FromTemplate("./Assistants/Mathematician.yaml")
            .WithKernel(mathematicianKernel)
            .Build();

        var butler = AssistantBuilder.FromTemplate("./Assistants/Butler.yaml")
            .WithKernel(Kernel.CreateBuilder()
                              .AddAzureOpenAIChatCompletion(azureOpenAIChatCompletionDeployment, azureOpenAIEndpoint, azureOpenAIKey)
                              .Build())
            .Build();

        var logger = this._loggerFactory.CreateLogger("Tests");

        var thread = Assistant.CreateGroupThread(butler, mathematician);

        thread.OnMessageReceived += (object? sender, ChatMessageContent message) =>
        {
            var agent = sender as IAssistant;
            this._output.WriteLine($"{agent!.Name} > {message}");
        };

        this._output.WriteLine($"User > {prompt}");

        await thread.AddUserMessageAsync(prompt)
            .ConfigureAwait(true);
    }
}
