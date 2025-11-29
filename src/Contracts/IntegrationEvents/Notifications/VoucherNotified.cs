// This namespace is needed to send message to the promotion engine appropriately.
// DO NOT CHANGE IT
namespace Genocs.Fiscanner.Contracts.Notifications;

/// <summary>
/// This event is raised when a voucher is notified to the member.
/// </summary>
public record VoucherNotified
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

    public string? MemberId { get; init; }
    public string? NotificationTag { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}