namespace Genocs.TelegramIntegration.Contracts.Options;

/// <summary>
/// Redis settings.
/// </summary>
public class RedisSettings
{
    public const string Position = "Redis";

    /// <summary>
    /// The Admin connection string.
    /// </summary>
    public string ConnectionStringAdmin => $"{ConnectionStringTxn},allowAdmin=true";

    /// <summary>
    /// The connection string.
    /// </summary>
    public string? ConnectionStringTxn { get; internal set; }

    /// <summary>
    /// To String override behavior.
    /// </summary>
    /// <returns></returns>
    public override string? ToString()
    {
        return ConnectionStringTxn;
    }
}
