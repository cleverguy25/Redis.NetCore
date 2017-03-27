using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisKeyClient
    {
        Task DeleteKeyAsync(params string[] keys);

        Task<int> GetTimeToLive(string key);

        Task<long> GetPreciseTimeToLive(string key);

        Task<bool> MoveAsync(string key, int databaseIndex);

        Task<bool> ExistsAsync(string key);

        Task<int> ExistsAsync(params string[] keys);

        Task<bool> SetExpirationAsync(string key, TimeSpan expiration);

        Task<bool> SetExpirationAsync(string key, int seconds);

        Task<bool> SetExpirationAsync(string key, DateTime dateTime);

        Task<bool> PersistAsync(string key);
    }
}
