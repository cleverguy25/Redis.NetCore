using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisHyperLogLogClient
    {
        public async Task<bool> HyperLogLogAddAsync(string hyperKey, params byte[][] elements)
        {
            var request = ComposeRequest(RedisCommands.HyperLogLogAdd, hyperKey.ToBytes(), elements);
            var bytes = await SendCommandAsync(request);
            return bytes.ConvertBytesToBool();
        }

        public async Task<bool> HyperLogLogAddStringAsync(string hyperKey, params string[] elements)
        {
            var request = ComposeRequest(RedisCommands.HyperLogLogAdd, hyperKey.ToBytes(), elements);
            var bytes = await SendCommandAsync(request);
            return bytes.ConvertBytesToBool();
        }

        public async Task<int> HyperLogLogCountAsync(params string[] keys)
        {
            var request = ComposeRequest(RedisCommands.HyperLogLogCount, keys);
            var bytes = await SendCommandAsync(request);
            return bytes.ConvertBytesToInteger();
        }

        public Task HyperLogLogMergeAsync(string storeKey, params string[] keys)
        {
            var request = ComposeRequest(RedisCommands.HyperLogLogMerge, storeKey.ToBytes(), keys);
            return SendCommandAsync(request);
        }
    }
}