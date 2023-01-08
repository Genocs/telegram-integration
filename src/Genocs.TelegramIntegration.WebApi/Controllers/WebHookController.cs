using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Telegram.BotAPI.GettingUpdates;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WebHookController : ControllerBase
{
    private readonly ILogger<WebHookController> _logger;
    private readonly ITelegramProxy _telegramProxy;

    public WebHookController(ILogger<WebHookController> logger, ITelegramProxy telegramProxy)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramProxy = telegramProxy ?? throw new ArgumentNullException(nameof(telegramProxy));
    }

    [HttpPost()]
    public async Task<IActionResult> PostUpdates(Update message)
    {
        _logger.LogCritical(JsonSerializer.Serialize(message));
        await _telegramProxy.ProcessMessageAsync(message);
        await _telegramProxy.LogMessageAsync(JsonSerializer.Serialize(message));
        return Ok(message);
    }
}