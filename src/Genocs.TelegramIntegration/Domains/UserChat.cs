using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Repositories;

namespace Genocs.TelegramIntegration.Domains;

[TableMapping("Users")]
public class UserChat : Core.Domain.Entities.AggregateRoot<MongoDB.Bson.ObjectId>, IMongoDbEntity
{
    public int ChatId { get; set; }
    public string MemberId { get; set; } = default!;
}
