using Genocs.Fiscanner.Contracts.Promotions;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.Worker.Consumers;

public class VoucherIssuedConsumer : IConsumer<VoucherIssued>
{
    private readonly ILogger<VoucherIssuedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<ChatUpdate> _chatUpdateRepository;

    public VoucherIssuedConsumer(
                                            ILogger<VoucherIssuedConsumer> logger,
                                            ITelegramProxy telegramProxy,
                                            IMongoDbRepository<ChatUpdate> chatUpdateRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _chatUpdateRepository = chatUpdateRepository ?? throw new ArgumentNullException(nameof(chatUpdateRepository));
    }

    public async Task Consume(ConsumeContext<VoucherIssued> context)
    {
        _logger.LogDebug("Received VoucherIssued");

        if (string.IsNullOrEmpty(context.Message.ReferenceId))
        {
            _logger.LogWarning($"Received VoucherIssued. ReferenceId is null or empty");
            return;
        }

        if (int.TryParse(context.Message.ReferenceId, out int updateId))
        {

            ChatUpdate update = await _chatUpdateRepository.FirstOrDefaultAsync(x => x.Message.UpdateId == updateId);

            if (update is null)
            {
                _logger.LogWarning($"Received VoucherIssuingRequested. ChatUpdate  is null for the updateId: '{context.Message.ReferenceId}'");
                return;
            }

            // Notify
            await _telegramProxy.SendMessageAsync(update.Message.Message.Chat.Id, "VoucherIssuingRequested");

            // Check the amount of vouchers issued for the user
            if (context.Message.Cost > 0)
            {
                // Send notification to complete checkout about the cost of the voucher
                await _telegramProxy.CheckoutAsync(update.Message.Message.Chat.Id, context.Message.Cost, "EUR");
            }
        }
    }
}
