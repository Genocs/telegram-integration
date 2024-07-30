namespace Genocs.TelegramIntegration.WebApi.Controllers;

/// <summary>
/// Command to link an external identifier to a chat.
/// </summary>
public class NotifyExternalIdCommand
{
    /// <summary>
    /// The external identifier to link to the chat.
    /// </summary>
    public string ExternalId { get; set; } = default!;

    /// <summary>
    /// The image url.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The chat identifier.
    /// </summary>
    public string? Caption { get; set; }
}