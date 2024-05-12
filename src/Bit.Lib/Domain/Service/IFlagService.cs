using CloudNative.CloudEvents;

namespace Bit.Lib.Domain.Service;

/// <summary>
/// IFlagService is an interface that defines methods for managing feature flags.
/// </summary>
public interface IFlagService
{
    /// <summary>
    /// Retrieves the specified flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the flag data.</returns>
    Task<CloudEvent> GetFlag(string flagName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the specified flag is enabled.
    /// </summary>
    /// <param name="flagName">The name of the flag to check.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the flag status.</returns>
    Task<CloudEvent> IsFlagEnabled(string flagName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of the specified flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to update.</param>
    /// <param name="isEnabled">The new status of the flag.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the updated flag data.</returns>
    Task<CloudEvent> UpdateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new flag with the specified status.
    /// </summary>
    /// <param name="flagName">The name of the flag to create.</param>
    /// <param name="isEnabled">The initial status of the flag.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the created flag data.</returns>
    Task<CloudEvent> CreateFlag(string flagName, bool isEnabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to delete.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the deleted flag data.</returns>
    Task<CloudEvent> DeleteFlag(string flagName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes the cache.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the cache flush operation.</returns>
    Task<CloudEvent> FlushCache(CancellationToken cancellationToken = default);

    
    /// <summary>
    /// Retrieves the statistics of the memory cache for the specified flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to retrieve the statistics for.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the memory cache statistics.</returns>
    Task<CloudEvent> GetMemCacheStats(string flagName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the statistics of the Redis cache for the specified flag.
    /// </summary>
    /// <param name="flagName">The name of the flag to retrieve the statistics for.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a CloudEvent with the Redis cache statistics.</returns>
    Task<CloudEvent> GetRedisCacheStats(string flagName, CancellationToken cancellationToken = default);
}