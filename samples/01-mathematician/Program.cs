// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernel.Assistants;

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
        .ClearProviders()
        .AddConfiguration(configuration.GetSection("Logging"));
});

string azureOpenAIEndpoint = configuration["AzureOpenAIEndpoint"]!;
string azureOpenAIKey = configuration["AzureOpenAIAPIKey"]!;

var mathematician = AssistantBuilder.FromTemplate("./Assistants/Mathematician.yaml",
    azureOpenAIEndpoint,
    azureOpenAIKey,
    plugins: new List<IKernelPlugin>()
    {
       KernelPluginFactory.CreateFromObject(new MathPlugin(), "math")
    });

var agent = AssistantBuilder.FromTemplate("./Assistants/Butler.yaml",
    azureOpenAIEndpoint,
    azureOpenAIKey, 
    assistants: mathematician);

var thread = agent.CreateThread();

while (true)
{
    Console.Write("User > ");
    await thread.InvokeAsync(Console.ReadLine());
}
