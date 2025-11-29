// This namespace is needed to send/receive message to the promotion engine appropriately.
// DO NOT CHANGE IT
namespace Genocs.Fiscanner.Contracts.Notifications;

public record RewardProcessed
{
    public string? MemberId { get; init; }
    public string? NotificationTag { get; init; }
    public string? Language { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}