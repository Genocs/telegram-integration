using OpenAI_API;
using OpenAI_API.Models;

namespace Genocs.TelegramIntegration.Models;

public static class OpenAIBuilder
{
    public static async Task<string?> BuildIsValidTaxFree(string imageUrl, string apiKey)
    {
        OpenAIAPI apiClient = new OpenAIAPI(apiKey);

        // Create Conversation
        var chat = apiClient.Chat.CreateConversation();
        chat.Model = Model.GPT4_Vision;
        chat.RequestParameters.MaxTokens = 128;

        chat.AppendSystemMessage(@"You are an assistant to help identify whether the provided image is a taxfree form issued by the VROs. VRO companies are: 'Global Blue', 'Planet', 'Tax refund'. Only TaxFree form issued by one of those company are valid ones. Please replay to in a concise way.");
        chat.AppendUserInput("Is it a valid image?", OpenAI_API.Chat.ChatMessage.ImageInput.FromImageUrl(imageUrl));
        return await chat.GetResponseFromChatbotAsync();
    }
}