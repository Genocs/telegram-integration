using Genocs.Fiscanner.Contracts.Promotions;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class VoucherIssuingCompletedConsumer : IConsumer<VoucherIssuingCompleted>
{
    private readonly ILogger<VoucherIssuingCompletedConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;

    public VoucherIssuingCompletedConsumer(
                                            ILogger<VoucherIssuingCompletedConsumer> logger,
                                            ITelegramProxy telegramProxy)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
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
