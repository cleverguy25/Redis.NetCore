using System;
using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore
{
    public interface IRedisClient : IDisposable
    {
        Task SetAsync(string key, byte[] data);

        Task SetAsync(string key, byte[] data, TimeSpan expiration);

        Task SetAsync(string key, byte[] data, int seconds);

        Task<byte[]> GetAsync(string key);

        Task<byte[][]> GetAsync(params string[] key);

        Task<int> GetTimeToLive(string key);

        Task SetStringAsync(string key, string data);

        Task SetStringAsync(string key, string data, TimeSpan expiration);

        Task SetStringAsync(string key, string data, int seconds);

        Task<string> GetStringAsync(string key);

        Task<string[]> GetStringAsync(params string[] key);

    }
}