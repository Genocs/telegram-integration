using Genocs.TelegramIntegration.Configurations;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Genocs.TelegramIntegration.Services;

public class OpenAIMiddleware : IOpenAIMiddleware
{
    private readonly OpenAISettings _openAIOptions;

    public OpenAIMiddleware(IOptions<OpenAISettings> openAIOptions)
    {
        if (openAIOptions == null) throw new ArgumentNullException(nameof(openAIOptions));
        if (openAIOptions.Value == null) throw new ArgumentNullException(nameof(openAIOptions.Value));

        _openAIOptions = openAIOptions.Value;
    }

    public async Task<string?> ValidateDocumentAsync(string imageUrl)
    {
        ChatClient chatClient = new ChatClient(model: "GPT4_Vision", _openAIOptions.APIKey);

        // Create Conversation
        ChatCompletionOptions options = new()
        {
            MaxOutputTokenCount = 128,
            Temperature = 0.7F,
        };

        List<ChatMessage> messages =
        [
            new SystemChatMessage(_openAIOptions.SystemMessage),
            new UserChatMessage("Is it a valid document?", ChatMessageContentPart.CreateImagePart(new Uri(imageUrl), ChatImageDetailLevel.High))
        ];

        ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options);

        return completion.Content[0].Text;

    }

    public async Task<string?> ChatWithGPTAsync(string userChat)
    {
        ChatClient chatClient = new ChatClient(model: "gpt-4o", _openAIOptions.APIKey);

        ChatCompletion completion = await chatClient.CompleteChatAsync(userChat);

        return completion.Content[0].Text;
    }
}