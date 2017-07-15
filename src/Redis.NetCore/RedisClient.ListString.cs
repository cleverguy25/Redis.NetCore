// <copyright file="RedisClient.ListString.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using Redis.NetCore.Abstractions;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisListStringClient
    {
        public Task<int> ListPushStringAsync(string listKey, params string[] values)
        {
            return ListPushAsync(listKey, values.Select(value => value.ToBytes()).ToArray());
        }

        public Task<int> ListPushStringIfExistsAsync(string listKey, string value)
        {
            return ListPushIfExistsAsync(listKey, value.ToBytes());
        }

        public Task<int> ListTailPushStringAsync(string listKey, params string[] values)
        {
            return ListTailPushAsync(listKey, values.Select(value => value.ToBytes()).ToArray());
        }

        public Task<int> ListTailPushStringIfExistsAsync(string listKey, string value)
        {
            return ListTailPushIfExistsAsync(listKey, value.ToBytes());
        }

        public async Task<string> ListPopStringAsync(string listKey)
        {
            var bytes = await ListPopAsync(listKey).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }

        public async Task<Tuple<string, string>> ListBlockingPopStringAsync(int timeoutSeconds, params string[] listKeys)
        {
            CheckListKeys(listKeys);

            var timeoutString = new[] { timeoutSeconds.ToString(CultureInfo.InvariantCulture) };
            var request = ComposeRequest(RedisCommands.ListBlockingPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            if (bytes.Length == 0)
            {
                return null;
            }

            var foundListKey = bytes[0].ConvertBytesToString();
            var value = bytes[1].ConvertBytesToString();
            return Tuple.Create(foundListKey, value);
        }

        public async Task<string> ListTailPopStringAsync(string listKey)
        {
            var bytes = await ListTailPopAsync(listKey).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }

        public async Task<Tuple<string, string>> ListBlockingTailPopStringAsync(int timeoutSeconds, params string[] listKeys)
        {
            CheckListKeys(listKeys);

            var timeoutString = new[] { timeoutSeconds.ToString(CultureInfo.InvariantCulture) };
            var request = ComposeRequest(RedisCommands.ListBlockingTailPop, listKeys.Union(timeoutString));
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            if (bytes.Length == 0)
            {
                return null;
            }

            var foundListKey = bytes[0].ConvertBytesToString();
            var value = bytes[1].ConvertBytesToString();
            return Tuple.Create(foundListKey, value);
        }

        public async Task<string> ListTailPopAndPushStringAsync(string listKey1, string listKey2)
        {
            var bytes = await ListTailPopAndPushAsync(listKey1, listKey2).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }

        public async Task<string> ListBlockingTailPopAndPushStringAsync(string listKey1, string listKey2, int timeoutSeconds)
        {
            var bytes = await ListBlockingTailPopAndPushAsync(listKey1, listKey2, timeoutSeconds).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }

        public async Task<string> ListIndexStringAsync(string listKey, int index)
        {
            var bytes = await ListIndexAsync(listKey, index).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }

        public Task<int> ListInsertBeforeStringAsync(string listKey, string pivot, string value)
        {
            return ListInsertBeforeAsync(listKey, pivot.ToBytes(), value.ToBytes());
        }

        public Task<int> ListInsertAfterStringAsync(string listKey, string pivot, string value)
        {
            return ListInsertAfterAsync(listKey, pivot.ToBytes(), value.ToBytes());
        }

        public async Task<string[]> ListRangeStringAsync(string listKey, int start, int end)
        {
            var bytes = await ListRangeAsync(listKey, start, end);
            return bytes.ConvertByteArrayToStringArray();
        }

        public Task<int> ListRemoveStringAsync(string listKey, int count, string value)
        {
            return ListRemoveAsync(listKey, count, value.ToBytes());
        }

        public Task ListSetStringAsync(string listKey, int index, string value)
        {
            return ListSetAsync(listKey, index, value.ToBytes());
        }
    }
}
