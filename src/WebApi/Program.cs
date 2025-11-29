using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.Metrics.AppMetrics;
using Genocs.Persistence.MongoDb.Extensions;
using Genocs.TelegramIntegration.Infrastructure.Extensions;
using Genocs.Tracing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

StaticLogger.EnsureInitialized();

var builder = WebApplication.CreateBuilder(args);

builder.Host
        .UseLogging();

var services = builder.Services;

services
    .AddGenocs(builder.Configuration)
    .AddOpenTelemetry()
    .AddMetrics()
    .AddMongoWithRegistration();

services.AddCors();

services.AddControllers().AddJsonOptions(x =>
{
    // serialize Enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

services.AddHealthChecks();

services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(20);
    options.Predicate = check => check.Tags.Contains("ready");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Add services to the container.
services.AddHttpClient();

// Add MassTransit bus configuration
services.AddCustomMassTransit(builder.Configuration);
services.AddApplicationServices(builder.Configuration);
services.AddCustomCache(builder.Configuration);

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

app.MapHealthChecks("/hc");

app.Run();

Log.CloseAndFlush();