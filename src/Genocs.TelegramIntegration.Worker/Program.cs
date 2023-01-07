using Genocs.Core.Demo.Worker;
using Genocs.Monitoring;
using Genocs.Persistence.MongoDb.Options;
using Genocs.Persistence.MongoDb;
using Genocs.Persistence.MongoDb.Repositories;
using Genocs.TelegramIntegration.Domains;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;
using MassTransit;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, builder) =>
    {
        builder.AddUserSecrets<Program>();
    })
    .ConfigureServices((hostContext, services) =>
    {
        //TelemetryAndLogging.Initialize(hostContext.Configuration.GetConnectionString("ApplicationInsights"));
        services.AddCustomOpenTelemetry(hostContext.Configuration);

        ConfigureMongoDb(services, hostContext.Configuration);
        ConfigureMassTransit(services, hostContext.Configuration);
        ConfigureServices(services, hostContext.Configuration);

        // Add services to the container.
        services.AddHttpClient();

        // Start hosted service 
        services.AddHostedService<MainHostedService>();


        services.AddTransient<IMongoDbRepository<GenocsChat>, MongoDbRepository<GenocsChat>>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(dispose: true);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();

await TelemetryAndLogging.FlushAndCloseAsync();

Log.CloseAndFlush();



static IServiceCollection ConfigureMongoDb(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<DBSettings>(configuration.GetSection(DBSettings.Position));

    services.TryAddSingleton<IMongoDatabaseProvider, MongoDatabaseProvider>();
    services.TryAddSingleton<IMongoDbRepository<GenocsChat>, MongoDbRepositoryBase<GenocsChat>>();

    // Add Repository here

    return services;
}

static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.Position));
    services.Configure<OpenAISettings>(configuration.GetSection(OpenAISettings.Position));

    services.TryAddSingleton<ITelegramProxy, TelegramProxy>();

    //services.TryAddSingleton<IRepository<Order, string>, MongoDbRepositoryBase<Order, string>>();

    // Add Repository here

    return services;
}

static IServiceCollection ConfigureMassTransit(IServiceCollection services, IConfiguration configuration)
{
    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
    services.AddMassTransit(cfg =>
    {
        // Consumer configuration
        //cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

        // Set the transport
        cfg.UsingRabbitMq(ConfigureBus);
    });

    return services;
}

static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
{
    //configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));

    //configurator.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipBatchEventConsumer>(), e =>
    //{
    //    e.PrefetchCount = 20;

    //    e.Batch<RoutingSlipCompleted>(b =>
    //    {
    //        b.MessageLimit = 10;
    //        b.TimeLimit = TimeSpan.FromSeconds(5);

    //        b.Consumer<RoutingSlipBatchEventConsumer, RoutingSlipCompleted>(context);
    //    });
    //});

    // This configuration allow to handle the Scheduling
    configurator.UseMessageScheduler(new Uri("queue:quartz"));

    // This configuration will configure the Activity Definition
    configurator.ConfigureEndpoints(context);
}


