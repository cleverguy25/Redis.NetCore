using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisKeyClient
    {
        Task DeleteKeyAsync(params string[] keys);

        Task<int> GetTimeToLive(string key);

        Task<bool> MoveAsync(string key, int databaseIndex);

        Task<bool> ExistsAsync(string key);

        Task<int> ExistsAsync(params string[] keys);
    }
}
