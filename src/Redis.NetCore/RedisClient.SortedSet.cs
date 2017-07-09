using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
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

        public async Task<int?> SortedSetGetRankAsync(string setKey, byte[] member)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetRank, setKey.ToBytes(), member).ConfigureAwait(false);
            return bytes == null ? (int?)null : ConvertBytesToInteger(bytes);
        }

        public async Task<int?> SortedSetGetReverseRankAsync(string setKey, byte[] member)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetReverseRank, setKey.ToBytes(), member).ConfigureAwait(false);
            return bytes == null ? (int?)null : ConvertBytesToInteger(bytes);
        }

        public async Task<int> SortedSetCardinalityAsync(string setKey)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetCardinality, setKey.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> SortedSetGetCountAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetCount, setKey.ToBytes(), min.ToBytes(), max.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> SortedSetIncrementByAsync(string setKey, byte[] member, int increment)
        {
            CheckSetKey(setKey);

            var incrementBytes = increment.ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.SortedSetIncrementBy, setKey.ToBytes(), incrementBytes, member).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public Task<int> SortedSetStoreIntersectionMembersAsync(string storeKey, string[] sets, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return SortedSetStoreCommandAsync(storeKey, sets, aggregate, RedisCommands.SortedSetIntersectionStore);
        }

        public Task<int> SortedSetStoreIntersectionMembersAsync(string storeKey, (string set, int weight)[] sets, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return SortedSetStoreCommandAsync(storeKey, sets, aggregate, RedisCommands.SortedSetIntersectionStore);
        }

        public Task<int> SortedSetStoreUnionMembersAsync(string storeKey, string[] sets, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return SortedSetStoreCommandAsync(storeKey, sets, aggregate, RedisCommands.SortedSetUnionStore);
        }

        public Task<int> SortedSetStoreUnionMembersAsync(string storeKey, (string set, int weight)[] sets, RedisAggregate aggregate = RedisAggregate.Sum)
        {
            return SortedSetStoreCommandAsync(storeKey, sets, aggregate, RedisCommands.SortedSetUnionStore);
        }

        public Task<byte[][]> SortedSetGetRangeAsync(string setKey, int start, int end)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRange, setKey.ToBytes(), start.ToBytes(), end.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public async Task<(byte[] Member, int Weight)[]> SortedSetGetRangeWithScoresAsync(string setKey, int start, int end)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRange, setKey.ToBytes(), start.ToBytes(), end.ToBytes(), RedisCommands.WithScores);
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertToSortedSetTuple(bytes);
        }

        public Task<byte[][]> SortedSetGetReverseRangeAsync(string setKey, int start, int end)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRange, setKey.ToBytes(), start.ToBytes(), end.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public async Task<(byte[] Member, int Weight)[]> SortedSetGetReverseRangeWithScoresAsync(string setKey, int start, int end)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRange, setKey.ToBytes(), start.ToBytes(), end.ToBytes(), RedisCommands.WithScores);
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertToSortedSetTuple(bytes);
        }

        public Task<byte[][]> SortedSetGetRangeByScoreAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[][]> SortedSetGetRangeByScoreAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public async Task<(byte[] Member, int Weight)[]> SortedSetGetRangeByScoreWithScoresAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.WithScores);
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertToSortedSetTuple(bytes);
        }

        public async Task<(byte[] Member, int Weight)[]> SortedSetGetRangeByScoreWithScoresAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.WithScores, RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertToSortedSetTuple(bytes);
        }

        public Task<byte[][]> SortedSetGetReverseRangeByScoreAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[][]> SortedSetGetReverseRangeByScoreAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public async Task<(byte[] Member, int Weight)[]> SortedSetGetReverseRangeByScoreWithScoresAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.WithScores);
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertToSortedSetTuple(bytes);
        }

        public async Task<(byte[] Member, int Weight)[]> SortedSetGetReverseRangeByScoreWithScoresAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByScore, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.WithScores, RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            var bytes = await SendMultipleCommandAsync(request);
            return ConvertToSortedSetTuple(bytes);
        }

        private static (byte[] Member, int Weight)[] ConvertToSortedSetTuple(IReadOnlyList<byte[]> bytes)
        {
            var results = new List<(byte[] member, int weight)>();
            for (var i = 0; i < bytes.Count; i += 2)
            {
                var member = bytes[i];
                var weight = ConvertBytesToInteger(bytes[i + 1]);
                results.Add((member, weight));
            }

            return results.ToArray();
        }

        private static byte[][] ConvertTupleItemsToByteArray(IReadOnlyList<(byte[] member, int weight)> items)
        {
            var byteArray = new byte[items.Count * 2][];
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var position = i * 2;
                byteArray[position] = item.weight.ToBytes();
                byteArray[position + 1] = item.member;
            }

            return byteArray;
        }

        private static void AddAggregateTypeToRequest(RedisAggregate aggregate, IList<byte[]> request, int index)
        {
            request[index] = RedisCommands.Aggregate;
            switch (aggregate)
            {
                case RedisAggregate.Sum:
                    request[index + 1] = RedisCommands.AggregateSum;
                    break;
                case RedisAggregate.Min:
                    request[index + 1] = RedisCommands.AggregateMin;
                    break;
                case RedisAggregate.Max:
                    request[index + 1] = RedisCommands.AggregateMax;
                    break;
            }
        }

        private async Task<int> SortedSetStoreCommandAsync(string storeKey, string[] sets, RedisAggregate aggregate, byte[] command)
        {
            CheckSetKey(storeKey);

            var countBytes = sets.Length.ToBytes();
            var request = new byte[sets.Length + 5][];
            request[0] = command;
            request[1] = storeKey.ToBytes();
            request[2] = countBytes;
            var index = CopyBytesToRequest(sets.ToBytes(), request, 3);
            AddAggregateTypeToRequest(aggregate, request, index);

            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        private async Task<int> SortedSetStoreCommandAsync(string storeKey, ValueTuple<string, int>[] sets, RedisAggregate aggregate, byte[] command)
        {
            CheckSetKey(storeKey);

            var countBytes = sets.Length.ToBytes();
            var request = new byte[(2 * sets.Length) + 6][];
            request[0] = command;
            request[1] = storeKey.ToBytes();
            request[2] = countBytes;
            var index = CopyBytesToRequest(sets.Select<(string set, int weight), string>(item => item.set).ToBytes(), request, 3);
            request[index] = RedisCommands.Weight;
            index = CopyBytesToRequest(sets.Select<(string set, int weight), byte[]>(item => item.weight.ToBytes()).ToArray(), request, index + 1);
            AddAggregateTypeToRequest(aggregate, request, index);

            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }
    }
}