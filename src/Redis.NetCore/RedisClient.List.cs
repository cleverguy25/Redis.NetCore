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
            var request = ComposeRequest(RedisCommands.ListBlockingPop, listKeys.Union(timeoutString));
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
            var request = ComposeRequest(RedisCommands.ListBlockingTailPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            if (bytes.Length == 0)
            {
                return null;
            }

            var foundListKey = ConvertBytesToString(bytes[0]);
            return Tuple.Create(foundListKey, bytes[1]);
        }

        public Task<byte[]> ListTailPopAndPushAsync(string listKey1, string listKey2)
        {
            CheckListKey(listKey1);
            CheckListKey(listKey2);

            return SendCommandAsync(RedisCommands.ListTailPopAndPush, listKey1.ToBytes(), listKey2.ToBytes());
        }

        public Task<byte[]> ListBlockingTailPopAndPushAsync(string listKey1, string listKey2, int timeoutSeconds)
        {
            CheckListKey(listKey1);
            CheckListKey(listKey2);

            var timeoutBytes = timeoutSeconds.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.ListBlockingTailPopAndPush, listKey1.ToBytes(), listKey2.ToBytes(), timeoutBytes);
        }

        public Task<byte[]> ListIndexAsync(string listKey, int index)
        {
            CheckListKey(listKey);

            var indexBytes = index.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.ListIndex, listKey.ToBytes(), indexBytes);
        }

        public async Task<int> ListInsertBeforeAsync(string listKey, byte[] pivot, byte[] value)
        {
            CheckListKey(listKey);

            var bytes = await SendCommandAsync(RedisCommands.ListInsert, listKey.ToBytes(), RedisCommands.Before, pivot, value)
                .ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> ListInsertAfterAsync(string listKey, byte[] pivot, byte[] value)
        {
            CheckListKey(listKey);

            var bytes = await SendCommandAsync(RedisCommands.ListInsert, listKey.ToBytes(), RedisCommands.After, pivot, value).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> ListLength(string listKey)
        {
            CheckListKey(listKey);

            var bytes = await SendCommandAsync(RedisCommands.ListLength, listKey.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<byte[][]> ListRangeAsync(string listKey, int start, int end)
        {
            CheckListKey(listKey);

            var startBytes = start.ToString(CultureInfo.InvariantCulture).ToBytes();
            var endBytes = end.ToString(CultureInfo.InvariantCulture).ToBytes();
            var messageBytes = new[] { listKey.ToBytes(), startBytes, endBytes };
            var request = ComposeRequest(RedisCommands.ListRange, messageBytes);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes;
        }

        public async Task<int> ListRemoveAsync(string listKey, int count, byte[] value)
        {
            CheckListKey(listKey);

            var countBytes = count.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.ListRemove, listKey.ToBytes(), countBytes, value).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public Task ListSetAsync(string listKey, int index, byte[] value)
        {
            CheckListKey(listKey);

            var indexBytes = index.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.ListSet, listKey.ToBytes(), indexBytes, value);
        }

        public Task ListTrimAsync(string listKey, int start, int end)
        {
            CheckListKey(listKey);

            var startBytes = start.ToString(CultureInfo.InvariantCulture).ToBytes();
            var endBytes = end.ToString(CultureInfo.InvariantCulture).ToBytes();
            return SendCommandAsync(RedisCommands.ListTrim, listKey.ToBytes(), startBytes, endBytes);
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