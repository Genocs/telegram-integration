namespace Genocs.Fiscanner.Contracts.Notifications;

public interface RewardProcessed
{
    string MemberId { get; }
    string NotificationTag { get; }
    string Language { get; }
    Dictionary<string, string> Metadata { get; }
}