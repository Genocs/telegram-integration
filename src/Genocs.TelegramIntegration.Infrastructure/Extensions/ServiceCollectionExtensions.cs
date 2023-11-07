using Genocs.Integration.CognitiveServices.Interfaces;
using Genocs.Integration.CognitiveServices.Options;
using Genocs.Integration.CognitiveServices.Services;
using Genocs.TelegramIntegration.Contracts.Options;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Genocs.TelegramIntegration.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Setup MassTransit with RabbitMQ transport and MongoDB persistence layer.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection you can use to create chain.</returns>
    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddMediator();
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

        var rabbitMQSettings = new RabbitMQSettings();
        configuration.GetSection(RabbitMQSettings.Position).Bind(rabbitMQSettings);

        services.AddSingleton(rabbitMQSettings);

        services.AddMassTransit(x =>
        {
            // Consumer
            // x.AddConsumersFromNamespaceContaining<LangChainResponse>();
            x.AddConsumers(Assembly.GetEntryAssembly());

            // x.AddActivities(Assembly.GetExecutingAssembly());
            x.SetKebabCaseEndpointNameFormatter();

            // Transport RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);

                // cfg.UseHealthCheck(context);
                cfg.Host(
                         rabbitMQSettings.HostName,
                         rabbitMQSettings.VirtualHost,
                         rabbitMQSettings.ConnectionName,
                         h =>
                         {
                             h.Username(rabbitMQSettings.UserName);
                             h.Password(rabbitMQSettings.Password);

                             // h.UseSsl(s =>
                             // {
                             //    s.Protocol = SslProtocols.Tls12;
                             // });
                         });
            });

            // Persistence MongoDB
        });

        return services;
    }

    /// <summary>
    /// Configure Proxy and Settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection you can use to create chain.</returns>
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.Position));
        services.Configure<OpenAISettings>(configuration.GetSection(OpenAISettings.Position));
        services.Configure<ApiClientSettings>(configuration.GetSection(ApiClientSettings.Position));
        services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.Position));

        services.Configure<AzureCognitiveServicesSettings>(configuration.GetSection(AzureCognitiveServicesSettings.Position));
        services.Configure<AzureStorageSettings>(configuration.GetSection(AzureStorageSettings.Position));
        services.Configure<ImageClassifierSettings>(configuration.GetSection(ImageClassifierSettings.Position));
        services.Configure<AzureCognitiveServicesSettings>(configuration.GetSection(AzureCognitiveServicesSettings.Position));

        services.TryAddSingleton<IFormRecognizer, FormRecognizerService>();
        services.TryAddSingleton<IImageClassifier, ImageClassifierService>();

        services.TryAddSingleton<ITelegramProxy, TelegramProxy>();

        return services;
    }

    /// <summary>
    /// Add custom cache. The system can use Redis cache or default InMemory Cache.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection you can use to create chain.</returns>
    public static IServiceCollection AddCustomCache(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new RedisSettings();
        configuration.GetSection(RedisSettings.Position).Bind(settings);

        services.AddSingleton(settings);

        if (string.IsNullOrWhiteSpace(settings.ConnectionStringTxn))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.ConnectionStringTxn;
            });
        }

        return services;
    }
}
