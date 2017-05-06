using Redis.NetCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public async Task<int> ListPushIfExistsAsync(string listKey, byte[] value)
        {
            var bytes = await SendCommandAsync(RedisCommands.ListPushIfExists, listKey.ToBytes(), value).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> ListTailPushAsync(string listKey, params byte[][] values)
        {
            var request = ComposeRequest(RedisCommands.ListTailPush, listKey.ToBytes(), values);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> ListTailPushIfExistsAsync(string listKey, byte[] value)
        {
            var bytes = await SendCommandAsync(RedisCommands.ListTailPushIfExists, listKey.ToBytes(), value).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public Task<byte[]> ListPopAsync(string listKey)
        {
            return SendCommandAsync(RedisCommands.ListPop, listKey.ToBytes());
        }

        public async Task<Tuple<string, byte[]>> ListBlockingPopAsync(int timeoutSeconds, params string[] listKeys)
        {
            var timeoutString = new[] { timeoutSeconds.ToString(CultureInfo.InvariantCulture) };
            var request = ComposeRequest(RedisCommands.BlockingListPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            var foundListKey = ConvertBytesToString(bytes[0]);
            return Tuple.Create(foundListKey, bytes[1]);
        }

        public Task<byte[]> ListTailPopAsync(string listKey)
        {
            return SendCommandAsync(RedisCommands.ListTailPop, listKey.ToBytes());
        }

        public async Task<Tuple<string, byte[]>> ListBlockingTailPopAsync(int timeoutSeconds, params string[] listKeys)
        {
            var timeoutString = new[] { timeoutSeconds.ToString(CultureInfo.InvariantCulture) };
            var request = ComposeRequest(RedisCommands.BlockingListTailPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            var foundListKey = ConvertBytesToString(bytes[0]);
            return Tuple.Create(foundListKey, bytes[1]);
        }
    }
}
