using System.ComponentModel.DataAnnotations;

namespace Genocs.TelegramIntegration.Configurations;

public class ApiClientSettings
{
    public static string Position = "ApiClient";

    [Required(AllowEmptyStrings = false, ErrorMessage = "ApiClient FormRecognizerUrl cannot be null or empty")]
    public string? FormRecognizerUrl { get; set; }
}
