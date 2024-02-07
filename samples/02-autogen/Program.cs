// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using _02_autogen.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernel.Assistants;
using SemanticKernel.Assistants.Extensions;
using Spectre.Console;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.development.json", optional: true)
    .Build();

using var loggerFactory = LoggerFactory.Create(logging =>
{
    logging
        .AddDebug()
        .AddConfiguration(configuration.GetSection("Logging"));
});

AnsiConsole.Write(new FigletText($"DA Copilot").Color(Color.Green));
AnsiConsole.WriteLine("");

IAssistant assistant = null!;

AnsiConsole.Status().Start("Initializing...", ctx =>
{
    string azureOpenAIEndpoint = configuration["AzureOpenAIEndpoint"]!;
    string azureOpenAIGPT4DeploymentName = configuration["AzureOpenAIGPT4DeploymentName"]!;
    string azureOpenAIGPT35DeploymentName = configuration["AzureOpenAIGPT35DeploymentName"]!;
    string azureOpenAIKey = configuration["AzureOpenAIAPIKey"]!;
    string ollamaEndpoint = configuration["OllamaEndpoint"]!;

    var codeInterpretionOptions = new CodeInterpretionPluginOptions();
    configuration!.Bind("CodeInterpreter", codeInterpretionOptions);

    var butlerKernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(azureOpenAIGPT4DeploymentName, azureOpenAIEndpoint, azureOpenAIKey)
                    .Build();

    butlerKernel.ImportPluginFromObject(new FileAccessPlugin(codeInterpretionOptions.OutputFilePath, loggerFactory), "file");
    butlerKernel.ImportPluginFromAssistant(CreateCodeInterpreter(codeInterpretionOptions, azureOpenAIGPT35DeploymentName, azureOpenAIEndpoint, azureOpenAIKey));

    assistant = AssistantBuilder.FromTemplate("./Assistants/AssistantAgent.yaml")
        .WithKernel(butlerKernel)
        .Build();
});

var options = configuration.GetRequiredSection("CodeInterpreter");

var thread = assistant.CreateThread();

while (true)
{
    var prompt = AnsiConsole.Prompt(new TextPrompt<string>("User > ").PromptStyle("teal"));

    await AnsiConsole.Status().StartAsync("Creating...", async ctx =>
    {
        ctx.Spinner(Spinner.Known.Star);
        ctx.SpinnerStyle(Style.Parse("green"));
        ctx.Status($"Processing ...");

        var answer = await thread.InvokeAsync(prompt).ConfigureAwait(true);

        AnsiConsole.MarkupLine($"AutoGen > [cyan]{answer.Content!}\n[/]");
    });
}

IAssistant CreateCodeInterpreter(CodeInterpretionPluginOptions codeInterpretionOptions, string azureOpenAIDeploymentName, string azureOpenAIEndpoint, string azureOpenAIKey)
{
    var kernel = Kernel.CreateBuilder()
                        .AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName, azureOpenAIEndpoint, azureOpenAIKey)
                        .Build();

    kernel.ImportPluginFromObject(new CodeInterpretionPlugin(codeInterpretionOptions, loggerFactory), "code");

    return AssistantBuilder.FromTemplate("./Assistants/CodeInterpreter.yaml")
        .WithKernel(kernel)
        .Build();
}