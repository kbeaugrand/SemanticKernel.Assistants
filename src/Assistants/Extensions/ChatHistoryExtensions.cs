using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Collections.Generic;

namespace SemanticKernel.Assistants.Extensions
{
    internal static class ChatHistoryExtensions
    {
        public static void AddFunctionMessage(this ChatHistory chatHistory, string message, string functionName)
        {
            chatHistory.AddMessage(new("function"), message, metadata: new Dictionary<string, object?>(1) { { OpenAIChatMessageContent.ToolCallsProperty, functionName } });
        }
    }
}
