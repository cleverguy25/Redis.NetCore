// <copyright file="RedisClient.Raw.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IDistributedCache
    {
        private static readonly Task<byte[][]> MultipleEmptyResultTask = Task.FromResult(new byte[0][]);
        private static readonly Task<byte[][]> MultipleNullResultTask = Task.FromResult(null as byte[][]);
        private static readonly Task<byte[]> NullResultTask = Task.FromResult(null as byte[]);
        private static readonly Task<byte[]> EmptyResultTask = Task.FromResult(new byte[0]);

        public Task SetAsync(string key, byte[] data)
        {
            CheckKey(key);

            return SendCommandAsync(RedisCommands.Set, key.ToBytes(), data);
        }

        public Task SetAsync(IDictionary<string, byte[]> dictionary)
        {
            var data = new List<byte[]>();
            foreach (var keyValue in dictionary)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value);
            }

            var request = ComposeRequest(RedisCommands.MultipleSet, data);
            return SendMultipleCommandAsync(request);
        }

        public async Task<bool> SetNotExistsAsync(IDictionary<string, byte[]> dictionary)
        {
            var data = new List<byte[]>();
            foreach (var keyValue in dictionary)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value);
            }

            var request = ComposeRequest(RedisCommands.MultipleSetNotExists, data);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0][0] == '1';
        }

        public Task SetAsync(string key, byte[] data, TimeSpan expiration)
        {
            CheckKey(key);

            var milliseconds = (long)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.PrecisionExpiration, expirationBytes);
        }

        public Task SetAsync(string key, byte[] data, int seconds)
        {
            CheckKey(key);

            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.Expiration, expirationBytes);
        }

        public async Task<bool> SetExistsAsync(string key, byte[] data)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.SetExists).ConfigureAwait(false);
            return bytes != null;
        }

        public async Task<bool> SetExistsAsync(string key, byte[] data, TimeSpan expiration)
        {
            CheckKey(key);

            var milliseconds = (long)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.SetExists, RedisCommands.PrecisionExpiration, expirationBytes).ConfigureAwait(false);
            return bytes != null;
        }

        public async Task<bool> SetExistsAsync(string key, byte[] data, int seconds)
        {
            CheckKey(key);

            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.SetExists, RedisCommands.Expiration, expirationBytes).ConfigureAwait(false);
            return bytes != null;
        }

        public async Task<bool> SetNotExistsAsync(string key, byte[] data)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.SetNotExists).ConfigureAwait(false);
            return bytes != null;
        }

        public async Task<bool> SetNotExistsAsync(string key, byte[] data, TimeSpan expiration)
        {
            CheckKey(key);

            var milliseconds = (long)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.SetNotExists, RedisCommands.PrecisionExpiration, expirationBytes).ConfigureAwait(false);
            return bytes != null;
        }

        public async Task<bool> SetNotExistsAsync(string key, byte[] data, int seconds)
        {
            CheckKey(key);

            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.Set, key.ToBytes(), data, RedisCommands.SetNotExists, RedisCommands.Expiration, expirationBytes).ConfigureAwait(false);
            return bytes != null;
        }

        public async Task<int> SetRangeAsync(string key, int offset, byte[] data)
        {
            CheckKey(key);

            var offsetBytes = offset.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.SetRange, key.ToBytes(), offsetBytes, data).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public Task<byte[][]> GetAsync(params string[] keys)
        {
            if (keys == null)
            {
                return MultipleNullResultTask;
            }

            if (keys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.MultipleGet, keys);
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[]> GetAsync(string key)
        {
            if (key == null)
            {
                return NullResultTask;
            }

            if (key.Length == 0)
            {
                return EmptyResultTask;
            }

            return SendCommandAsync(RedisCommands.Get, key.ToBytes());
        }

        public Task<byte[]> GetRangeAsync(string key, int begin, int end = -1)
        {
            CheckKey(key);

            var beginBytes = begin.ToString(CultureInfo.InvariantCulture).ToBytes();
            var endBytes = end.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.GetRange, key.ToBytes(), beginBytes, endBytes);
        }

        public Task<byte[]> GetSetAsync(string key, byte[] data)
        {
            CheckKey(key);

            return SendCommandAsync(RedisCommands.GetSet, key.ToBytes(), data);
        }
    }
}