using System.Diagnostics.Metrics;
using Bit.Lib.Domain.Service;
using Bit.Lib.Infra.Instrumentation.Jobs;
using Bit.Lib.Infra.Metrics;
using Bit.Lib.Infra.Repositories;
using Bit.Lib.Infra.ServiceMemoryCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;


namespace Bit.Lib;

public static class DependencyInjection
{
    public static IServiceCollection AddBitServices(this IServiceCollection services)
    {
        services.AddSingleton<ICacheManagerService, MemoryCacheManager>();
        services.AddSingleton<MemoryCacheManager>();
        services.AddScoped<IFlagRepository, FlagRepository>();
        services.AddScoped<IFlagService, FlagService>();
        
        services.AddSingleton<Histogram<double>>(_ =>
        {
            var meter = new Meter("Bit.Lib.Infra.Instrumentation");
            return meter.CreateHistogram<double>("redis_latency", "ms", "Redis latency");
        });
        services.AddHostedService<HistogramRunner>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<HistogramRunner>>();
            var histogram = provider.GetRequiredService<Histogram<double>>();
            var connectionMultiplexer = provider.GetRequiredService<ConnectionMultiplexer>();
            return new HistogramRunner(logger, histogram, connectionMultiplexer, TimeSpan.FromSeconds(30)); // Check every 30 seconds
        });
        
        services.AddSingleton<HttpCommonMetrics>();
        
        return services;
    }
}