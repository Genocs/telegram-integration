using System.Net.Mime;
using Genocs.Persistence.MongoDb.Domain.Repositories;
using Genocs.TelegramIntegration.Domains;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.TelegramIntegration.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IMongoDbRepository<UserChat> _mongoDbRepository;
    public ConfigurationController(IMongoDbRepository<UserChat> mongoDbRepository)
    {
        _mongoDbRepository = mongoDbRepository ?? throw new ArgumentNullException(nameof(mongoDbRepository));
    }

    [HttpPost("LinkChatToExternalId")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostLinkExternalId(LinkExternalIdCommand command)
    {
        var res = await _mongoDbRepository.InsertOrUpdateAsync(new UserChat { ChatId = command.ChatId, ExternalId = command.ExternalId });
        return Accepted(res.Id);
    }
}