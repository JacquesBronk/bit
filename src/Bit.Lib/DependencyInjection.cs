using Bit.Lib.Domain.Service;
using Bit.Lib.Infra.Repositories;
using Bit.Lib.Infra.ServiceMemoryCache;
using Microsoft.Extensions.DependencyInjection;


namespace Bit.Lib;

public static class DependencyInjection
{
    public static IServiceCollection AddBitServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
   
        });
        services.AddMemoryCache();
      
        services.AddSingleton<ICacheManagerService, MemoryCacheManager>();
        services.AddSingleton<MemoryCacheManager>(); // Add this line
        services.AddScoped<IFlagRepository, FlagRepository>();
        services.AddScoped<IFlagService, FlagService>();
        return services;
    }
}