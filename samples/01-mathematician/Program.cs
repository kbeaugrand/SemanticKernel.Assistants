// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using _01_mathematician.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernel.Assistants;
using SemanticKernel.Assistants.Ollama;
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
    string azureOpenAIKey = configuration["AzureOpenAIAPIKey"]!;
    string ollamaEndpoint = configuration["OllamaEndpoint"]!;

    var financialCalculator = AssistantBuilder.FromTemplate("./Assistants/FinancialCalculator.yaml",
        plugins: new List<KernelPlugin>()
        {
            KernelPluginFactory.CreateFromObject(new FinancialPlugin(), "financial")
        }, loggerFactory: loggerFactory)
                //.WithAzureOpenAIChatCompletion(azureOpenAIEndpoint, azureOpenAIKey)
                .WithOllamaChatCompletion(ollamaEndpoint, client => { 
                    client.Timeout = TimeSpan.FromMinutes(5);
                })
                .Build();

    assistant = AssistantBuilder.FromTemplate("./Assistants/Butler.yaml",
           assistants: new IAssistant[]
           {
                financialCalculator
           }, loggerFactory: loggerFactory)
                //.WithAzureOpenAIChatCompletion(azureOpenAIEndpoint, azureOpenAIKey)
                .WithOllamaChatCompletion(ollamaEndpoint, client => {
                    client.Timeout = TimeSpan.FromMinutes(5);
                })
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
