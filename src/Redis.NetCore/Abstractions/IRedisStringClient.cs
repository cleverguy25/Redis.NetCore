using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisStringClient
    {
        Task SetStringAsync(string key, string data);

        Task SetStringAsync(string key, string data, TimeSpan expiration);

        Task SetStringAsync(string key, string data, int seconds);

        Task SetStringsAsync(IEnumerable<KeyValuePair<string, string>> keyValues);

        Task<bool> SetStringNotExistsAsync(string key, string data);

        Task<string> GetStringAsync(string key);

        Task<string[]> GetStringsAsync(params string[] keys);

        Task<string> GetSetStringAsync(string key, string data);

        Task<int> AppendStringAsync(string key, string data);
    }
}
