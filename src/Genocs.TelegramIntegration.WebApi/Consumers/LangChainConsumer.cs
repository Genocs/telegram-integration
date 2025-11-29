using Genocs.Fiscanner.Contracts.Notifications;
using MassTransit;

namespace Genocs.TelegramIntegration.WebApi.Consumers;

public class LangChainResponseConsumer : IConsumer<LangChainResponse>
{
    private readonly ILogger<LangChainResponseConsumer> _logger;

    public LangChainResponseConsumer(
                                        ILogger<LangChainResponseConsumer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    }

    public async Task Consume(ConsumeContext<LangChainResponse> context)
    {
        _logger.LogInformation("Received LangChainResponse");
        await Task.CompletedTask;
    }
}
