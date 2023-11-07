using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.Worker.Consumers;

public class LangChainResponseConsumer : IConsumer<LangChainResponse>
{
    private readonly ILogger<LangChainResponseConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;


    public LangChainResponseConsumer(
                                    ILogger<LangChainResponseConsumer> logger,
                                    ITelegramProxy telegramProxy)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
    }

    public async Task Consume(ConsumeContext<LangChainResponse> context)
    {
        _logger.LogInformation("Received LangChainResponse");
        await Task.CompletedTask;
    }
}
