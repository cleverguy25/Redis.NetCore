using System;
using System.Collections.Generic;
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

        public Task SetStringsAsync(IDictionary<string, string> dictionary)
        {
            var data = new List<byte[]>();
            foreach (var keyValue in dictionary)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value.ToBytes());
            }

            var request = ComposeRequest(RedisCommands.MultipleSet, data);
            return SendMultipleCommandAsync(request);
        }

        public async Task<bool> SetStringsNotExistsAsync(IDictionary<string, string> dictionary)
        {
            var data = new List<byte[]>();
            foreach (var keyValue in dictionary)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value.ToBytes());
            }

            var request = ComposeRequest(RedisCommands.MultipleSet, data);
            var bytes = await SendMultipleCommandAsync(request);
            return bytes[0][0] == '1';
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

        public Task<bool> SetStringNotExistsAsync(string key, string data, TimeSpan expiration)
        {
            return SetNotExistsAsync(key, data.ToBytes(), expiration);
        }

        public Task<bool> SetStringNotExistsAsync(string key, string data, int seconds)
        {
            return SetNotExistsAsync(key, data.ToBytes(), seconds);
        }

        public Task<bool> SetStringExistsAsync(string key, string data)
        {
            return SetExistsAsync(key, data.ToBytes());
        }

        public Task<bool> SetStringExistsAsync(string key, string data, TimeSpan expiration)
        {
            return SetExistsAsync(key, data.ToBytes(), expiration);
        }

        public Task<bool> SetStringExistsAsync(string key, string data, int seconds)
        {
            return SetExistsAsync(key, data.ToBytes(), seconds);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var bytes = await GetAsync(key);
            return ConvertBytesToString(bytes);
        }

        public async Task<string[]> GetStringsAsync(params string[] keys)
        {
            var response = await GetAsync(keys);
            return response.Select(ConvertBytesToString).ToArray();
        }

        private static string ConvertBytesToString(byte[] bytes)
        {
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }

        public async Task<int> AppendStringAsync(string key, string data)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Append, key.ToBytes(), data.ToBytes());
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> GetStringLengthAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.StringLength, key.ToBytes());
            return ConvertBytesToInteger(bytes);
        }

        public async Task<string> GetSetStringAsync(string key, string data)
        {
            var bytes = await GetSetAsync(key, data.ToBytes());
            return ConvertBytesToString(bytes);
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