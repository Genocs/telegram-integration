using System.ComponentModel.DataAnnotations;

namespace Genocs.TelegramIntegration.Options;

public class StripeSettings
{
    public static string Position = "Stripe";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Stripe Token cannot be null or empty")]
    public string? Token { get; set; }
}
