using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public partial class RedisClient : IRedisSortedSetClient
    {
        public async Task<int> SortedSetAddMembersAsync(string setKey, params(byte[] member, int score)[] items)
        {
            CheckSetKey(setKey);

            var byteArray = ConvertTupleItemsToByteArray(items);

            var request = ComposeRequest(RedisCommands.SortedSetAdd, setKey.ToBytes(), byteArray);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> SortedSetAddOnlyMembersAsync(string setKey, params (byte[] member, int score)[] items)
        {
            CheckSetKey(setKey);

            var byteArray = ConvertTupleItemsToByteArray(items);

            var request = ComposeRequest(RedisCommands.SortedSetAdd, setKey.ToBytes(), RedisCommands.SetNotExists, byteArray);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> SortedSetUpsertMembersAsync(string setKey, params (byte[] member, int score)[] items)
        {
            CheckSetKey(setKey);

            var byteArray = ConvertTupleItemsToByteArray(items);

            var request = ComposeRequest(RedisCommands.SortedSetAdd, setKey.ToBytes(), RedisCommands.SetChanged, byteArray);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int> SortedSetUpdateMembersAsync(string setKey, params (byte[] member, int score)[] items)
        {
            CheckSetKey(setKey);

            var byteArray = ConvertTupleItemsToByteArray(items);

            var request = ComposeRequest(RedisCommands.SortedSetAdd, setKey.ToBytes(), RedisCommands.SetExists, byteArray);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<int?> SortedSetGetScoreAsync(string setKey, byte[] member)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetScore, setKey.ToBytes(), member).ConfigureAwait(false);
            return bytes == null ? (int?)null : ConvertBytesToInteger(bytes);
        }

        public async Task<int> SortedSetCardinalityAsync(string setKey)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetCardinality, setKey.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        private static byte[][] ConvertTupleItemsToByteArray(ValueTuple<byte[], int>[] items)
        {
            var byteArray = new byte[items.Length * 2][];
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var position = i * 2;
                byteArray[position] = item.Item2.ToBytes();
                byteArray[position + 1] = item.Item1;
            }

            return byteArray;
        }
    }
}
