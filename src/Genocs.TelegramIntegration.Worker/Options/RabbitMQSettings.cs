namespace Genocs.TelegramIntegration.Worker.Options;

public class RabbitMQSettings
{
    public static string Position = "RabbitMq";

    public string HostName { get; set; } = default!;
    public string VirtualHost { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool UseSSL { get; set; }
    public int Port { get; set; } = 5672;
}
