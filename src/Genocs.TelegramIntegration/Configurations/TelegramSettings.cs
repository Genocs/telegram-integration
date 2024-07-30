using System.ComponentModel.DataAnnotations;

namespace Genocs.TelegramIntegration.Configurations;

public class TelegramSettings
{
    public static string Position = "Telegram";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Telegram Token cannot be null or empty")]
    public string? Token { get; set; }
}
