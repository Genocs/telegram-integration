namespace Genocs.TelegramIntegration.Contracts.Options;
public class RedisSettings
{
    public const string Position = "Redis";
    public string ConnectionStringAdmin => $"{ConnectionStringTxn},allowAdmin=true";

    public string ConnectionStringTxn { get; internal set; }

    public override string ToString()
    {
        return ConnectionStringTxn;
    }
}
