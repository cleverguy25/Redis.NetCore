using System;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisSetClient
    {
        public async Task<int> SetAddAsync(string setKey, params byte[][] members)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SetAdd, setKey.ToBytes(), members);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> SetCardinalityAsync(string setKey)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SetCardinality, setKey.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<bool> SetIsMemberAsync(string setKey, byte[] member)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SetIsMember, setKey.ToBytes(), member).ConfigureAwait(false);
            return ConvertBytesToBool(bytes);
        }

        public Task<byte[][]> SetDifferenceAsync(params string[] setKeys)
        {
            if (setKeys == null)
            {
                return MultipleNullResultTask;
            }

            if (setKeys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.SetDifference, setKeys);
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[][]> SetIntersectionAsync(params string[] setKeys)
        {
            if (setKeys == null)
            {
                return MultipleNullResultTask;
            }

            if (setKeys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.SetIntersection, setKeys);
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[][]> SetUnionAsync(params string[] setKeys)
        {
            if (setKeys == null)
            {
                return MultipleNullResultTask;
            }

            if (setKeys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.SetUnion, setKeys);
            return SendMultipleCommandAsync(request);
        }

        private static void CheckSetKey(string setKey)
        {
            if (string.IsNullOrEmpty(setKey))
            {
                throw new ArgumentNullException(nameof(setKey), "setKey cannot be null or empty");
            }
        }
    }
}
