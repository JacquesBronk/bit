namespace Bit.Lib.Common;

public static class EventCodes
{
    public const string FlagFetch = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.fetch";
    public const string FlagUpdate = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.update";
    public const string FlagCreate = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.create";
    public const string FlagDelete = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.delete";
    public const string FlagIsEnabled = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.{TopicDictionary.Feature}.is-enabled";
    public const string FlagIsDisabled = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.{TopicDictionary.Feature}.is-disabled";
    public const string FlagStatistics = $"{TopicDictionary.Data}.{TopicDictionary.Flag}.{TopicDictionary.Statistics}";
    public const string FlagMemoryCacheStatistics = $"{FlagStatistics}.{TopicDictionary.MemoryCache}";
    public const string FlagRedisCacheStatistics = $"{FlagStatistics}.{TopicDictionary.RedisCache}";
    public const string FlagKeyCountStatistics = $"{FlagStatistics}.{TopicDictionary.KeyCount}";
    public const string CacheFlushed = $"{TopicDictionary.Cache}.{TopicDictionary.Flush}.success";
}