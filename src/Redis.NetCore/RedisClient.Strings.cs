﻿using System;
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

            var request = ComposeRequest(RedisCommands.MultipleSetNotExists, data);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
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

        public Task<int> SetStringRangeAsync(string key, int offset, string data)
        {
            return SetRangeAsync(key, offset, data.ToBytes());
        }

        public async Task<string> GetStringAsync(string key)
        {
            var bytes = await GetAsync(key).ConfigureAwait(false);
            return ConvertBytesToString(bytes);
        }

        public async Task<string[]> GetStringsAsync(params string[] keys)
        {
            var response = await GetAsync(keys).ConfigureAwait(false);
            return response.Select(ConvertBytesToString).ToArray();
        }

        public async Task<string> GetStringRangeAsync(string key, int begin, int end = -1)
        {
            var response = await GetRangeAsync(key, begin, end).ConfigureAwait(false);
            return Encoding.UTF8.GetString(response);
        }

        private static string ConvertBytesToString(byte[] bytes)
        {
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }

        public async Task<int> AppendStringAsync(string key, string data)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Append, key.ToBytes(), data.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> GetStringLengthAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.StringLength, key.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<string> GetSetStringAsync(string key, string data)
        {
            var bytes = await GetSetAsync(key, data.ToBytes()).ConfigureAwait(false);
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