using Telegram.BotAPI.GettingUpdates;

namespace Genocs.TelegramIntegration.Services.Interfaces;

/// <summary>
/// Telegram proxy interface definition.
/// </summary>
public interface ITelegramProxy
{
    /// <summary>
    /// Pull updates from Telegram.
    /// </summary>
    /// <returns></returns>
    Task PullUpdatesAsync();

    /// <summary>
    /// Process a message from Telegram.
    /// This function is called by the webhook.
    /// </summary>
    /// <param name="message">The message received by the webhook.</param>
    /// <returns>async task.</returns>
    Task ProcessMessageAsync(Update? message);

    /// <summary>
    /// Proxy to send a message to a recipient.
    /// </summary>
    /// <param name="recipient">The Recipient as chatId.</param>
    /// <param name="message">Payload with the message.</param>
    /// <returns>The async response with the message sent to Telegram platform.</returns>
    Task<Telegram.BotAPI.AvailableTypes.Message?> SendMessageAsync(long recipient, string? message);
}
