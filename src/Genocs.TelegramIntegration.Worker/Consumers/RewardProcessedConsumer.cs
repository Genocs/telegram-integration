using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.Worker.Consumers;

public class RewardProcessedConsumer : IConsumer<RewardProcessed>
{
    private readonly ILogger<RewardProcessedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<UserChat> _usersChatRepository;
    private readonly IMongoDbRepository<LocalizedMessage> _localizedMessagesRepository;


    public RewardProcessedConsumer(ILogger<RewardProcessedConsumer> logger,
                                    ITelegramProxy telegramProxy,
                                    IMongoDbRepository<UserChat> usersChatRepository,
                                    IMongoDbRepository<LocalizedMessage> localizedMessagesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _usersChatRepository = usersChatRepository ?? throw new ArgumentNullException(nameof(usersChatRepository));
        _localizedMessagesRepository = localizedMessagesRepository ?? throw new ArgumentNullException(nameof(localizedMessagesRepository));
    }

    public async Task Consume(ConsumeContext<RewardProcessed> context)
    {
        _logger.LogInformation("Received RewardProcessed");

        var user = await _usersChatRepository.FirstOrDefaultAsync(c => c.MemberId == context.Message.MemberId);

        if (user is null) return;

        var localizedMessage = await _localizedMessagesRepository.FirstOrDefaultAsync(c => c.LanguageId == context.Message.Language
                                                                                        && c.NotificationTag == context.Message.NotificationTag);

        if (localizedMessage is null) return;

        switch (context.Message.NotificationTag)
        {
            case "voucher_issued":
                await _telegramProxy.SendMessageAsync(user.ChatId,
                                                        string.Format(localizedMessage.Message,
                                                                        context.Message.Metadata["amount"],
                                                                        context.Message.Metadata["reward_amount"]));
                break;
        }
    }
}
