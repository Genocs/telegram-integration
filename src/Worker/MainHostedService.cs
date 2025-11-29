using Genocs.TelegramIntegration.Services.Interfaces;

namespace Genocs.TelegramIntegration.Worker;

public class MainHostedService(ILogger<MainHostedService> logger, ITelegramProxy proxy) : IHostedService, IDisposable
{
    private readonly ILogger<MainHostedService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ITelegramProxy _proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));

    private Timer? _timer = null;
    private bool _disposed = false;

    private int _executionCount;

    public void Dispose()
    {
        if (_disposed)
        {
            _disposed = true;
            _timer?.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(
                            DoWork,
                            null,
                            TimeSpan.Zero,
                            TimeSpan.FromSeconds(20));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;

        // return _bus.StopAsync(cancellationToken);
    }

    private void DoWork(object? state)
    {
        int count = Interlocked.Increment(ref _executionCount);

        Task.Run(_proxy.PullUpdatesAsync);

        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
    }
}
