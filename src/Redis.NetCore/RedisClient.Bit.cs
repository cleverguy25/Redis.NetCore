using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
        public async Task<int> GetBitCountAsync(string key, int begin = 0, int end = -1)
        {
			CheckKey(key);

            var beginBytes = begin.ToString(CultureInfo.InvariantCulture).ToBytes();
            var endBytes = end.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.BitCount, key.ToBytes(), beginBytes, endBytes);
            return ConvertBytesToInteger(bytes);
        }

        public Task<int> PerformBitwiseAndAsync(string destinationKey, params string[] sourceKeys)
        {
            return PerformOperation(RedisCommands.And, destinationKey, sourceKeys);
        }

        public Task<int> PerformBitwiseOrAsync(string destinationKey, params string[] sourceKeys)
        {
            return PerformOperation(RedisCommands.Or, destinationKey, sourceKeys);
        }

        public Task<int> PerformBitwiseXorAsync(string destinationKey, params string[] sourceKeys)
        {
            return PerformOperation(RedisCommands.Xor, destinationKey, sourceKeys);
        }

        public Task<int> PerformBitwiseNotAsync(string destinationKey, string sourceKey)
        {
            return PerformOperation(RedisCommands.Not, destinationKey, sourceKey);
        }

        private async Task<int> PerformOperation(string operation, string destinationKey, params string[] sourceKeys)
        {
            var keys = new List<string> { operation, destinationKey };
            keys.AddRange(sourceKeys);
            var request = ComposeRequest(RedisCommands.BitOperation, keys);
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertBytesToInteger(bytes[0]);
        }
    }
}
