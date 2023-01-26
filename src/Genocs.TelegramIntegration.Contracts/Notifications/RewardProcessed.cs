using System.Collections.Generic;

namespace Genocs.Fiscanner.Contracts.Notifications
{
    public interface RewardProcessed
    {
        string MemberId { get; }
        string NotificationTag { get; }
        Dictionary<string, string> Metadata { get; }
    }
}