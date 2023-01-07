namespace Genocs.TelegramIntegration.Options;

public class OpenAISettings
{
    public static string Position = "OpenAISettings";
    public string APIKey { get; set; } = default!;

    public string Url { get; set; } = default!;
}
