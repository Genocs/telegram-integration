using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Domain.Entities;
using MongoDB.Bson;

namespace Genocs.TelegramIntegration.Domains;

[TableMapping("Users")]
public class UserChat : Core.Domain.Entities.AggregateRoot<ObjectId>, IMongoDbEntity
{
    /// <summary>
    /// The chat id.
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// The external id. This is the id that the external system uses to identify the user.
    /// </summary>
    public string ExternalId { get; set; } = default!;

    /// <summary>
    /// Default language for the user.
    /// </summary>
    public string Language { get; set; } = "en-US";
}
