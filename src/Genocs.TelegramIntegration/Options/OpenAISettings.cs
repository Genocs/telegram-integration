using System.ComponentModel.DataAnnotations;

namespace Genocs.TelegramIntegration.Options;

public class OpenAISettings
{
    public static string Position = "OpenAI";

    [Required(AllowEmptyStrings = false, ErrorMessage = "OpenAI APIKey cannot be null or empty")]
    public string APIKey { get; set; } = default!;

    [Required(AllowEmptyStrings = false, ErrorMessage = "OpenAI Url cannot be null or empty")]
    public string Url { get; set; } = default!;
}
