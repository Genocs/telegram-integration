using Genocs.Monitoring;
using Genocs.Persistence.MongoDb.Extensions;
using Genocs.TelegramIntegration.Options;
using Genocs.TelegramIntegration.Services;
using Genocs.TelegramIntegration.Services.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("MassTransit", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((ctx, lc) =>
{
    lc.WriteTo.Console();
    lc.WriteTo.ApplicationInsights(new TelemetryConfiguration
    {
        ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights")
    }, TelemetryConverter.Traces);

});


// add services to DI container
var services = builder.Services;

// Set Custom Open telemetry
services.AddCustomOpenTelemetry(builder.Configuration);


services.AddMongoDatabase(builder.Configuration);

services.AddCors();
services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

services.AddHealthChecks();

// Add services to the container.
services.AddHttpClient();

services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2);
    options.Predicate = check => check.Tags.Contains("ready");
});

ConfigureServices(services, builder.Configuration);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddOptions();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// global cors policy
app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();

Log.CloseAndFlush();

static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<TelegramSettings>(configuration.GetSection(TelegramSettings.Position));
    services.Configure<OpenAISettings>(configuration.GetSection(OpenAISettings.Position));
    services.TryAddScoped<ITelegramProxy, TelegramProxy>();

    return services;
}
