namespace Genocs.Fiscanner.Contracts.Notifications;

public record RewardNotified
{
    public string MemberId { get; init; }
    public string NotificationTag { get; init; }
    public string Language { get; init; }
    public Dictionary<string, string> Metadata { get; init; }
}