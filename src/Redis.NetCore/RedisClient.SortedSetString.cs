using System;
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

        public Task<int?> SortedSetGetRankStringAsync(string setKey, string member)
        {
            return SortedSetGetRankAsync(setKey, member.ToBytes());
        }

        public Task<int?> SortedSetGetReverseRankStringAsync(string setKey, string member)
        {
            return SortedSetGetReverseRankAsync(setKey, member.ToBytes());
        }

        public Task<int> SortedSetIncrementByStringAsync(string setKey, string member, int increment)
        {
            return SortedSetIncrementByAsync(setKey, member.ToBytes(), increment);
        }

        public async Task<string[]> SortedSetGetRangeStringAsync(string setKey, int start, int end)
        {
            var bytes = await SortedSetGetRangeAsync(setKey, start, end).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetRangeWithScoresStringAsync(string setKey, int start, int end)
        {
            var tuples = await SortedSetGetRangeWithScoresAsync(setKey, start, end).ConfigureAwait(false);
            return tuples.Select(item => (item.Member.ConvertBytesToString(), item.Weight)).ToArray();
        }

        public async Task<string[]> SortedSetGetReverseRangeStringAsync(string setKey, int start, int end)
        {
            var bytes = await SortedSetGetReverseRangeAsync(setKey, start, end).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetReverseRangeWithScoresStringAsync(string setKey, int start, int end)
        {
            var tuples = await SortedSetGetReverseRangeWithScoresAsync(setKey, start, end).ConfigureAwait(false);
            return tuples.Select(item => (item.Member.ConvertBytesToString(), item.Weight)).ToArray();
        }

        public async Task<int> SortedSetGetLexCountAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SortedSetLexCount, setKey.ToBytes(), min.ToBytes(), max.ToBytes()).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<string[]> SortedSetGetLexRangeAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SortedSetGetLexRangeAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SortedSetGetReverseLexRangeAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SortedSetGetReverseLexRangeAsync(string setKey, string min, string max, int offset, int count)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetReverseRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes(), RedisCommands.Limit, offset.ToBytes(), count.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SortedSetGetRangeByScoreStringAsync(string setKey, string min, string max)
        {
            var bytes = await SortedSetGetRangeByScoreAsync(setKey, min, max).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SortedSetGetRangeByScoreStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var bytes = await SortedSetGetRangeByScoreAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetRangeByScoreWithScoresStringAsync(string setKey, string min, string max)
        {
            var tuples = await SortedSetGetRangeByScoreWithScoresAsync(setKey, min, max).ConfigureAwait(false);
            return tuples.Select(item => (item.Member.ConvertBytesToString(), item.Weight)).ToArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetRangeByScoreWithScoresStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var tuples = await SortedSetGetRangeByScoreWithScoresAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return tuples.Select(item => (item.Member.ConvertBytesToString(), item.Weight)).ToArray();
        }

        public async Task<string[]> SortedSetGetReverseRangeByScoreStringAsync(string setKey, string min, string max)
        {
            var bytes = await SortedSetGetReverseRangeByScoreAsync(setKey, min, max).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SortedSetGetReverseRangeByScoreStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var bytes = await SortedSetGetReverseRangeByScoreAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetReverseRangeByScoreWithScoresStringAsync(string setKey, string min, string max)
        {
            var tuples = await SortedSetGetReverseRangeByScoreWithScoresAsync(setKey, min, max).ConfigureAwait(false);
            return tuples.Select(item => (item.Member.ConvertBytesToString(), item.Weight)).ToArray();
        }

        public async Task<(string Member, int Weight)[]> SortedSetGetReverseRangeByScoreWithScoresStringAsync(string setKey, string min, string max, int offset, int count)
        {
            var tuples = await SortedSetGetReverseRangeByScoreWithScoresAsync(setKey, min, max, offset, count).ConfigureAwait(false);
            return tuples.Select(item => (item.Member.ConvertBytesToString(), item.Weight)).ToArray();
        }

        public Task<int> SortedSetRemoveMembersStringAsync(string setKey, params string[] members)
        {
            return SortedSetRemoveMembersAsync(setKey, members.ToBytes());
        }

        public async Task<int> SortedSetRemoveRangeByLexAsync(string setKey, string min, string max)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SortedSetRemoveRangeByLex, setKey.ToBytes(), min.ToBytes(), max.ToBytes());
            var bytes = await SendCommandAsync(request).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey)
        {
            CheckKey(hashKey);

            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), ZeroBit).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, ScanCursor cursor)
        {
            CheckKey(hashKey);

            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), cursorPositionBytes).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), ZeroBit, "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, ScanCursor cursor, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToBytes();
            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), cursorPositionBytes, "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, string match)
        {
            CheckKey(hashKey);

            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), ZeroBit, "MATCH".ToBytes(), match.ToBytes()).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, ScanCursor cursor, string match)
        {
            CheckKey(hashKey);

            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), cursorPositionBytes, "MATCH".ToBytes(), match.ToBytes()).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, string match, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), ZeroBit, "MATCH".ToBytes(), match.ToBytes(), "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        public async Task<SortedSetScanCursor> SortedSetScanAsync(string hashKey, ScanCursor cursor, string match, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToBytes();
            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SortedSetScan, hashKey.ToBytes(), cursorPositionBytes, "MATCH".ToBytes(), match.ToBytes(), "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToSortedSetScanCursor(bytes);
        }

        private static SortedSetScanCursor ConvertToSortedSetScanCursor(byte[][] bytes)
        {
            var cursorPosition = bytes[0].ConvertBytesToInteger();
            return new SortedSetScanCursor(cursorPosition, bytes[1]);
        }
    }
}