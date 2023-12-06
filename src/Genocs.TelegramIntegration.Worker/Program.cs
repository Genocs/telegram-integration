using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.Metrics.AppMetrics;
using Genocs.Monitoring;
using Genocs.Persistence.MongoDb.Extensions;
using Genocs.TelegramIntegration.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseLogging()
    .ConfigureServices((hostContext, services) =>
    {
        // Run the hosted service
        // services.AddHostedService<MainHostedService>();

        services
            .AddGenocs(hostContext.Configuration)
            .AddMetrics()
            .AddMongoFast() // It adds the MongoDb Repository to the project and register all the Domain Objects with the standard interface
            .RegisterMongoRepositories(Assembly.GetExecutingAssembly()); // It registers the repositories that has been overridden. No need in case of standard repository

        services.AddCustomMassTransit(hostContext.Configuration);
        services.AddCustomOpenTelemetry(hostContext.Configuration);
        services.AddCustomServices(hostContext.Configuration);
        services.AddCustomCache(hostContext.Configuration);

        // Add services to the container.
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();

Log.CloseAndFlush();
