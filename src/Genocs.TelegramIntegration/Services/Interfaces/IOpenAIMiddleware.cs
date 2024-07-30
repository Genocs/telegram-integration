namespace Genocs.TelegramIntegration.Services.Interfaces;

/// <summary>
/// OpenAI middleware interface definition.
/// </summary>
public interface IOpenAIMiddleware
{
    Task<string?> ValidateTaxFreeFormAsync(string imageUrl);

    Task<string?> ChatWithGPTAsync(string userChat);
}
