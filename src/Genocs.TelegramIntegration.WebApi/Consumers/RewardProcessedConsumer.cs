using Genocs.Fiscanner.Contracts.Notifications;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class RewardProcessedConsumer : IConsumer<RewardProcessed>
{
    private readonly ILogger<RewardProcessedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<ChatUpdate> _chatUpdateRepository;

    public RewardProcessedConsumer(
                                   ILogger<RewardProcessedConsumer> logger,
                                   ITelegramProxy telegramProxy,
                                   IMongoDbRepository<ChatUpdate> chatUpdateRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _chatUpdateRepository = chatUpdateRepository ?? throw new ArgumentNullException(nameof(chatUpdateRepository));
    }

    public async Task Consume(ConsumeContext<RewardProcessed> context)
    {
        _logger.LogInformation("Received RewardProcessed");
        await Task.CompletedTask;

        // if (string.IsNullOrEmpty(context.Message.ReferenceId))
        // {
        //     _logger.LogInformation($"Received RewardProcessed. ReferenceId is null or empty");
        //     return;
        // }
           
        // if (int.TryParse(context.Message.ReferenceId, out int updateId))
        // {
           
        //     var update = await _chatUpdateRepository.FirstOrDefaultAsync(x => x.Message.UpdateId == updateId);
           
        //     if (update is null)
        //     {
        //         _logger.LogWarning($"Received VoucherIssuingRequested. ChatUpdate  is null for the updateId: '{context.Message.ReferenceId}'");
        //         return;
        //     }
           
        //     // Notify
        //     await _telegramProxy.SendMessageAsync(update.Message.Message.Chat.Id, "Good News! You won a amazing Voucher. Please complete the Payment to use it!");
        // }
    }
}
