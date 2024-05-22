using System.Runtime.InteropServices;
using Bit.EndpointHandlers;
using Bit.Lib;
using Bit.Lib.Infra;
using Bit.Lib.Infra.Os;
using Bit.Log;
using Bit.Log.Common.Exception;
using Bit.Log.Infra.Telemetry;
using Bit.Middleware;
using StackExchange.Redis;

namespace Bit;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureAppConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .AddIniFile("conf/bit.ini", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddDockerSecrets();

        return builder;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string apiKey = configuration["ApiKey"] ?? throw new ApiKeyNotFoundException(nameof(WebApplication.CreateBuilder), []);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "echo %API_KEY%" : "echo $API_KEY";
            apiKey = OsInterop.ExecuteCommand(command).Trim();
            configuration["ApiKey"] = apiKey;
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ApiKeyNotFoundException(nameof(WebApplication.CreateBuilder), []);
        }
       
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c => c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Bit.Lib.xml")));
        services.AddHealthChecks();

        var redisHost = configuration.GetConnectionString("Redis") ?? throw new RedisConnectionStringEmptyException(nameof(WebApplication.CreateBuilder), "Redis connection string missing", [], new ArgumentNullException(nameof(configuration)));
        services.AddSingleton(ConnectionMultiplexer.Connect(redisHost));
        services.AddMemoryCache(x => x.SizeLimit = 40 * 1024 * 1024);
        services.AddBitServices();

        services.AddLogging(configuration)
            .AddBitOpenTelemetry(meterBuilder =>
            {
                meterBuilder.AddMeter(CommonMetrics.Infrastructure.HostingMeter);
                meterBuilder.AddMeter(CommonMetrics.Infrastructure.KestrelMeter);
                meterBuilder.AddMeter(CommonMetrics.Redis.RedisMeter);
                meterBuilder.AddMeter(CommonMetrics.Traffic.RoutingMeter);
                meterBuilder.AddMeter(CommonMetrics.Traffic.HttpConnectionsMeter);
                meterBuilder.AddMeter(CommonMetrics.Traffic.RateLimitingMeter);
                meterBuilder.AddMeter(CommonMetrics.Traffic.DiagnosticsMeter);
            }, MetricExporterType.Jaeger, configuration
        );

        return services;
    }

    public static WebApplication ConfigureMiddlewareAndEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        
        app.MapEndpoints();
        return app;
    }

    private static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => DateTime.UtcNow.ToString("O"));
        app.MapGet("/isEnabled/{flag}", BitEndpoints.MapIsEnabledEndpoint).WithName("Get").WithOpenApi();
        app.MapPatch("/update/{flag}/{enabled}", BitEndpoints.MapUpdateFlagEndpoint).WithName("Update").WithOpenApi();
        app.MapPost("/create/{flag}/{enabled}", BitEndpoints.MapCreateFlagEndpoint).WithName("Create").WithOpenApi();
        app.MapDelete("/delete/{flag}", BitEndpoints.MapDeleteFlagEndpoint).WithName("Delete").WithOpenApi();
        app.MapPost("/flush", BitEndpoints.MapFlushEndpoint).WithName("Flush").WithOpenApi();
    }
}