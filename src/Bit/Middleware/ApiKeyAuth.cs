using Bit.Log.Common.Exception;

//NOT FOR PRODUCTION USE!!
namespace Bit.Middleware;

public class ApiKeyAuthenticationMiddleware(RequestDelegate next)
{
    private const string ApiKeyName = "x-api-key";

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/health") || context.Request.Path.StartsWithSegments("/alive"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(new UnAuthorizedError(nameof(Bit), "Unauthorized client. Api key not found.", new string[] { ApiKeyName }).ToString());
            return;
        }

        var apiKey = configuration["ApiKey"];

        if (apiKey == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(new UnAuthorizedError(nameof(Bit), "Unauthorized client. Api key not found.", new string[] { ApiKeyName }).ToString());
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(new UnAuthorizedError(nameof(Bit), "Unauthorized client.", new string[] { ApiKeyName }).ToString());
            return;
        }

        await next(context);
    }
}