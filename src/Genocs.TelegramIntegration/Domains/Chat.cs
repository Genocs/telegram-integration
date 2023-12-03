using Genocs.Persistence.MongoDb.Repositories.Clean;
using MongoDB.Bson;

namespace Genocs.TelegramIntegration.Domains;

public class GenocsChat : Core.Domain.Entities.AggregateRoot<ObjectId>, IMongoDbEntity
{
    public int UpdateId { get; set; }
    public string? Message { get; set; }
}
