using Microsoft.AspNetCore.Mvc;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{

    [HttpGet()]
    public IActionResult Get()
    {
        return Ok("Genocs Telegram Integration Service");
    }
}