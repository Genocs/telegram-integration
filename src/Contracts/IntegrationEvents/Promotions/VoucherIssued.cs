// This namespace is needed to send/receive message to the promotion engine appropriately.
// DO NOT CHANGE IT
namespace Genocs.Fiscanner.Contracts.Promotions;

public record VoucherIssued
{
    /// <summary>
    /// The reference id. Client can use it to keep external reference.
    /// </summary>
    public string? ReferenceId { get; init; }

    /// <summary>
    /// The Unique identifier of the request.
    /// </summary>
    public string? RequestId { get; init; }

    /// <summary>
    /// The context id.
    /// </summary>
    public string? ContextId { get; init; }

    /// <summary>
    /// The campaign code that determined the voucher issuing.
    /// </summary>
    public string? CampaignCode { get; init; }

    /// <summary>
    /// The issuer of the voucher.
    /// </summary>
    public string? Issuer { get; init; }

    /// <summary>
    /// The Voucher's owner.
    /// </summary>
    public string? OwnerId { get; init; }

    /// <summary>
    /// The voucher Id.
    /// </summary>
    public string VoucherId { get; init; } = default!;

    /// <summary>
    /// The nominal value of the voucher.
    /// </summary>
    public decimal Value { get; init; }

    /// <summary>
    /// The cost of the voucher.
    /// </summary>
    public decimal Cost { get; init; }

    /// <summary>
    /// The currency of the voucher.
    /// </summary>
    public string Currency { get; init; } = "EUR";
    public DateTime ExpirationDate { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }

}