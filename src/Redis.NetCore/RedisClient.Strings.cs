// <copyright file="RedisClient.Strings.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisStringClient
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

            var request = ComposeRequest(RedisCommands.MultipleSet, data.ToArray());
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

            var request = ComposeRequest(RedisCommands.MultipleSetNotExists, data.ToArray());
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
            return bytes.ConvertBytesToString();
        }

        public async Task<string[]> GetStringsAsync(params string[] keys)
        {
            var response = await GetAsync(keys).ConfigureAwait(false);
            return response.ConvertByteArrayToStringArray();
        }

        public async Task<string> GetStringRangeAsync(string key, int begin, int end = -1)
        {
            var response = await GetRangeAsync(key, begin, end).ConfigureAwait(false);
            return Encoding.UTF8.GetString(response);
        }

        public async Task<int> AppendStringAsync(string key, string data)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Append, key.ToBytes(), data.ToBytes()).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<int> GetStringLengthAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.StringLength, key.ToBytes()).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<string> GetSetStringAsync(string key, string data)
        {
            var bytes = await GetSetAsync(key, data.ToBytes()).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }
    }
}