namespace Bit.Lib.Common;

public static class ExceptionCodes
{
    private const string ExceptionPrefix = "exception";
    private const string CriticalError = "critical.error";

    public static class Infrastructure
    {
        public const string ApiKeyNotFound = $"{Topics.Infra}.{Topics.Credential}.{Topics.Missing}.{ExceptionPrefix}.{Topics.Critical}.{Topics.ApiKey}";
        public const string RedisConnectionStringEmpty = $"{Topics.Infra}.{Topics.Connection}.{Topics.ConnectionString}.{Topics.Missing}.{CriticalError}.{Topics.Cache}";
        public const string CannotFlushCache = $"{Topics.Infra}.{Topics.Cache}.{Topics.Flush}.{ExceptionPrefix}.{Topics.Data}.{Topics.Error}";
        public const string CannotGetMemoryCacheStatistics = $"{Topics.Infra}.{Topics.Cache}.{Topics.MemoryCache}.{Topics.Statistics}.{ExceptionPrefix}";
        public const string CannotGetRedisCacheStatistics = $"{Topics.Infra}.{Topics.Cache}.{Topics.RedisCache}.{Topics.Statistics}.{ExceptionPrefix}";
        public const string Unauthorized = $"{Topics.Infra}.{Topics.Security}.{Topics.Unauthorized}.{ExceptionPrefix}";
    }

    public static class Data
    {
        public const string CannotCreateFlag = $"{Topics.Infra}.{Topics.Data}.{Topics.Create}.{ExceptionPrefix}";
        public const string CannotDeleteFlag = $"{Topics.Infra}.{Topics.Data}.{Topics.Delete}.{ExceptionPrefix}";
        public const string CannotFindKey = $"{Topics.Infra}.{Topics.Data}.{Topics.Information}.{Topics.Missing}";
        public const string CannotUpdateKey = $"{Topics.Infra}.{Topics.Data}.{Topics.Update}.{ExceptionPrefix}";
        public const string FeatureDisabled = $"{Topics.Infra}.{Topics.Feature}.{Topics.FlagIsDisabled}.{ExceptionPrefix}";
        public const string KeyNotFound = $"{Topics.Infra}.{Topics.Data}.{Topics.Missing}.{ExceptionPrefix}";
    }
}