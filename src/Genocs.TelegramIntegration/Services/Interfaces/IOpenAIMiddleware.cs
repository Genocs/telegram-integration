namespace Genocs.TelegramIntegration.Services.Interfaces;

/// <summary>
/// OpenAI middleware interface definition.
/// </summary>
public interface IOpenAIMiddleware
{
    Task<string?> ValidateDocumentAsync(string documentUrl);

    Task<string?> ChatWithGPTAsync(string userChat);
}
