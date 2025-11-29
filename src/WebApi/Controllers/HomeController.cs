using Microsoft.AspNetCore.Mvc;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
        => Ok("Genocs Telegram Integration Services");

    [HttpGet("ping")]
    public IActionResult GetPing()
        => Ok("pong");
}