namespace Bit.Lib.Common;

public static class EventCodes
{
    public const string FlagFetch = $"{Topics.Data}.{Topics.Flag}.fetch";
    public const string FlagUpdate = $"{Topics.Data}.{Topics.Flag}.update";
    public const string FlagCreate = $"{Topics.Data}.{Topics.Flag}.create";
    public const string FlagDelete = $"{Topics.Data}.{Topics.Flag}.delete";
    public const string FlagIsEnabled = $"{Topics.Data}.{Topics.Flag}.{Topics.Feature}.is-enabled";
    public const string FlagIsDisabled = $"{Topics.Data}.{Topics.Flag}.{Topics.Feature}.is-disabled";
    public const string FlagStatistics = $"{Topics.Data}.{Topics.Flag}.{Topics.Statistics}";
    public const string FlagMemoryCacheStatistics = $"{FlagStatistics}.{Topics.MemoryCache}";
    public const string FlagRedisCacheStatistics = $"{FlagStatistics}.{Topics.RedisCache}";
    public const string FlagKeyCountStatistics = $"{FlagStatistics}.{Topics.KeyCount}";
    public const string CacheFlushed = $"{Topics.Cache}.{Topics.Flush}.success";
}