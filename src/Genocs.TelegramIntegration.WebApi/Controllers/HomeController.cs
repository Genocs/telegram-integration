using Microsoft.AspNetCore.Mvc;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
        => Ok("Genocs - TelegramIntegration WebApi");

    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Ping()
        => Ok("Pong");
}