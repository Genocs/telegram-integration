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
    private readonly IMongoDbRepository<UserChat> _usersChatRepository;
    private readonly IMongoDbRepository<LocalizedMessage> _localizedMessagesRepository;

    public RewardNotifiedConsumer(
                                  ILogger<RewardNotifiedConsumer> logger,
                                  ITelegramProxy telegramProxy,
                                  IMongoDbRepository<UserChat> usersChatRepository,
                                  IMongoDbRepository<LocalizedMessage> localizedMessagesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _usersChatRepository = usersChatRepository ?? throw new ArgumentNullException(nameof(usersChatRepository));
        _localizedMessagesRepository = localizedMessagesRepository ?? throw new ArgumentNullException(nameof(localizedMessagesRepository));
    }

    public async Task Consume(ConsumeContext<RewardNotified> context)
    {
        _logger.LogDebug("Received RewardNotified");

        var user = await _usersChatRepository.FirstOrDefaultAsync(c => c.MemberId == context.Message.MemberId);

        if (user is null)
        {
            _logger.LogWarning($"Received RewardNotified. User chat is null for memberId: '{context.Message.MemberId}'");
            return;
        }

        var localizedMessage = await _localizedMessagesRepository.FirstOrDefaultAsync(c => c.LanguageId == user.Language
                                                                                        && c.NotificationTag == context.Message.NotificationTag);

        // Use ChatGPT3 to generate the message
        if (localizedMessage is null) return;

        switch (context.Message.NotificationTag)
        {
            case "voucher_issued":
                await _telegramProxy.SendMessageAsync(user.ChatId,
                                                        string.Format(localizedMessage.Message,
                                                                        context.Message.Metadata["amount"],
                                                                        context.Message.Metadata["reward_amount"]));
                break;

            case "reward_issued":
                await _telegramProxy.SendMessageAsync(user.ChatId,
                                                        string.Format(localizedMessage.Message,
                                                                        context.Message.Metadata["amount"],
                                                                        context.Message.Metadata["reward_amount"]));
                break;
        }
    }
}
