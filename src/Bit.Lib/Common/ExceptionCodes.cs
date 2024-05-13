namespace Bit.Lib.Common;

public static class ExceptionCodes
{
    private const string ExceptionPrefix = "exception";
    private const string CriticalError = "critical.error";

    public static class Infrastructure
    {
        public const string ApiKeyNotFound = $"{TopicDictionary.Infra}.{TopicDictionary.Credential}.{TopicDictionary.Missing}.{ExceptionPrefix}.{TopicDictionary.Critical}.{TopicDictionary.ApiKey}";
        public const string RedisConnectionStringEmpty = $"{TopicDictionary.Infra}.{TopicDictionary.Connection}.{TopicDictionary.ConnectionString}.{TopicDictionary.Missing}.{CriticalError}.{TopicDictionary.Cache}";
        public const string CannotFlushCache = $"{TopicDictionary.Infra}.{TopicDictionary.Cache}.{TopicDictionary.Flush}.{ExceptionPrefix}.{TopicDictionary.Data}.{TopicDictionary.Error}";
        public const string CannotGetMemoryCacheStatistics = $"{TopicDictionary.Infra}.{TopicDictionary.Cache}.{TopicDictionary.MemoryCache}.{TopicDictionary.Statistics}.{ExceptionPrefix}";
        public const string CannotGetRedisCacheStatistics = $"{TopicDictionary.Infra}.{TopicDictionary.Cache}.{TopicDictionary.RedisCache}.{TopicDictionary.Statistics}.{ExceptionPrefix}";
        public const string Unauthorized = $"{TopicDictionary.Infra}.{TopicDictionary.Security}.{TopicDictionary.Unauthorized}.{ExceptionPrefix}";
    }

    public static class Data
    {
        public const string CannotCreateFlag = $"{TopicDictionary.Infra}.{TopicDictionary.Data}.{TopicDictionary.Create}.{ExceptionPrefix}";
        public const string CannotDeleteFlag = $"{TopicDictionary.Infra}.{TopicDictionary.Data}.{TopicDictionary.Delete}.{ExceptionPrefix}";
        public const string CannotFindKey = $"{TopicDictionary.Infra}.{TopicDictionary.Data}.{TopicDictionary.Information}.{TopicDictionary.Missing}";
        public const string CannotUpdateKey = $"{TopicDictionary.Infra}.{TopicDictionary.Data}.{TopicDictionary.Update}.{ExceptionPrefix}";
        public const string FeatureDisabled = $"{TopicDictionary.Infra}.{TopicDictionary.Feature}.{TopicDictionary.FlagIsDisabled}.{ExceptionPrefix}";
        public const string KeyNotFound = $"{TopicDictionary.Infra}.{TopicDictionary.Data}.{TopicDictionary.Missing}.{ExceptionPrefix}";
    }
}