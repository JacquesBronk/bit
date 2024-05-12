using Bit.Lib.App;

namespace Bit.Lib.Infra.Repositories;

public interface IFlagRepository
{
    Task<Result<bool>> IsEnabled(string flagName, CancellationToken cancellationToken = default);
    Task<Result<(string flagName, bool IsEnabled)>> FetchFlagStatus(string flagName, CancellationToken cancellationToken = default);
    Task<Result<(string flagName, bool IsEnabled)>> UpdateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default);
    Task<Result<(string flagName, bool IsEnabled)>>  CreateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default);
    Task<Result<(string flagName, bool IsEnabled)>>  DeleteFlag(string flagName, CancellationToken cancellationToken = default);
    Task<Result<List<KeyValuePair<string,bool>>>>  FetchAllFlags(CancellationToken cancellationToken = default);
    Task<Result<bool>> FlushCache(CancellationToken cancellationToken = default);
    Task<Result<int>> RedisKeyCount(CancellationToken cancellationToken = default);
}