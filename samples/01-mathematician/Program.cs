// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using _01_mathematician.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernel.Assistants;
using Spectre.Console;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.development.json", optional: true)
    .Build();

using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging
        .AddConsole(opts =>
        {
            opts.FormatterName = "simple";
        })
        .AddConfiguration(configuration.GetSection("Logging"));
});

AnsiConsole.Write(new FigletText($"Copilot").Color(Color.Green));
AnsiConsole.WriteLine("");

IAssistant assistant = null!;

AnsiConsole.Status().Start("Initializing...", ctx =>
{
    string azureOpenAIEndpoint = configuration["AzureOpenAIEndpoint"]!;
    string azureOpenAIDeploymentName = configuration["AzureOpenAIGPT35Endpoint"]!;
    string azureOpenAIKey = configuration["AzureOpenAIAPIKey"]!;
    string ollamaEndpoint = configuration["OllamaEndpoint"]!;

    var financialKernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName, azureOpenAIEndpoint, azureOpenAIKey)
                    .Build();

    financialKernel.CreatePluginFromObject(new FinancialPlugin(), "financial");

    var butlerKernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName, azureOpenAIEndpoint, azureOpenAIKey)
                    .Build();

    var financialCalculator = AssistantBuilder.FromTemplate("./Assistants/FinancialCalculator.yaml")
        .WithKernel(financialKernel)
                    .Build();

    assistant = AssistantBuilder.FromTemplate("./Assistants/Butler.yaml",
           assistants: new IAssistant[]
           {
                financialCalculator
           })
        .WithKernel(butlerKernel)
        .Build();
});

var thread = assistant.CreateThread();

while (true)
{
    var prompt = AnsiConsole.Prompt(new TextPrompt<string>("User > ").PromptStyle("teal"));

    await AnsiConsole.Status().StartAsync("Processing...", async ctx =>
    {
        ctx.Spinner(Spinner.Known.Star);
        ctx.SpinnerStyle(Style.Parse("green"));
        ctx.Status($"Processing ...");

        var answer = await thread.InvokeAsync(prompt).ConfigureAwait(true);

        AnsiConsole.MarkupLine($"[cyan]Copilot > {answer.Content!}\n[/]");
    });
}
