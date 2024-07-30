using System.ComponentModel.DataAnnotations;

namespace Genocs.TelegramIntegration.Configurations;

public class OpenAISettings
{
    public static string Position = "OpenAI";

    [Required(AllowEmptyStrings = false, ErrorMessage = "OpenAI APIKey cannot be null or empty")]
    public string APIKey { get; set; } = default!;
}
