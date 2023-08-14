using Telegram.BotAPI.GettingUpdates;

namespace Genocs.TelegramIntegration.Services.Interfaces;


/// <summary>
/// Telegram proxy interface definition
/// </summary>
public interface ITelegramProxy
{
    /// <summary>
    /// Pull updates from Telegram
    /// </summary>
    /// <returns></returns>
    Task PullUpdatesAsync();

    /// <summary>
    /// Process a message from Telegram
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>async task</returns>
    Task ProcessMessageAsync(Update? message);
    Task LogMessageAsync(string? message);
    Task SendMessageAsync(long recipient, string? message);
}
