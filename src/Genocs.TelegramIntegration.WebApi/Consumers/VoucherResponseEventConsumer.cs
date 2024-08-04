using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;
using UTU.Voucher.Contracts;

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
        // Get the voucher journal from the database - utu platform
        VoucherJournal voucherJournal = await _voucherJournalRepository.GetAsync(x => x.Code == context.Message.VoucherCode);

        if (voucherJournal is null)
        {
            _logger.LogError("VoucherJournal not found");
            return;
        }

        if (voucherJournal.CurrencyAlliance is null)
        {
            _logger.LogError("CurrencyAlliance not found");
            return;
        }

        // Check if there is someone registerd to receive the voucher notification
        var usersChat = await _userChatRepository.GetAllListAsync(x => x.ExternalId == voucherJournal.ExternalRequestId);

        if (usersChat is null || !usersChat.Any())
        {
            _logger.LogError("No user chat registerd");
            return;
        }

        // Send notification to the user by telegram
        foreach (var userChat in usersChat)
        {
            string photoUrl = $"https://barcode.tec-it.com/barcode.ashx?data={voucherJournal?.CurrencyAlliance.BarcodeString}&code=Code128";
            string caption = $"utu Voucher id: {voucherJournal?.CurrencyAlliance?.BarcodeString}-- Cost: €{voucherJournal?.VATRefundAmount}-- Value: €{voucherJournal?.Amount}-- email: {voucherJournal?.Email}-- date: {voucherJournal?.IssuedDate}";
            await _telegramProxy.SendMessageWithImageAsync(userChat.ChatId, photoUrl, caption);
        }
    }
}
