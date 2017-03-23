using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisRawClient
    {
        Task SetAsync(string key, byte[] data);

        Task SetAsync(IEnumerable<KeyValuePair<string, byte[]>> keyValues);

        Task SetAsync(string key, byte[] data, TimeSpan expiration);

        Task SetAsync(string key, byte[] data, int seconds);

        Task<bool> SetNotExistsAsync(string key, byte[] data);

        Task<byte[][]> GetAsync(params string[] keys);

        Task<byte[]> GetAsync(string key);

        Task<byte[]> GetSetAsync(string key, byte[] data);
    }
}
