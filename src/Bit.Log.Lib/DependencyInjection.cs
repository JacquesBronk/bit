using System.Diagnostics.Metrics;
using Bit.Log.Infra.Configuration;
using Bit.Log.Infra.Jobs;
using Bit.Log.Infra.Telemetry;
using Bit.Log.Infra.Telemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;

namespace Bit.Log;

public static class DependencyInjection
{
    public static IServiceCollection AddDefaultLogging(this IServiceCollection services, IConfiguration configuration)
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
        });

        services.AddLogging(logBuilder =>
        {
            logBuilder.AddJsonConsole();
            logBuilder.AddConfiguration(configuration.GetSection("Logging"));
            
        });
        services.AddMetrics();
        services.AddAllBitMeters(MetricExporterType.OpenTelemetryCollector);

        return services;
    }

    public static IServiceCollection AddBitMeters(this IServiceCollection services, Action<MeterProviderBuilder> configureMeters, string exporterType)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddIniFile("conf/bit.ini", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            
            .Build();

        services.Configure<OpenTelemetryOptions>(configuration.GetSection("Logging:OpenTelemetry"));

        services.AddSingleton<MetricExporterStrategyFactory>();
        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            var serviceProvider = services.BuildServiceProvider();
            configureMeters(builder);
            var exporterStrategyFactory = serviceProvider.GetRequiredService<MetricExporterStrategyFactory>();
            var exporterStrategy = exporterStrategyFactory.CreateExporterStrategy(exporterType);
            exporterStrategy.ConfigureExporter(builder);
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

    public static IServiceCollection AddAllBitMeters(this IServiceCollection services, string exporterType)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddIniFile("conf/bit.ini", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.Configure<OpenTelemetryOptions>(configuration.GetSection("Logging:OpenTelemetry"));


        services.AddSingleton<MetricExporterStrategyFactory>();

        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<OpenTelemetryOptions>>();
            var metricsOptions = optionsMonitor.CurrentValue.Metrics;


            builder
                .AddMeter(CommonMetrics.Infrastructure.HostingMeter)
                .AddMeter(CommonMetrics.Infrastructure.KestrelMeter)
                .AddMeter(CommonMetrics.Redis.RedisMeter)
                .AddMeter(CommonMetrics.Traffic.HttpConnectionsMeter)
                .AddMeter(CommonMetrics.Traffic.RoutingMeter)
                .AddMeter(CommonMetrics.Traffic.DiagnosticsMeter)
                .AddMeter(CommonMetrics.Traffic.RateLimitingMeter);

            if (metricsOptions != null)
            {
                var meter = new Meter("Bit.Log.Lib.Meter");
                var histogramBuilder = new HistogramBuilder(meter);
                histogramBuilder.AddHistogramsFromConfiguration(metricsOptions);
            }

            var exporterStrategyFactory = serviceProvider.GetRequiredService<MetricExporterStrategyFactory>();
            var exporterStrategy = exporterStrategyFactory.CreateExporterStrategy(exporterType);
            exporterStrategy.ConfigureExporter(builder);
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