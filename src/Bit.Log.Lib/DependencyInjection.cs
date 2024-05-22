using System.Diagnostics.Metrics;
using Bit.Log.Extensions;
using Bit.Log.Infra.Configuration;
using Bit.Log.Infra.Jobs;
using Bit.Log.Infra.Telemetry;
using Bit.Log.Infra.Telemetry.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Bit.Log;

public static class DependencyInjection
{
    
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
        });
        
        builder.Services.AddHttpLogging(logging =>
        {
            logging.RequestHeaders.Add("Authorization");
            logging.ResponseHeaders.Add("Authorization");
            logging.MediaTypeOptions.AddText("application/json");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });

        return builder;
    } 


    private static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(x =>
        {
            x.IncludeScopes = true;
            x.IncludeFormattedMessage = true;
        });
        
        var hostName = System.Net.Dns.GetHostName();
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService("Bit", serviceVersion: "1.0.0")
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector()
            .AddAttributes(new Dictionary<string, object>
            {
                ["service.name"] = "Bit",
                ["service.instance.id"] = Environment.MachineName,
                ["host.name"] = hostName
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
                else
                {
                    x.SetSampler<ParentBasedSampler>();
                }
                

                
                x.AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(resourceBuilder);
            });

        builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
        builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());

        builder.Services.AddSingleton(provider =>
        {
            var monitor = provider.GetRequiredService<IOptionsMonitor<OpenTelemetryOptions>>();
            var options = monitor.CurrentValue.Metrics;

            var meter = new Meter("Bit.Log.Lib.Meter");
            var histogramBuilder = new HistogramBuilder(meter);
            histogramBuilder.AddHistogramsFromConfiguration(options);
            return histogramBuilder;
        });

        builder.Services.AddHostedService<HistogramRunner>();
        
        builder.Services.AddMetrics();

        return builder;
    }



    private static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live")
        });

        return app;
    }
}