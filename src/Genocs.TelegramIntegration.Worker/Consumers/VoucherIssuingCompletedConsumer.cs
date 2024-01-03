using Genocs.Fiscanner.Contracts.Promotions;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;
using Telegram.BotAPI;

namespace Genocs.TelegramIntegration.Worker.Consumers;

public class VoucherIssuingCompletedConsumer : IConsumer<VoucherIssuingCompleted>
{
    private readonly ILogger<VoucherIssuingCompletedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<UserChat> _usersChatRepository;
    private readonly IMongoDbRepository<LocalizedMessage> _localizedMessagesRepository;

    public VoucherIssuingCompletedConsumer(
                                            ILogger<VoucherIssuingCompletedConsumer> logger,
                                            ITelegramProxy telegramProxy,
                                            IMongoDbRepository<UserChat> usersChatRepository,
                                            IMongoDbRepository<LocalizedMessage> localizedMessagesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _usersChatRepository = usersChatRepository ?? throw new ArgumentNullException(nameof(usersChatRepository));
        _localizedMessagesRepository = localizedMessagesRepository ?? throw new ArgumentNullException(nameof(localizedMessagesRepository));
    }

    public async Task Consume(ConsumeContext<VoucherIssuingCompleted> context)
    {
        if (long.TryParse(context.Message.ReferenceId, out long recipient))
        {
            await _telegramProxy.SendMessageAsync(recipient, "VoucherIssuingCompleted");
        }
        else
        {
            _logger.LogError("VoucherIssuingCompleted got an error. Received {recipient},", context.Message.ReferenceId);
        }
    }
}
