using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Models;

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

    public async Task<string?> ValidateTaxFreeFormAsync(string imageUrl)
    {
        var apiClient = new OpenAIAPI(_openAIOptions.APIKey);

        // Create Conversation
        var chat = apiClient.Chat.CreateConversation();
        chat.Model = Model.GPT4_Vision;
        chat.RequestParameters.MaxTokens = 128;

        chat.AppendSystemMessage(@"You are an assistant to help identify whether the provided image is a taxfree form issued by the VROs. VRO companies are: 'Global Blue', 'Planet', 'Tax refund'. Only TaxFree form issued by one of those company are valid ones. Please replay to in a concise way.");
        chat.AppendUserInput("Is it a valid image?", OpenAI_API.Chat.ChatMessage.ImageInput.FromImageUrl(imageUrl));

        return await chat.GetResponseFromChatbotAsync();
    }

    public async Task<string?> ChatWithGPTAsync(string userChat)
    {
        var apiClient = new OpenAIAPI(_openAIOptions.APIKey);

        // Create Conversation
        var result = await apiClient.Chat.CreateChatCompletionAsync(userChat);

        return result?.ToString();
    }
}