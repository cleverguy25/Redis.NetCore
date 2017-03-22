using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
        public Task SetStringAsync(string key, string data)
        {
            return SendCommandAsync(RedisCommands.Set, key.ToBytes(), data.ToBytes());
        }

        public Task SetStringAsync(string key, string data, TimeSpan expiration)
        {
            var milliseconds = (int)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.PrecisionSetExpiration, key.ToBytes(), expirationBytes, data.ToBytes());
        }

        public Task SetStringAsync(string key, string data, int seconds)
        {
            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.SetExpiration, key.ToBytes(), expirationBytes, data.ToBytes());
        }

        public async Task<string> GetStringAsync(string key)
        {
            var bytes = await SendCommandAsync(RedisCommands.Get, key.ToBytes()).ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }

        public async Task<string[]> GetStringAsync(params string[] keys)
        {
            var request = ComposeRequest(RedisCommands.MultipleGet, keys);
            var response = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return response.Select(bytes => bytes == null ?
                        null : Encoding.UTF8.GetString(bytes)).ToArray();
        }
    }
}
