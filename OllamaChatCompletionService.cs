using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.Text.RegularExpressions;

public class OllamaChatCompletionService : IChatCompletionService
{
    private readonly IOllamaApiClient _ollamaApiClient;

    public OllamaChatCompletionService(IOllamaApiClient ollamaApiClient)
    {
        _ollamaApiClient = ollamaApiClient;
    }

    public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        var request = CreateChatRequest(chatHistory);
        var content = new StringBuilder();
        var innerContent = new List<ChatResponseStream>();
        AuthorRole? authorRole = null;

        // Corrected method call
        await foreach (var response in _ollamaApiClient.ChatAsync(request, cancellationToken))
        {
            if (response?.Message == null) continue;

            innerContent.Add(response);

            if (response.Message.Content != null)
                content.Append(response.Message.Content);

            authorRole = GetAuthorRole(response.Message.Role);
        }

        return new List<ChatMessageContent>
        {
            new ChatMessageContent
            {
                Role = authorRole ?? AuthorRole.Assistant,
                Content = content.ToString(),
                InnerContent = innerContent,
                ModelId = "llama3:latest" // Update model name here
            }
        };
    }

    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = CreateChatRequest(chatHistory);
        var buffer = new StringBuilder();

        await foreach (var response in _ollamaApiClient.ChatAsync(request, cancellationToken))
        {
            if (response?.Message == null) continue;

            var content = response.Message.Content;
            if (string.IsNullOrEmpty(content)) continue;

            // Clean up any markdown code blocks for better console display
            content = CleanMarkdownCodeBlocks(content);
            
            yield return new StreamingChatMessageContent(
                role: GetAuthorRole(response.Message.Role) ?? AuthorRole.Assistant,
                content: content,
                innerContent: response,
                modelId: "llama3:latest"
            );
        }
    }

    private static AuthorRole? GetAuthorRole(ChatRole? role)
    {
        return role?.ToString().ToLowerInvariant() switch
        {
            "user" => AuthorRole.User,
            "assistant" => AuthorRole.Assistant,
            "system" => AuthorRole.System,
            _ => null
        };
    }

    private static ChatRequest CreateChatRequest(ChatHistory chatHistory)
    {
        var messages = new List<Message>();

        foreach (var message in chatHistory)
        {
            string role;
            if (message.Role == AuthorRole.User)
                role = "user";
            else if (message.Role == AuthorRole.Assistant)
                role = "assistant";
            else
                role = "system";

            messages.Add(new Message
            {
                Role = role,
                Content = message.Content,
            });
        }

        return new ChatRequest
        {
            Messages = messages,
            Stream = true,
            Model = "llama3:latest"
        };
    }

    private static string CleanMarkdownCodeBlocks(string content)
    {
        // Remove markdown code block syntax
        content = Regex.Replace(content, @"```[\w]*\n", "");
        content = Regex.Replace(content, @"```", "");
        
        // Normalize newlines
        content = content.Replace("\r\n", "\n").Replace("\r", "\n");
        
        return content;
    }
}
