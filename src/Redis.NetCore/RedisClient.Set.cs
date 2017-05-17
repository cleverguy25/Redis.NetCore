using System;
using System.Collections.Generic;
using System.Text;
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

            var bytes = await SendCommandAsync(RedisCommands.SetCardinality, setKey.ToBytes());
            return ConvertBytesToInteger(bytes);
        }

        public async Task<bool> SetIsMemberAsync(string setKey, byte[] member)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SetIsMember, setKey.ToBytes(), member);
            return ConvertBytesToBool(bytes);
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
