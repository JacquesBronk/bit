using System.Diagnostics.Metrics;
using Bit.Log.Infra.Configuration;
using Bit.Log.Infra.Jobs;
using Bit.Log.Infra.Telemetry;
using Bit.Log.Infra.Telemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;

namespace Bit.Log;

public static class DependencyInjection
{
    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpLogging(logging =>
        {
            logging.RequestHeaders.Add("Authorization");
            logging.ResponseHeaders.Add("Authorization");
            logging.MediaTypeOptions.AddText("application/json");
            logging.MediaTypeOptions.AddText("application/xml");
            logging.MediaTypeOptions.AddText("text/plain");
            logging.MediaTypeOptions.AddText("text/html");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        }).AddLogging(logBuilder =>
        {
            logBuilder.AddJsonConsole();
            logBuilder.AddConfiguration(configuration.GetSection("Logging"));
        });

        return services;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(x =>
        {
            x.IncludeScopes = true;
            x.IncludeFormattedMessage = true;
        });
        
        builder.Services.AddOpenTelemetry()
            .WithMetrics(x =>
            {
                x.AddRuntimeInstrumentation()
                    .AddMeter(CommonMetrics.Infrastructure.HostingMeter,
                CommonMetrics.Infrastructure.KestrelMeter,
                CommonMetrics.Redis.RedisMeter,
                CommonMetrics.Traffic.RoutingMeter,
                CommonMetrics.Traffic.HttpConnectionsMeter,
                CommonMetrics.Traffic.RateLimitingMeter,
                CommonMetrics.Traffic.DiagnosticsMeter,
                CommonMetrics.Application.Bit);
            })
            .WithTracing(x =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    x.SetSampler<AlwaysOnSampler>();
                }
                
                var resourceBuilder = ResourceBuilder.CreateDefault()
                    .AddService("Bit", serviceInstanceId: Environment.MachineName)
                    .AddTelemetrySdk()
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("host.name", Environment.MachineName),
                        new KeyValuePair<string, object>("service.name", "Bit"),
                        new KeyValuePair<string, object>("service.version", Environment.Version.ToString() )
                    });

                x.ConfigureResource(_ => resourceBuilder.Build());
                
                x.AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();
    }

    public static IServiceCollection AddBitOpenTelemetry(this IServiceCollection services, Action<MeterProviderBuilder> configureMeters, string exporterType, IConfiguration configuration)
    {
        services.Configure<OpenTelemetryOptions>(configuration.GetSection("Logging:OpenTelemetry"));
    
        services.AddOpenTelemetry(
                )
            .WithMetrics(configureMeters)
            .WithTracing(tracing =>
            {
                var resourceBuilder = ResourceBuilder.CreateDefault()
                    .AddService("Bit", serviceInstanceId: Environment.MachineName)
                    .AddTelemetrySdk()
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("host.name", Environment.MachineName),
                        new KeyValuePair<string, object>("service.name", "Bit"),
                        new KeyValuePair<string, object>("service.version", Environment.Version.ToString() )
                    });

                tracing.SetResourceBuilder(resourceBuilder);

                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddOtlpExporter();
            });

        services.AddSingleton(provider =>
        {
            var monitor = provider.GetRequiredService<IOptionsMonitor<OpenTelemetryOptions>>();
            var options = monitor.CurrentValue.Metrics;

            var meter = new Meter("Bit.Log.Lib.Meter");
            var histogramBuilder = new HistogramBuilder(meter);
            histogramBuilder.AddHistogramsFromConfiguration(options);
            return histogramBuilder;
        });

        services.AddHostedService<HistogramRunner>();
        return services;
    }
}