using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class RewardNotifiedConsumer : IConsumer<RewardNotified>
{
    private readonly ILogger<RewardNotifiedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<ChatUpdate> _chatUpdateRepository;
    private readonly IMongoDbRepository<LocalizedMessage> _localizedMessagesRepository;

    public RewardNotifiedConsumer(
                                  ILogger<RewardNotifiedConsumer> logger,
                                  ITelegramProxy telegramProxy,
                                  IMongoDbRepository<ChatUpdate> chatUpdateRepository,
                                  IMongoDbRepository<LocalizedMessage> localizedMessagesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _chatUpdateRepository = chatUpdateRepository ?? throw new ArgumentNullException(nameof(chatUpdateRepository));
        _localizedMessagesRepository = localizedMessagesRepository ?? throw new ArgumentNullException(nameof(localizedMessagesRepository));
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
                _logger.LogWarning($"Received VoucherIssuingRequested. ChatUpdate  is null for the updateId: '{context.Message.ReferenceId}'");
                return;
            }

            //switch (context.Message.NotificationTag)
            //{
            //    case "voucher_issued":
            //        await _telegramProxy.SendMessageAsync(update.Message.Message.Chat.Id,
            //                                                string.Format(localizedMessage.Message,
            //                                                                context.Message.Metadata["amount"],
            //                                                                context.Message.Metadata["reward_amount"]));
            //        break;

            //    case "reward_issued":
            //        await _telegramProxy.SendMessageAsync(update.Message.Message.Chat.Id,
            //                                                string.Format(localizedMessage.Message,
            //                                                                context.Message.Metadata["amount"],
            //                                                                context.Message.Metadata["reward_amount"]));
            //        break;
            //}
        }
    }
}
