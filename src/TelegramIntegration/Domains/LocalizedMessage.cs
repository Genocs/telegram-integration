using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Domain.Entities;
using MongoDB.Bson;

namespace Genocs.TelegramIntegration.Domains;

[TableMapping("LocalizedMessages")]
public class LocalizedMessage : Core.Domain.Entities.AggregateRoot<ObjectId>, IMongoDbEntity
{
    public string LanguageId { get; set; } = default!;
    public string NotificationTag { get; set; } = default!;
    public string Message { get; set; } = default!;
}
