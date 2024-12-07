using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using System.Text;

var builder = Kernel.CreateBuilder();

builder.Services.AddScoped<IOllamaApiClient>(_ => new OllamaApiClient("http://localhost:11434"));

builder.Services.AddScoped<IChatCompletionService, OllamaChatCompletionService>();

var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();

Console.WriteLine(" Welcome to Ollama Chat!");
Console.WriteLine("Type 'exit' to end the conversation\n");

history.AddSystemMessage("You are a helpful assistant that provides clear and concise answers.");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("\n🧑 You: ");
    Console.ForegroundColor = ConsoleColor.White;
    
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage) || userMessage.ToLower() == "exit")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nGoodbye! 👋");
        Console.ResetColor();
        break;
    }

    history.AddUserMessage(userMessage);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("\n🤖 Assistant: ");
    Console.ForegroundColor = ConsoleColor.White;

    var response = new StringBuilder();
    
    try
    {
        await foreach (var message in chatService.GetStreamingChatMessageContentsAsync(history))
        {
            Console.Write(message.Content);
            response.Append(message.Content);
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n❌ Error: {ex.Message}");
        Console.ResetColor();
        continue;
    }

    history.AddAssistantMessage(response.ToString());
}
