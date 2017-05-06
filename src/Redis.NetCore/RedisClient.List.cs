using Redis.NetCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisListClient
    {
        public async Task<int> ListPushAsync(string listKey, params byte[][] values)
        {
            var request = ComposeRequest(RedisCommands.ListPush, listKey.ToBytes(), values);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> ListTailPushAsync(string listKey, params byte[][] values)
        {
            var request = ComposeRequest(RedisCommands.ListTailPush, listKey.ToBytes(), values);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public Task<byte[]> ListPopAsync(string listKey)
        {
            return SendCommandAsync(RedisCommands.ListPop, listKey.ToBytes());
        }

        public Task<byte[]> ListTailPopAsync(string listKey)
        {
            return SendCommandAsync(RedisCommands.ListTailPop, listKey.ToBytes());
        }
    }
}
