using Genocs.Core.Domain.Repositories;
using Genocs.Persistence.MongoDb.Repositories.Clean;
using MongoDB.Bson;
using Telegram.BotAPI.GettingUpdates;

namespace Genocs.TelegramIntegration.Domains;

/// <summary>
/// This is the chat object received from the webhook.
/// </summary>
[TableMapping("ChatUpdates")]
public class ChatUpdate : Core.Domain.Entities.Entity<ObjectId>, IMongoDbEntity
{
    /// <summary>
    /// The chat message object received from the webhook.
    /// </summary>
    public Update Message { get; private set; }

    public bool Processed { get; set; }

    public ChatUpdate(Update update)
    {
        Message = update;
    }
}
