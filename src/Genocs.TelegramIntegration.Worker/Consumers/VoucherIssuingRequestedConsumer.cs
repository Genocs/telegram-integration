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

        await Task.CompletedTask;
    }
}
