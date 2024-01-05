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

    var mathematician = Assistant.FromTemplate("./Assistants/Mathematician.yaml",
        azureOpenAIEndpoint,
        azureOpenAIKey,
        plugins: new List<IKernelPlugin>()
        {3. Create a new conversation thread with your assistant.

           KernelPluginFactory.CreateFromObject(new MathPlugin(), "math")
        });
   ```
   ```csharp
   var thread = mathematician.CreateThread();
   await thread.InvokeAsync("Your ask to the assistant.");
   ```

## License

This project is licensed under the [MIT License](LICENSE).