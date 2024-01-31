// Copyright (c) Kevin BEAUGRAND. All rights reserved.

using HandlebarsDotNet;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SemanticKernel.Assistants.GroupThread;

internal class GroupThread : IGroupThread
{
    public IReadOnlyList<ChatMessageContent> ChatMessages => throw new NotImplementedException();

    private readonly Dictionary<IAssistant, IThread> _assistantThreads = new();

    public event EventHandler<ChatMessageContent>? OnMessageReceived;

    internal GroupThread(IEnumerable<IAssistant> agents)
    {
        this._assistantThreads = agents.ToDictionary(agent => agent, agent =>
        {
            var thread = agent.CreateThread();
            thread.AddSystemMessage(this.GetInstructions()(new
            {
                agent,
                participants = agents
            }));

            return thread;
        });

        this.OnMessageReceived += (sender, message) =>
        {
            var agent = sender as IAssistant;

            this.DispatchMessageRecievedAsync(agent!.Name!, message); // TODO fix to run it synchronously
        };
    }

    public async Task AddUserMessageAsync(string message)
    {
        await this.DispatchMessageRecievedAsync("User", new ChatMessageContent(AuthorRole.User, message)).ConfigureAwait(false);
    }

    /// <summary>
    /// Dispatches the message to all recipients.
    /// </summary>
    /// <param name="sender">The sender of the message (can be the agent name or "User").</param>
    /// <param name="message">The message..</param>
    /// <returns></returns>
    private async Task DispatchMessageRecievedAsync(string sender, ChatMessageContent message)
    {
        await Task.WhenAll(this._assistantThreads
                     .Where(c => c.Key.Name != sender)
                     .Select(async assistantThread =>
                     {
                         var response = await assistantThread.Value.InvokeAsync($"{sender} > {message}")
                                                                    .ConfigureAwait(false);

                         if (response.Content.Equals("[silence]", StringComparison.OrdinalIgnoreCase))
                         {
                             return;
                         }

                         this.OnMessageReceived!(assistantThread.Key, response);
                     })).ConfigureAwait(false);
    }

    private HandlebarsTemplate<object, object> GetInstructions()
    {
        var roomInstructionTemplate = this.ReadManifestResource("GroupThreadInstructions.handlebars");

        IHandlebars handlebarsInstance = Handlebars.Create(
           new HandlebarsConfiguration
           {
               NoEscape = true
           });

        var template = handlebarsInstance.Compile(roomInstructionTemplate);

        return template;
    }

    private string ReadManifestResource(string resourceName)
    {
        var promptStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(GroupThread).Namespace}.{resourceName}")!;

        using var reader = new StreamReader(promptStream);

        return reader.ReadToEnd();
    }
}
