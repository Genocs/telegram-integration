using Genocs.Monitoring;
using Genocs.Persistence.MongoDb.Extensions;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using Genocs.TelegramIntegration.Worker.Consumers;
using Genocs.TelegramIntegration.Worker.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization.Conventions;
using Serilog;
using Serilog.Events;
using System.Reflection;

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

        ConfigureCustomSettings(services);

        services.AddMongoDatabase(hostContext.Configuration);

        ConfigureMassTransit(services, hostContext.Configuration);
        ConfigureServices(services, hostContext.Configuration);

        // Add services to the container.
        services.AddHttpClient();

        // Start hosted service 
        //services.AddHostedService<MainHostedService>();
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


static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.Position));
    services.Configure<OpenAISettings>(configuration.GetSection(OpenAISettings.Position));

    services.TryAddSingleton<ITelegramProxy, TelegramProxy>();


    // Add Repository here

    return services;
}

/// <summary>
/// Setup Masstransit with RabbitMQ transport and MongoDB persistence layer
/// </summary>
/// <param name="services">The service collection</param>
static IServiceCollection ConfigureMassTransit(IServiceCollection services, IConfiguration configuration)
{
    //services.AddMediator();
    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

    var rabbitMQSettings = new RabbitMQSettings();
    configuration.GetSection(RabbitMQSettings.Position).Bind(rabbitMQSettings);

    services.AddSingleton(rabbitMQSettings);

    services.AddMassTransit(x =>
    {
        // Consumer
        //x.AddConsumersFromNamespaceContaining<RewardProcessedConsumer>();
        x.AddConsumers(Assembly.GetExecutingAssembly());
        x.AddActivities(Assembly.GetExecutingAssembly());
        x.SetKebabCaseEndpointNameFormatter();

        // Transport RabbitMQ
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
            //cfg.UseHealthCheck(context);
            cfg.Host(rabbitMQSettings.HostName, rabbitMQSettings.VirtualHost,
                h =>
                {
                    h.Username(rabbitMQSettings.UserName);
                    h.Password(rabbitMQSettings.Password);

                    //h.UseSsl(s =>
                    //{
                    //    s.Protocol = SslProtocols.Tls12;
                    //});
                }
            );
        });

        // Persistence MongoDB
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

static IServiceCollection ConfigureCustomSettings(IServiceCollection services)
{
    // MongoDb global Settings 
    var pack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true),
            new CamelCaseElementNameConvention()
    };
    ConventionRegistry.Register("Solution Conventions", pack, t => true);

    return services;
}


