using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Domain.Entities;
using MongoDB.Bson;

namespace Genocs.TelegramIntegration.Domains;

/// <summary>
/// This is the domain Object you can import from an external platform.
/// </summary>
[TableMapping("VoucherJournal")]
public class VoucherJournal : Core.Domain.Entities.Entity<ObjectId>, IMongoDbEntity
{
    public string? Code { get; private set; }

    public string? RequestId { get; private set; }

    public string? Email { get; private set; }

    public decimal Cost { get; private set; }

    public decimal Value { get; private set; }

    public DateTime IssuedDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
}