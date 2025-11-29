namespace Genocs.TelegramIntegration.WebApi.Controllers;

/// <summary>
/// Command to link an external identifier to a chat.
/// </summary>
public class LinkExternalIdCommand
{
    /// <summary>
    /// The external identifier to link to the chat.
    /// </summary>
    public string ExternalId { get; set; } = default!;

    /// <summary>
    /// The chat identifier.
    /// </summary>
    public long ChatId { get; set; }
}