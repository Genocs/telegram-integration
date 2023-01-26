using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using Telegram.BotAPI.GettingUpdates;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WebHookController : ControllerBase
{
    private readonly ITelegramProxy _telegramProxy;
    public WebHookController(ITelegramProxy telegramProxy)
    {
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
    }

    [HttpPost("")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostUpdates(object message)
    {
        // This is the only way I fund to overcome the issue on deserialize
        // PaymentCheckout payload
        string ser = JsonSerializer.Serialize(message);
        Update? update = JsonSerializer.Deserialize<Update>(ser);

        await _telegramProxy.ProcessMessageAsync(update);
        return Accepted();
    }
}