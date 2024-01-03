using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Repositories.Clean;
using MongoDB.Bson;

namespace Genocs.TelegramIntegration.Domains;

[TableMapping("Users")]
public class UserChat : Core.Domain.Entities.AggregateRoot<ObjectId>, IMongoDbEntity
{
    public long ChatId { get; set; }
    public string MemberId { get; set; } = default!;
    public string? Language { get; set; }
}
