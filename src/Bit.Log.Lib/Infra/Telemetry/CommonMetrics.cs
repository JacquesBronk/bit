namespace Bit.Log.Infra.Telemetry;

public static class CommonMetrics
{
    public static class Infrastructure
    {
        public const string HostingMeter = "Microsoft.AspNetCore.Hosting";
        public const string KestrelMeter = "Microsoft.AspNetCore.Server.Kestrel";
    }

    public static class Traffic
    {
        public const string HttpConnectionsMeter = "Microsoft.AspNetCore.Http.Connections";
        public const string RoutingMeter = "Microsoft.AspNetCore.Routing";
        public const string DiagnosticsMeter = "Microsoft.AspNetCore.Diagnostics";
        public const string RateLimitingMeter = "Microsoft.AspNetCore.RateLimiting";
    }

    public static class Redis
    {
        public const string RedisMeter = "Microsoft.Extensions.Caching.StackExchangeRedis";
    }
}