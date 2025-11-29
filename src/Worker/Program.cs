using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.Metrics.AppMetrics;
using Genocs.Persistence.MongoDb.Extensions;
using Genocs.TelegramIntegration.Infrastructure.Extensions;
using Genocs.Tracing;
using Serilog;

StaticLogger.EnsureInitialized();

IHost host = Host.CreateDefaultBuilder(args)
    .UseLogging()
    .ConfigureServices((hostContext, services) =>
    {
        // Run the hosted service
        // services.AddHostedService<MainHostedService>();
        services
            .AddGenocs(hostContext.Configuration)
            .AddOpenTelemetry()
            .AddMetrics()
            .AddMongoWithRegistration();

        services.AddApplicationServices(hostContext.Configuration);
        services.AddCustomCache(hostContext.Configuration);

        // Add services to the container.
        services.AddHttpClient();
    }).Build();

await host.RunAsync();

Log.CloseAndFlush();
