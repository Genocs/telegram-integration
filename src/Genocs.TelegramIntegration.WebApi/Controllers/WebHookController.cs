using Genocs.TelegramIntegration.Contracts.Models;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
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

    [HttpGet()]
    public IActionResult Get()
    {
        return Ok("xx");
    }

    [HttpPost("test")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostUpdates(object message)
    {
        string ser = JsonSerializer.Serialize(message);

        Update? update = JsonSerializer.Deserialize<Update>(ser);
        await _telegramProxy.ProcessMessageAsync(update);
        return Accepted();
    }
}