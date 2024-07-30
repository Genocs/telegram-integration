using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMongoDbRepository<UserChat> _mongoDbRepository;
    private readonly ITelegramProxy _telegramProxy;

    private readonly static string BarcodeGeneratorTemplate = @"https://barcode.tec-it.com/barcode.ashx?data={0}&code=Code128";

    public NotificationController(IMongoDbRepository<UserChat> mongoDbRepository, ITelegramProxy telegramProxy)
    {
        _mongoDbRepository = mongoDbRepository ?? throw new ArgumentNullException(nameof(mongoDbRepository));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
    }

    [HttpPost("Notify")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostLinkExternalId(NotifyExternalIdCommand command)
    {
        var user = await _mongoDbRepository.GetAsync(c => c.ExternalId == command.ExternalId);
        if (user is null)
        {
            return NotFound();
        }

        // Apply string interpolation to the BarcodeGenerator string
        await _telegramProxy.SendMessageWithImageAsync(
                                                        user.ChatId,
                                                        string.Format(BarcodeGeneratorTemplate, command.ImageUrl),
                                                        command.Caption);

        return Accepted();
    }
}