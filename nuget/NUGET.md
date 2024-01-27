# Assistants for Semantic Kernel

This is assistant support for the [Semantic Kernel](https://aka.ms/semantic-kernel).

This enables the usage of assistants for the Semantic Kernel **without relying on OpenAI Assistant APIs**.
It runs locally planners and plugins for the assistants.

It provides different scenarios for the usage of assistants such as:
- **Assistant with Semantic Kernel plugins**
- **Multi-Assistant conversation**

## Usage

1. Create you agent description file in yaml: 
    ```yaml
    name: Mathematician
    description: A mathematician that resolves given maths problems.
    instructions: |
      You are a mathematician.
      Given a math problem, you must answer it with the best calculation formula.
      No need to show your work, just give the answer to the math problem.
      Use calculation results.
    input_parameter: 
      default_value: ""
      description: |
          The word mathematics problem to solve in 2-3 sentences.
          Make sure to include all the input variables needed along with their values and units otherwise the math function will not be able to solve it.
    execution_settings:
      planner: Handlebars
      model: gpt-3.5-turbo
      service_id: gpt-35-turbo-1106
      prompt_settings: 
        temperature: 0.0
        top_p: 1
        max_tokens: 100
    ```
2. Instanciate your assistant in your code: 
   ```csharp
    string azureOpenAIEndpoint = configuration["AzureOpenAIEndpoint"]!;
    string azureOpenAIKey = configuration["AzureOpenAIAPIKey"]!;

    var mathematician = AssistantBuilder.FromTemplate("./Assistants/Mathematician.yaml",
        plugins: new List<IKernelPlugin>()
        {
           KernelPluginFactory.CreateFromObject(new MathPlugin(), "math")
        })
        .WithAzureOpenAIChatCompletion(azureOpenAIEndpoint, azureOpenAIKey)
        .Build();
   ```
   ```csharp
   var thread = mathematician.CreateThread();
   await thread.InvokeAsync("Your ask to the assistant.");
   ```

## Ollama Suport

This assistant supports the [Ollama](https://ollama.ai/) platform, giving you the ability to use the assistant by hosting easyly your LLM models locally.

To use Ollama, install the Ollama extension package: 

```dotnetcli
dotnet add package SemanticKernel.Assistants --version 1.2.0-preview

```

Then, instanciate your assistant with the Ollama extension: 

```csharp
var mathematician = AssistantBuilder.FromTemplate("./Assistants/Mathematician.yaml",
        plugins: new List<IKernelPlugin>()
        {
           KernelPluginFactory.CreateFromObject(new MathPlugin(), "math")
        })
        .WithOllamaChatCompletion(ollamaEndpoint, client => { 
            client.Timeout = TimeSpan.FromMinutes(5);
        })
        .Build();
```

## License

This project is licensed under the [MIT License](LICENSE).