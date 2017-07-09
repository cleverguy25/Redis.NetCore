﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public partial class RedisClient : IRedisSortedSetStringClient
    {
        public Task<int> SortedSetAddMembersStringAsync(string setKey, params(string member, int score)[] items)
        {
            return SortedSetAddMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int> SortedSetAddOnlyMembersStringAsync(string setKey, params (string member, int score)[] items)
        {
            return SortedSetAddOnlyMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int> SortedSetUpsertMembersStringAsync(string setKey, params (string member, int score)[] items)
        {
            return SortedSetUpsertMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int> SortedSetUpdateMembersStringAsync(string setKey, params (string member, int score)[] items)
        {
            return SortedSetUpdateMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int?> SortedSetGetScoreStringAsync(string setKey, string member)
        {
            return SortedSetGetScoreAsync(setKey, member.ToBytes());
        }

        public Task<int> SortedSetIncrementByStringAsync(string setKey, string member, int increment)
        {
            return SortedSetIncrementByAsync(setKey, member.ToBytes(), increment);
        }

        public async Task<string[]> SortedSetGetRangeStringAsync(string setKey, int start, int end)
        {
            var bytes = await SortedSetGetRangeAsync(setKey, start, end).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetRangeWithScoresStringAsync(string setKey, int start, int end)
        {
            var tuples = await SortedSetGetRangeWithScoresAsync(setKey, start, end).ConfigureAwait(false);
            return tuples.Select(item => (ConvertBytesToString(item.Member), item.Weight)).ToArray();
        }

        public async Task<string[]> SortedSetGetReverseRangeStringAsync(string setKey, int start, int end)
        {
            var bytes = await SortedSetGetReverseRangeAsync(setKey, start, end).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetReverseRangeWithScoresStringAsync(string setKey, int start, int end)
        {
            var tuples = await SortedSetGetReverseRangeWithScoresAsync(setKey, start, end).ConfigureAwait(false);
            return tuples.Select(item => (ConvertBytesToString(item.Member), item.Weight)).ToArray();
        }

        public async Task<int> SortedSetGetLexCountAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetLexCount, setKey.ToBytes(), min.ToBytes(), max.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<string[]> SortedSetGetLexRangeAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<string[]> SortedSetGetLexRangeAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<string[]> SortedSetGetReverseLexRangeAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<string[]> SortedSetGetReverseLexRangeAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<string[]> SortedSetGetRangeByScoreStringAsync(string setKey, string min, string max)
        {
            var bytes = await SortedSetGetRangeByScoreAsync(setKey, min, max).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<string[]> SortedSetGetRangeByScoreStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var bytes = await SortedSetGetRangeByScoreAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetRangeByScoreWithScoresStringAsync(string setKey, string min, string max)
        {
            var tuples = await SortedSetGetRangeByScoreWithScoresAsync(setKey, min, max).ConfigureAwait(false);
            return tuples.Select(item => (ConvertBytesToString(item.Member), item.Weight)).ToArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetRangeByScoreWithScoresStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var tuples = await SortedSetGetRangeByScoreWithScoresAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return tuples.Select(item => (ConvertBytesToString(item.Member), item.Weight)).ToArray();
        }

        public async Task<string[]> SortedSetGetReverseRangeByScoreStringAsync(string setKey, string min, string max)
        {
            var bytes = await SortedSetGetReverseRangeByScoreAsync(setKey, min, max).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<string[]> SortedSetGetReverseRangeByScoreStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var bytes = await SortedSetGetReverseRangeByScoreAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetReverseRangeByScoreWithScoresStringAsync(string setKey, string min, string max)
        {
            var tuples = await SortedSetGetReverseRangeByScoreWithScoresAsync(setKey, min, max).ConfigureAwait(false);
            return tuples.Select(item => (ConvertBytesToString(item.Member), item.Weight)).ToArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetReverseRangeByScoreWithScoresStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var tuples = await SortedSetGetReverseRangeByScoreWithScoresAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return tuples.Select(item => (ConvertBytesToString(item.Member), item.Weight)).ToArray();
        }
    }
}