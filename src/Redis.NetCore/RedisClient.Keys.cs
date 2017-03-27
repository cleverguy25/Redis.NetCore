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
        public async Task<string[]> GetKeysAsync(string pattern)
        {
            CheckKey(pattern);

            var bytes = await SendMultipleCommandAsync(RedisCommands.Keys, pattern.ToBytes());
            return bytes.Select(item => Encoding.UTF8.GetString(item)).ToArray();
        }

        public async Task<int> GetTimeToLive(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.TimeToLive, key.ToBytes()).ConfigureAwait(false);
            var timeToLiveString = Encoding.UTF8.GetString(bytes);
            int timeToLive;
            if (int.TryParse(timeToLiveString, out timeToLive) == false)
            {
                throw new FormatException($"Cannot parse time to live [{timeToLiveString}]");
            }

            return timeToLive;
        }

        public async Task<long> GetPreciseTimeToLive(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.PrecisionTimeToLive, key.ToBytes()).ConfigureAwait(false);
            var timeToLiveString = Encoding.UTF8.GetString(bytes);
            long timeToLive;
            if (long.TryParse(timeToLiveString, out timeToLive) == false)
            {
                throw new FormatException($"Cannot parse time to live [{timeToLiveString}]");
            }

            return timeToLive;
        }

        public Task DeleteKeyAsync(params string[] keys)
        {
            if (keys == null)
            {
                return MultipleNullResultTask;
            }

            if (keys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.Delete, keys);
            return SendMultipleCommandAsync(request);
        }

        public async Task<bool> MoveAsync(string key, int databaseIndex)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Move, key.ToBytes(), databaseIndex.ToString().ToBytes()).ConfigureAwait(false);
            return bytes[0] == '1';
        }

        public async Task<bool> ExistsAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Exists, key.ToBytes()).ConfigureAwait(false);
            return bytes[0] == '1';
        }

        public async Task<int> ExistsAsync(params string[] keys)
        {
            if (keys == null)
            {
                return 0;
            }

            if (keys.Length == 0)
            {
                return 0;
            }

            var request = ComposeRequest(RedisCommands.Exists, keys);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
        {
            CheckKey(key);

            var milliseconds = (long)expiration.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.PrecisionExpire, key.ToBytes(), expirationBytes);
            return bytes[0] == '1';
        }

        public async Task<bool> SetExpirationAsync(string key, int seconds)
        {
            CheckKey(key);

            var expirationBytes = seconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.Expire, key.ToBytes(), expirationBytes);
            return bytes[0] == '1';
        }

        public async Task<bool> SetExpirationAsync(string key, DateTime dateTime)
        {
            CheckKey(key);

            var epoch = new DateTime(1970, 1, 1);
            var timeSpan = dateTime - epoch;
            var milliseconds = (long)timeSpan.TotalMilliseconds;
            var expirationBytes = milliseconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.PrecisionExpireAt, key.ToBytes(), expirationBytes);
            return bytes[0] == '1';
        }

        public async Task<bool> PersistAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Persist, key.ToBytes()).ConfigureAwait(false);
            return bytes[0] == '1';
        }

        public Task RenameKeyAsync(string key, string newKey)
        {
            CheckKey(key);
            CheckKey(newKey);

            return SendCommandAsync(RedisCommands.Rename, key.ToBytes(), newKey.ToBytes());
        }

        public async Task<bool> RenameKeyNotExistsAsync(string key, string newKey)
        {
            CheckKey(key);
            CheckKey(newKey);

            var bytes = await SendCommandAsync(RedisCommands.RenameNotExists, key.ToBytes(), newKey.ToBytes());
            return bytes[0] == '1';
        }

        private static int ConvertBytesToInteger(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate(
                0, 
                (current, currentByte) => current * 10 + currentByte - RedisProtocolContants.Zero);
        }
    }
}
