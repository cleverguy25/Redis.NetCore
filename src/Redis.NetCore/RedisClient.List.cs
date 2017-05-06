using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisListClient
    {
        public async Task<int> ListPushAsync(string listKey, params byte[][] values)
        {
            CheckListKey(listKey);

            var request = ComposeRequest(RedisCommands.ListPush, listKey.ToBytes(), values);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> ListPushIfExistsAsync(string listKey, byte[] value)
        {
            CheckListKey(listKey);

            var bytes = await SendCommandAsync(RedisCommands.ListPushIfExists, listKey.ToBytes(), value).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> ListTailPushAsync(string listKey, params byte[][] values)
        {
            CheckListKey(listKey);

            var request = ComposeRequest(RedisCommands.ListTailPush, listKey.ToBytes(), values);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> ListTailPushIfExistsAsync(string listKey, byte[] value)
        {
            CheckListKey(listKey);

            var bytes = await SendCommandAsync(RedisCommands.ListTailPushIfExists, listKey.ToBytes(), value).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public Task<byte[]> ListPopAsync(string listKey)
        {
            CheckListKey(listKey);

            return SendCommandAsync(RedisCommands.ListPop, listKey.ToBytes());
        }

        public async Task<Tuple<string, byte[]>> ListBlockingPopAsync(int timeoutSeconds, params string[] listKeys)
        {
            CheckListKeys(listKeys);

            var timeoutString = new[] { timeoutSeconds.ToString(CultureInfo.InvariantCulture) };
            var request = ComposeRequest(RedisCommands.BlockingListPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            if (bytes.Length == 0)
            {
                return null;
            }

            var foundListKey = ConvertBytesToString(bytes[0]);
            return Tuple.Create(foundListKey, bytes[1]);
        }

        public Task<byte[]> ListTailPopAsync(string listKey)
        {
            CheckListKey(listKey);

            return SendCommandAsync(RedisCommands.ListTailPop, listKey.ToBytes());
        }

        public async Task<Tuple<string, byte[]>> ListBlockingTailPopAsync(int timeoutSeconds, params string[] listKeys)
        {
            CheckListKeys(listKeys);

            var timeoutString = new[] { timeoutSeconds.ToString(CultureInfo.InvariantCulture) };
            var request = ComposeRequest(RedisCommands.BlockingListTailPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            if (bytes.Length == 0)
            {
                return null;
            }

            var foundListKey = ConvertBytesToString(bytes[0]);
            return Tuple.Create(foundListKey, bytes[1]);
        }

        private static void CheckListKeys(string[] listKeys)
        {
            if (listKeys == null)
            {
                throw new ArgumentNullException(nameof(listKeys));
            }

            if (listKeys.Length == 0)
            {
                throw new ArgumentException(nameof(listKeys), $"{nameof(listKeys)} length is zero.");
            }
        }

        private static void CheckListKey(string listKey)
        {
            if (string.IsNullOrEmpty(listKey))
            {
                throw new ArgumentNullException(nameof(listKey), "listKey cannot be null or empty");
            }
        }
    }
}