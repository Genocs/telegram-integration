// This namespace is needed to send message to the promotion engine appropriately.
// DO NOT CHANGE IT
namespace Genocs.Fiscanner.Contracts.Notifications;

/// <summary>
/// Use this event to notify the payment completed.
/// </summary>
public record PaymentCompleted
{
    public string MemberId { get; init; }
    public string NotificationTag { get; init; }
    public string Language { get; init; }
    public Dictionary<string, string> Metadata { get; init; }
}