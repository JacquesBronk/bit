using System.Runtime.InteropServices;
using Bit.EndpointHandlers;
using Bit.Lib;
using Bit.Lib.Common.Exception;
using Bit.Lib.Infra;
using Bit.Lib.Infra.Os;
using Bit.Middleware;
using Microsoft.AspNetCore.HttpLogging;
using StackExchange.Redis;

namespace Bit;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureAppConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "appsettings.json")))
        {
            builder.Configuration.AddDockerSecrets();
        }

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

        var redisHost = configuration.GetConnectionString("Redis") ?? throw new RedisConnectionStringEmptyException(nameof(WebApplication.CreateBuilder), "Redis connection string missing",[],new ArgumentNullException(nameof(configuration)));
        services.AddSingleton(ConnectionMultiplexer.Connect(redisHost));
        services.AddMemoryCache();
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add("Authorization");
            logging.ResponseHeaders.Add("Authorization");
            logging.MediaTypeOptions.AddText("application/json");
            logging.MediaTypeOptions.AddText("application/xml");
            logging.MediaTypeOptions.AddText("text/plain");
            logging.MediaTypeOptions.AddText("text/html");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });
        services.AddLogging();
        services.AddMetrics();
        services.AddBitServices();

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

        app.MapGet("/", () => string.Empty);
        app.MapEndpoints();

        return app;
    }
    
    private static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{flag}", BitEndpoints.MapGetFlagEndpointHandler).WithName("Get").WithOpenApi();
        app.MapGet("/isEnabled/{flag}", BitEndpoints.MapIsEnabledEndpoint).WithName("IsEnabled").WithOpenApi();
        app.MapPatch("/update/{flag}/{enabled}", BitEndpoints.MapUpdateFlagEndpoint).WithName("Update").WithOpenApi();
        app.MapPost("/create/{flag}/{enabled}", BitEndpoints.MapCreateFlagEndpoint).WithName("Create").WithOpenApi();
        app.MapDelete("/delete/{flag}", BitEndpoints.MapDeleteFlagEndpoint).WithName("Delete").WithOpenApi();
        app.MapPost("/flush", BitEndpoints.MapFlushEndpoint).WithName("Flush").WithOpenApi();
    }
}