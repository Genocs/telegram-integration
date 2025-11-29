using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class RewardProcessedConsumer : IConsumer<RewardProcessed>
{
    private readonly ILogger<RewardProcessedConsumer> _logger;

    public RewardProcessedConsumer(
                                   ILogger<RewardProcessedConsumer> logger,
                                   ITelegramProxy telegramProxy,
                                   IMongoDbRepository<ChatUpdate> chatUpdateRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<RewardProcessed> context)
    {
        _logger.LogInformation("Received RewardProcessed");
        await Task.CompletedTask;
    }
}
