using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
        public Task SetStringAsync(string key, string data)
        {
            return SetAsync(key, data.ToBytes());
        }

        public Task SetStringsAsync(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var data = new List<byte[]>();
            foreach (var keyValue in keyValues)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value.ToBytes());
            }

            var request = ComposeRequest(RedisCommands.MultipleSet, data);
            return SendMultipleCommandAsync(request);
        }

        public Task SetStringAsync(string key, string data, TimeSpan expiration)
        {
            return SetAsync(key, data.ToBytes(), expiration);
        }

        public Task SetStringAsync(string key, string data, int seconds)
        {
            return SetAsync(key, data.ToBytes(), seconds);
        }

        public Task<bool> SetStringNotExistsAsync(string key, string data)
        {
            return SetNotExistsAsync(key, data.ToBytes());
        }

        public async Task<string> GetStringAsync(string key)
        {
            var bytes = await GetAsync(key);
            return Encoding.UTF8.GetString(bytes);
        }

        public async Task<string[]> GetStringsAsync(params string[] keys)
        {
            var response = await GetAsync(keys);
            return response.Select(
                                   bytes => bytes == null ?
                                       null : Encoding.UTF8.GetString(bytes)).ToArray();
        }

        public async Task<int> AppendStringAsync(string key, string data)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Append, key.ToBytes(), data.ToBytes());
            return ConvertBytesToInteger(bytes);
        }

        public async Task<string> GetSetStringAsync(string key, string data)
        {
            var bytes = await GetSetAsync(key, data.ToBytes());
            return Encoding.UTF8.GetString(bytes);
        }

        private static void CheckKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
            }
        }
    }
}