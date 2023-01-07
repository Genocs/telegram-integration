using Genocs.TelegramIntegration.Contracts.Models;

namespace Genocs.TelegramIntegration.Services.Interfaces;

public interface ITelegramProxy
{
    Task PullUpdatesAsync();
    Task ProcessMessageAsync(TelegramMessage message);
    Task LogMessageAsync(string message);
}
