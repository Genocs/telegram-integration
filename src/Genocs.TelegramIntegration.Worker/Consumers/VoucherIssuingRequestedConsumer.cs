using Genocs.Fiscanner.Contracts.Promotions;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.Worker.Consumers;

public class VoucherIssuingRequestedConsumer : IConsumer<VoucherIssuingRequested>
{
    private readonly ILogger<VoucherIssuingRequestedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<UserChat> _usersChatRepository;
    private readonly IMongoDbRepository<LocalizedMessage> _localizedMessagesRepository;

    public VoucherIssuingRequestedConsumer(
                                            ILogger<VoucherIssuingRequestedConsumer> logger,
                                            ITelegramProxy telegramProxy,
                                            IMongoDbRepository<UserChat> usersChatRepository,
                                            IMongoDbRepository<LocalizedMessage> localizedMessagesRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _usersChatRepository = usersChatRepository ?? throw new ArgumentNullException(nameof(usersChatRepository));
        _localizedMessagesRepository = localizedMessagesRepository ?? throw new ArgumentNullException(nameof(localizedMessagesRepository));
    }

    public async Task Consume(ConsumeContext<VoucherIssuingRequested> context)
    {
        _logger.LogDebug("Received VoucherIssuingRequested");

        if (string.IsNullOrEmpty(context.Message.ReferenceId))
        {
            _logger.LogWarning($"Received VoucherIssuingRequested. ReferenceId is null or empty for memberId: '{context.Message.MemberId}'");
            return;
        }

        if (long.TryParse(context.Message.ReferenceId, out long referenceId))
        {

            UserChat chat = await _usersChatRepository.FirstOrDefaultAsync(x => x.ChatId == referenceId);

            if (chat is null)
            {
                _logger.LogWarning($"Received VoucherIssuingRequested. User chat is null for memberId: '{context.Message.MemberId}'");
                return;
            }

            // Notify
            await _telegramProxy.SendMessageAsync(chat.ChatId, "VoucherIssuingRequested");

            // Check the amount of vouchers issued for the user
            if (context.Message.Cost > 0)
            {
                // Send notification to complete checkout about the cost of the voucher
                await _telegramProxy.CheckoutAsync(chat.ChatId, context.Message.Cost, "EUR");
            }
        }
    }
}
