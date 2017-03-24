using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisRawClient
    {
        Task SetAsync(string key, byte[] data);

        Task SetAsync(IDictionary<string, byte[]> dictionary);

        Task<bool> SetNotExistsAsync(IDictionary<string, byte[]> dictionary);

        Task SetAsync(string key, byte[] data, TimeSpan expiration);

        Task SetAsync(string key, byte[] data, int seconds);

        Task<bool> SetNotExistsAsync(string key, byte[] data);

        Task<bool> SetNotExistsAsync(string key, byte[] data, TimeSpan expiration);

        Task<bool> SetNotExistsAsync(string key, byte[] data, int seconds);

        Task<bool> SetExistsAsync(string key, byte[] data);

        Task<bool> SetExistsAsync(string key, byte[] data, TimeSpan expiration);

        Task<bool> SetExistsAsync(string key, byte[] data, int seconds);

        Task<byte[][]> GetAsync(params string[] keys);

        Task<byte[]> GetAsync(string key);

        Task<byte[]> GetSetAsync(string key, byte[] data);
    }
}
