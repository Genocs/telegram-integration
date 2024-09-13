using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Repositories.Clean;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Genocs.TelegramIntegration.Domains;

/// <summary>
/// This is the domain Object imported by the external platform.
/// </summary>
[TableMapping("VoucherJournalV2")]
public class VoucherJournal : Core.Domain.Entities.Entity<ObjectId>, IMongoDbEntity
{
    [BsonElement("Code")]
    public string? Code { get; private set; }

    [BsonElement("MemberId")]
    public string? MemberId { get; private set; }

    [BsonElement("Nickname")]
    public string? Nickname { get; private set; }

    [BsonElement("Email")]
    public string? Email { get; private set; }

    [BsonElement("IssuedDate")]
    public DateTime? IssuedDate { get; private set; }

    [BsonElement("ExpiredDate")]
    public DateTime? ExpiredDate { get; private set; }

    /// <summary>
    /// The Voucher Value.
    /// </summary>
    [BsonElement("Amount")]
    public decimal? Amount { get; private set; }

    /// <summary>
    /// The Voucher Cost.
    /// </summary>
    [BsonElement("VATRefundAmount")]
    public decimal? VATRefundAmount { get; private set; }

    [BsonElement("Type")]
    public string? Type { get; private set; }

    [BsonElement("CurrencyAlliance")]
    public CurrencyAlliance? CurrencyAlliance { get; private set; }

    /// <summary>
    /// Where the voucher is coming from. For Dufry is the Evangelist Code..
    /// </summary>
    [BsonElement("ExternalRequestId")]
    public string? ExternalRequestId { get; private set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("UpdatedAt")]
    public DateTime UpdatedAt { get; private set; }
}

public class CurrencyAlliance(string barcodeFormat, string barcodeString, DateTime expirationDate, string giftCardCode, string caId)
{
    [BsonElement("BarcodeFormat")]
    public string? BarcodeFormat { get; private set; } = barcodeFormat;

    [BsonElement("BarcodeString")]
    public string? BarcodeString { get; private set; } = barcodeString;

    [BsonElement("ExpirationDate")]
    public DateTime ExpirationDate { get; private set; } = expirationDate;

    [BsonElement("GiftCardCode")]
    public string? GiftCardCode { get; private set; } = giftCardCode;

    [BsonElement("CaId")]
    public string? CaId { get; private set; } = caId;
}