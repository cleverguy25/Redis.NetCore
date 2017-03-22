using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
        public Task SetAsync(string key, byte[] data)
        {
            return SendCommandAsync(RedisCommands.Set, key.ToBytes(), data);
        }

        public Task SetAsync(string key, byte[] data, TimeSpan expiration)
        {
            var milliseconds = (int)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.PrecisionSetExpiration, key.ToBytes(), expirationBytes, data);
        }

        public Task SetAsync(string key, byte[] data, int seconds)
        {
            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.SetExpiration, key.ToBytes(), expirationBytes, data);
        }

        public Task<byte[]> GetAsync(string key)
        {
            return SendCommandAsync(RedisCommands.Get, key.ToBytes());
        }

        public Task<byte[][]> GetAsync(params string[] keys)
        {
            var request = ComposeRequest(RedisCommands.MultipleGet, keys);
            return SendMultipleCommandAsync(request);
        }
    }
}
