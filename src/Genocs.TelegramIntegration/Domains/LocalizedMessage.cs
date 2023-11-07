using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Repositories;

namespace Genocs.TelegramIntegration.Domains;

[TableMapping("LocalizedMessages")]
public class LocalizedMessage : Core.Domain.Entities.AggregateRoot<Guid>, IMongoDbEntity
{
    public string LanguageId { get; set; } = default!;
    public string NotificationTag { get; set; } = default!;
    public string Message { get; set; } = default!;
}
