using Bit.Log.Domain;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
namespace Bit.Log;

public static class DependencyInjection
{
    public static IServiceCollection AddBitMeters(this IServiceCollection services, Action<MeterProviderBuilder> configureMeters)
    {
        services.AddOpenTelemetry().WithMetrics(configureMeters);
        return services;
    }

    public static IServiceCollection AddAllBitMeters(this IServiceCollection services)
    {
        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            builder
                .AddMeter(Telemetry.Infrastructure.HostingMeter)
                .AddMeter(Telemetry.Infrastructure.KestrelMeter)
                .AddMeter(Telemetry.Redis.RedisMeter)
                .AddMeter(Telemetry.Traffic.HttpConnectionsMeter)
                .AddMeter(Telemetry.Traffic.RoutingMeter)
                .AddMeter(Telemetry.Traffic.DiagnosticsMeter)
                .AddMeter(Telemetry.Traffic.RateLimitingMeter);
            
        });
        return services;
    }
}