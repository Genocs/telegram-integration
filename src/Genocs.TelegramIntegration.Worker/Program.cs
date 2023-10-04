using Genocs.Core.Builders;
using Genocs.Core.Demo.Worker;
using Genocs.Logging;
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
        services.AddHostedService<MainHostedService>();

        services
            .AddGenocs(hostContext.Configuration)
            .AddMongoFast() // It adds the MongoDb Repository to the project and register all the Domain Objects with the standard interface
            .RegisterMongoRepositories(Assembly.GetExecutingAssembly()); // It registers the repositories that has been overridden. No need in case of standard repository

        services.AddCustomMassTransit(hostContext.Configuration);
        services.AddCustomOpenTelemetry(hostContext.Configuration);
        services.ConfigureServices(hostContext.Configuration);
        services.ConfigureCache(hostContext.Configuration);

        // Add services to the container.
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();

Log.CloseAndFlush();


//static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
//{
//    //configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));

//    //configurator.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipBatchEventConsumer>(), e =>
//    //{
//    //    e.PrefetchCount = 20;

//    //    e.Batch<RoutingSlipCompleted>(b =>
//    //    {
//    //        b.MessageLimit = 10;
//    //        b.TimeLimit = TimeSpan.FromSeconds(5);

//    //        b.Consumer<RoutingSlipBatchEventConsumer, RoutingSlipCompleted>(context);
//    //    });
//    //});

//    // This configuration allow to handle the Scheduling
//    configurator.UseMessageScheduler(new Uri("queue:quartz"));

//    // This configuration will configure the Activity Definition
//    configurator.ConfigureEndpoints(context);
//}