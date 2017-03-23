using System;
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
            CheckKey(key);

            return SendCommandAsync(RedisCommands.Set, key.ToBytes(), data.ToBytes());
        }

        public Task SetStringAsync(string key, string data, TimeSpan expiration)
        {
            CheckKey(key);

            var milliseconds = (long)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.PrecisionSetExpiration, key.ToBytes(), expirationBytes, data.ToBytes());
        }

        public Task SetStringAsync(string key, string data, int seconds)
        {
            CheckKey(key);

            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.SetExpiration, key.ToBytes(), expirationBytes, data.ToBytes());
        }

        public async Task<string> GetStringAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var bytes = await SendCommandAsync(RedisCommands.Get, key.ToBytes()).ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }

        public async Task<string[]> GetStringAsync(params string[] keys)
        {
            if (keys == null)
            {
                return null;
            }

            if (keys.Length == 0)
            {
                return new string[0];
            }

            var request = ComposeRequest(RedisCommands.MultipleGet, keys);
            var response = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return response.Select(
                                   bytes => bytes == null ?
                                       null : Encoding.UTF8.GetString(bytes)).ToArray();
        }

        private static void CheckKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Key cannot be null or empty", nameof(key));
            }
        }
    }
}