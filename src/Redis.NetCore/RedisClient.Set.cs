// <copyright file="RedisClient.Set.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Globalization;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisSetClient
    {
        public async Task<int> SetAddMemberAsync(string setKey, params byte[][] members)
        {
            CheckSetKey(setKey);

            var request = ComposeRequest(RedisCommands.SetAdd, setKey.ToBytes(), members);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0].ConvertBytesToInteger();
        }

        public async Task<int> SetCardinalityAsync(string setKey)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SetCardinality, setKey.ToBytes()).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<bool> SetIsMemberAsync(string setKey, byte[] member)
        {
            CheckSetKey(setKey);

            var bytes = await SendCommandAsync(RedisCommands.SetIsMember, setKey.ToBytes(), member).ConfigureAwait(false);
            return bytes.ConvertBytesToBool();
        }

        public Task<byte[][]> SetGetDifferenceMembersAsync(params string[] setKeys)
        {
            if (setKeys == null)
            {
                return MultipleNullResultTask;
            }

            if (setKeys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.SetDifference, setKeys);
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[][]> SetGetIntersectionMembersAsync(params string[] setKeys)
        {
            if (setKeys == null)
            {
                return MultipleNullResultTask;
            }

            if (setKeys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.SetIntersection, setKeys);
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[][]> SetGetUnionMembersAsync(params string[] setKeys)
        {
            if (setKeys == null)
            {
                return MultipleNullResultTask;
            }

            if (setKeys.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.SetUnion, setKeys);
            return SendMultipleCommandAsync(request);
        }

        public async Task<int> SetStoreDifferenceMembersAsync(string storeKey, params string[] setKeys)
        {
            CheckSetKey(storeKey);

            var request = ComposeRequest(RedisCommands.SetDifferenceStore, storeKey.ToBytes(), setKeys);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0].ConvertBytesToInteger();
        }

        public async Task<int> SetStoreIntersectionMembersAsync(string storeKey, params string[] setKeys)
        {
            CheckSetKey(storeKey);

            var request = ComposeRequest(RedisCommands.SetIntersectionStore, storeKey.ToBytes(), setKeys);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0].ConvertBytesToInteger();
        }

        public async Task<int> SetStoreUnionMembersAsync(string storeKey, params string[] setKeys)
        {
            CheckSetKey(storeKey);

            var request = ComposeRequest(RedisCommands.SetUnionStore, storeKey.ToBytes(), setKeys);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0].ConvertBytesToInteger();
        }

        public Task<byte[][]> SetGetMembersAsync(string storeKey)
        {
            CheckSetKey(storeKey);

            var request = ComposeRequest(RedisCommands.SetMembers, storeKey.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public async Task<bool> SetMoveMemberAsync(string sourceSet, string destinationSet, byte[] member)
        {
            CheckSetKey(sourceSet);
            CheckSetKey(destinationSet);

            var bytes = await SendCommandAsync(RedisCommands.SetMove, sourceSet.ToBytes(), destinationSet.ToBytes(), member).ConfigureAwait(false);
            return bytes.ConvertBytesToBool();
        }

        public Task<byte[]> SetPopMemberAsync(string storeKey)
        {
            CheckSetKey(storeKey);

            return SendCommandAsync(RedisCommands.SetPop, storeKey.ToBytes());
        }

        public Task<byte[][]> SetGetRandomMemberAsync(string storeKey, int count = 1)
        {
            CheckSetKey(storeKey);

            var countBytes = count.ToBytes();
            return SendMultipleCommandAsync(RedisCommands.SetRandomMember, storeKey.ToBytes(), countBytes);
        }

        public async Task<int> SetRemoveMembersAsync(string storeKey, params byte[][] members)
        {
            CheckSetKey(storeKey);

            var request = ComposeRequest(RedisCommands.SetRemove, storeKey.ToBytes(), members);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0].ConvertBytesToInteger();
        }

        public async Task<ScanCursor> SetScanAsync(string setKey)
        {
            CheckSetKey(setKey);

            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), ZeroBit).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor)
        {
            CheckSetKey(setKey);

            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), cursorPositionBytes).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, int count)
        {
            CheckSetKey(setKey);

            var countBytes = count.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), ZeroBit, "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor, int count)
        {
            CheckSetKey(setKey);

            var countBytes = count.ToBytes();
            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), cursorPositionBytes, "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, string match)
        {
            CheckSetKey(setKey);

            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), ZeroBit, "MATCH".ToBytes(), match.ToBytes()).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor, string match)
        {
            CheckSetKey(setKey);

            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), cursorPositionBytes, "MATCH".ToBytes(), match.ToBytes()).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, string match, int count)
        {
            CheckSetKey(setKey);

            var countBytes = count.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), ZeroBit, "MATCH".ToBytes(), match.ToBytes(), "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
        }

        public async Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor, string match, int count)
        {
            CheckSetKey(setKey);

            var countBytes = count.ToBytes();
            var cursorPositionBytes = cursor.CursorPosition.ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.SetScan, setKey.ToBytes(), cursorPositionBytes, "MATCH".ToBytes(), match.ToBytes(), "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToScanCursor(bytes);
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
