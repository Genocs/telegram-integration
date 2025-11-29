using Genocs.Persistence.MongoDb.Domain.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using Genocs.Voucher.Contracts;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class VoucherResponseEventConsumer : IConsumer<VoucherResponseEvent>
{
    private readonly ILogger<VoucherResponseEventConsumer> _logger;
    private readonly ITelegramProxy _telegramProxy;
    private readonly IMongoDbRepository<VoucherJournal> _voucherJournalRepository;
    private readonly IMongoDbRepository<UserChat> _userChatRepository;

    public VoucherResponseEventConsumer(
                                    ILogger<VoucherResponseEventConsumer> logger,
                                    IMongoDbRepository<VoucherJournal> voucherJournalRepository,
                                    IMongoDbRepository<UserChat> userChatRepository,
                                    ITelegramProxy telegramProxy)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
        _voucherJournalRepository = voucherJournalRepository ?? throw new ArgumentNullException(nameof(voucherJournalRepository));
        _userChatRepository = userChatRepository ?? throw new ArgumentNullException(nameof(userChatRepository));
    }

    public async Task Consume(ConsumeContext<VoucherResponseEvent> context)
    {
        _logger.LogInformation($"Received VoucherResponseEvent for voucher with code: '{context.Message.VoucherCode}'");

        // Get the voucher journal from the database
        VoucherJournal voucherJournal = await _voucherJournalRepository.GetAsync(x => x.Code == context.Message.VoucherCode);

        if (voucherJournal is null)
        {
            _logger.LogError($"VoucherJournal not found for voucher with code: '{context.Message.VoucherCode}'");
            return;
        }

        if (string.IsNullOrWhiteSpace(voucherJournal.RequestId))
        {
            _logger.LogError("RequestId is null or empty.");
            return;
        }

        // Check if there is someone registered to receive the voucher notification
        var usersChat = await _userChatRepository.GetAllListAsync(x => voucherJournal.RequestId.StartsWith(x.ExternalId));

        if (usersChat is null || !usersChat.Any())
        {
            _logger.LogError($"No user chat registered for voucher with code: '{context.Message.VoucherCode}'. RequestId: '{voucherJournal.RequestId}'");
            return;
        }

        // Send notification to the user by telegram
        foreach (var userChat in usersChat)
        {
            string imageUrl = $"<your_api_url>/api/vouchers/?data={voucherJournal?.Code}";
            string caption = $"Voucher Code: {voucherJournal?.Code} -- Cost: €{voucherJournal?.Cost} -- Value: €{voucherJournal?.Value} -- email: {voucherJournal?.Email} -- date: {voucherJournal?.IssuedDate}";
            await _telegramProxy.SendMessageWithImageAsync(userChat.ChatId, imageUrl, caption);
            Task.Delay(500).Wait();
        }
    }
}
