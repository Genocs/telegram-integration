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
    /// <returns>The async task.</returns>
    Task PullUpdatesAsync();

    /// <summary>
    /// Process a message from Telegram.
    /// This function is called by the webhook.
    /// </summary>
    /// <param name="message">The message received by the webhook.</param>
    /// <returns>The async task.</returns>
    Task ProcessMessageAsync(Update? message);

    /// <summary>
    /// Proxy to send a message to a recipient.
    /// </summary>
    /// <param name="recipient">The Recipient as chatId.</param>
    /// <param name="message">The message payload.</param>
    /// <returns>The async response with the message sent to Telegram platform.</returns>
    Task<Telegram.BotAPI.AvailableTypes.Message?> SendMessageAsync(long recipient, string? message);

    /// <summary>
    /// Proxy to send a message to a recipient.
    /// </summary>
    /// <param name="recipient">The Recipient as chatId.</param>
    /// <param name="imageUrl">The image url.</param>
    /// <param name="caption">The image caption.</param>
    /// <returns>The async response with the message sent to Telegram platform.</returns>
    Task<Telegram.BotAPI.AvailableTypes.Message?> SendMessageWithImageAsync(long recipient, string? imageUrl, string? caption);

    /// <summary>
    /// Send a checkout request to the user.
    /// </summary>
    /// <param name="orderId">The orderId.</param>
    /// <param name="recipient">The recipient as chatId.</param>
    /// <param name="amount">The amount.</param>
    /// <param name="currency">The currency.</param>
    /// <returns>The async response with the message sent to Telegram platform.</returns>
    Task CheckoutAsync(string orderId, long recipient, decimal amount, string currency = "EUR");
}
