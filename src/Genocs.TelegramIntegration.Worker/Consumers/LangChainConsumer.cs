using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.Worker.Consumers;

public class LangChainUpdateConsumer : IConsumer<LangChainUpdate>
{
    private readonly ILogger<LangChainUpdateConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;


    public LangChainUpdateConsumer(
                                    ILogger<LangChainUpdateConsumer> logger,
                                    ITelegramProxy telegramProxy)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
    }

    public async Task Consume(ConsumeContext<LangChainUpdate> context)
    {
        _logger.LogInformation("Received LangChainUpdate");
        await Task.CompletedTask;
    }
}
