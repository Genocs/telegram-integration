using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.Metrics.AppMetrics;
using Genocs.Persistence.MongoDb.Extensions;
using Genocs.TelegramIntegration.Infrastructure.Extensions;
using Genocs.Tracing;
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

var builder = Host.CreateDefaultBuilder(args);

builder.UseLogging();

builder.ConfigureServices((hostContext, services) =>
    {
        // Run the hosted service
        // services.AddHostedService<MainHostedService>();

        services
            .AddGenocs(hostContext.Configuration)
            .AddOpenTelemetry()
            .AddMetrics()
            .AddMongoFast() // It adds the MongoDb Repository to the project and register all the Domain Objects with the standard interface
            .RegisterMongoRepositories(Assembly.GetExecutingAssembly()); // It registers the repositories that has been overridden. No need in case of standard repository

        services.AddApplicationServices(hostContext.Configuration);
        services.AddCustomCache(hostContext.Configuration);

        // Add services to the container.
        services.AddHttpClient();
    });

var host = builder.Build();
await host.RunAsync();

Log.CloseAndFlush();
