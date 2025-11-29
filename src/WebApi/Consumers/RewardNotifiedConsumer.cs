using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class RewardNotifiedConsumer : IConsumer<RewardNotified>
{
    private readonly ILogger<RewardNotifiedConsumer> _logger;
    private readonly IMongoDbRepository<ChatUpdate> _chatUpdateRepository;

    public RewardNotifiedConsumer(
                                  ILogger<RewardNotifiedConsumer> logger,
                                  ITelegramProxy telegramProxy,
                                  IMongoDbRepository<ChatUpdate> chatUpdateRepository,
                                  IMongoDbRepository<LocalizedMessage> localizedMessagesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _chatUpdateRepository = chatUpdateRepository ?? throw new ArgumentNullException(nameof(chatUpdateRepository));
    }

    public async Task Consume(ConsumeContext<RewardNotified> context)
    {
        _logger.LogDebug("Received RewardNotified");

        if (string.IsNullOrEmpty(context.Message.ReferenceId))
        {
            _logger.LogWarning($"Received VoucherIssued. ReferenceId is null or empty");
            return;
        }

        if (int.TryParse(context.Message.ReferenceId, out int updateId))
        {
            var update = await _chatUpdateRepository.FirstOrDefaultAsync(x => x.Message.UpdateId == updateId);

            if (update is null)
            {
                _logger.LogWarning($"Received RewardNotified. ChatUpdate is null for the updateId: '{context.Message.ReferenceId}'");
                return;
            }
        }
    }
}
