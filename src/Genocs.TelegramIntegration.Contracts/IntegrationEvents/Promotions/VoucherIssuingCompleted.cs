// This namespace is needed to send/receive message to the promotion engine appropriately.
// DO NOT CHANGE IT
namespace Genocs.Fiscanner.Contracts.Promotions;

public record VoucherIssuingCompleted
{
    public string? ReferenceId { get; init; }
    public string? RequestId { get; init; }
    public string? ContextId { get; init; }
    public string CampaignCode { get; init; }
    public string Issuer { get; init; }
    public string MemberId { get; init; }
}