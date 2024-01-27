using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;

namespace SemanticKernel.Assistants.Ollama
{
    public static class AssistantBuilderExtensions
    {
        /// <summary>
        /// Define the Ollama API as completion service (required).
        /// </summary>
        /// <returns><see cref="AssistantBuilder"/> instance for fluid expression.</returns>
        public static AssistantBuilder WithOllamaChatCompletion(this AssistantBuilder assistant, string endpoint)
        {
            assistant.ConfigureAIServices = (builder) =>
            {
                assistant.KernelBuilder.Services.AddHttpClient<HttpClient>();

                assistant.KernelBuilder.AddOllamaChatCompletion(baseUrl: endpoint, modelId: assistant.Model.ExecutionSettings.Model!, serviceId: assistant.Model.ExecutionSettings.ServiceId!);
                assistant.KernelBuilder.AddOllamaTextGeneration(baseUrl: endpoint, modelId: assistant.Model.ExecutionSettings.Model!, serviceId: assistant.Model.ExecutionSettings.ServiceId!);
            };

            return assistant;
        }

        /// <summary>
        /// Define the Ollama API as completion service (required).
        /// </summary>
        /// <returns><see cref="AssistantBuilder"/> instance for fluid expression.</returns>
        public static AssistantBuilder WithOllamaChatCompletion(this AssistantBuilder assistant, string endpoint, Action<HttpClient> httpClientOptions)
        {
            assistant.ConfigureAIServices = (builder) =>
            {
                assistant.KernelBuilder.Services.AddHttpClient(assistant.Model.Name!, httpClientOptions);

                assistant.KernelBuilder.AddOllamaChatCompletion(baseUrl: endpoint, modelId: assistant.Model.ExecutionSettings.Model!, serviceId: assistant.Model.ExecutionSettings.ServiceId!);
                assistant.KernelBuilder.AddOllamaTextGeneration(baseUrl: endpoint, modelId: assistant.Model.ExecutionSettings.Model!, serviceId: assistant.Model.ExecutionSettings.ServiceId!);
            };

            return assistant;
        }
    }
}
