using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisKeyClient
    {
        Task<string[]> GetKeysAsync(string pattern);

        Task DeleteKeyAsync(params string[] keys);

        Task<int> GetTimeToLiveAsync(string key);

        Task<long> GetPreciseTimeToLiveAsync(string key);

        Task<bool> MoveAsync(string key, int databaseIndex);

        Task<bool> ExistsAsync(string key);

        Task<int> ExistsAsync(params string[] keys);

        Task<bool> SetExpirationAsync(string key, TimeSpan expiration);

        Task<bool> SetExpirationAsync(string key, int seconds);

        Task<bool> SetExpirationAsync(string key, DateTime dateTime);

        Task<bool> PersistAsync(string key);

        Task RenameKeyAsync(string key, string newKey);

        Task<bool> RenameKeyNotExistsAsync(string key, string newKey);

        Task<string> GetRandomKeyAsync();

        Task<ScanCursor> ScanAsync();

        Task<ScanCursor> ScanAsync(ScanCursor cursor);

        Task TouchAsync(string key);

        Task<string> GetTypeAsync(string key);
    }
}
