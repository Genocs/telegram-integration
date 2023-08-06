using Genocs.Persistence.MongoDb.Repositories;

namespace Genocs.TelegramIntegration.Domains;

public class GenocsChat : Core.Domain.Entities.AggregateRoot<Guid>, IMongoDbEntity
{
    public int UpdateId { get; set; }
    public string? Message { get; set; }
}
