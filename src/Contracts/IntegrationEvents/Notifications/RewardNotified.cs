// This namespace is needed to send message to the promotion engine appropriately.
// DO NOT CHANGE IT
namespace Genocs.Fiscanner.Contracts.Notifications;

/// <summary>
/// This event is raised when a reward is notified to the member. This could happened whenever the member received a discount or got a Voucher.
/// </summary>
public record RewardNotified
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
    /// The Member Id that is associated to the reward.
    /// </summary>
    public string? MemberId { get; init; }

    /// <summary>
    /// The Notification Tag that is associated to the reward.
    /// </summary>
    /// at the time of writing this code could be voucher_issued | discount_received
    public string? NotificationTag { get; init; }

    /// <summary>
    /// Generic metadata that could be associated to the reward.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}