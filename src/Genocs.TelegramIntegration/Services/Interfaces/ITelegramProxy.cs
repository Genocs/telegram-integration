using Telegram.BotAPI.GettingUpdates;

namespace Genocs.TelegramIntegration.Services.Interfaces;

public interface ITelegramProxy
{
    Task PullUpdatesAsync();
    Task ProcessMessageAsync(Update? message);
    Task LogMessageAsync(string message);
    Task SendMessageAsync(int recipient, string message);
}
