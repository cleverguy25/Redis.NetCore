using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

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

        public Task SetAsync(string key, byte[] data, TimeSpan expiration)
        {
            CheckKey(key);

            var milliseconds = (long)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.PrecisionSetExpiration, key.ToBytes(), expirationBytes, data);
        }

        public Task SetAsync(string key, byte[] data, int seconds)
        {
            CheckKey(key);

            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.SetExpiration, key.ToBytes(), expirationBytes, data);
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
    }
}
