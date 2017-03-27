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
        public async Task<int> IncrementAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Increment, key.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> IncrementAsync(string key, int amount)
        {
            CheckKey(key);

            var amountBytes = amount.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.IncrementBy, key.ToBytes(), amountBytes).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> DecrementAsync(string key)
        {
            CheckKey(key);

            var bytes = await SendCommandAsync(RedisCommands.Decrement, key.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> DecrementAsync(string key, int amount)
        {
            CheckKey(key);

            var amountBytes = amount.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.DecrementBy, key.ToBytes(), amountBytes).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<float> IncrementAsync(string key, float amount)
        {
            CheckKey(key);

            var amountBytes = amount.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.IncrementByFloat, key.ToBytes(), amountBytes).ConfigureAwait(false);
            var stringValue = Encoding.UTF8.GetString(bytes);
            float value;
            if (float.TryParse(stringValue, out value))
            {
                return value;
            }

            throw new RedisException($"Could not parse [{stringValue}] for key [{key}]");
        }
    }
}