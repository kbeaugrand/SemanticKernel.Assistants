// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using _01_mathematician.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
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

var thread = mathematician.CreateThread();

while (true)
{
    Console.Write("User > ");
    var result = await thread.InvokeAsync(Console.ReadLine());
    Console.WriteLine($"Mathematician > {result.Content.Trim()}");
}
