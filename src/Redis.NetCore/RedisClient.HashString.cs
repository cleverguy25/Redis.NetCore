// <copyright file="RedisClient.Raw.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisHashStringClient
    {
        public Task<bool> HashSetFieldStringAsync(string hashKey, string field, string data)
        {
            return HashSetFieldAsync(hashKey, field, data.ToBytes());
        }

        public Task HashSetFieldsStringAsync(string hashKey, IDictionary<string, string> dictionary)
        {
            CheckHashKey(hashKey);

            var data = new List<byte[]> { hashKey.ToBytes() };
            foreach (var keyValue in dictionary)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value.ToBytes());
            }

            var request = ComposeRequest(RedisCommands.HashMultipleSet, data.ToArray());
            return SendMultipleCommandAsync(request);
        }

        public Task<bool> HashSetFieldStringNotExistsAsync(string hashKey, string field, string data)
        {
            return HashSetFieldNotExistsAsync(hashKey, field, data.ToBytes());
        }

        public async Task<string> HashGetFieldStringAsync(string hashKey, string field)
        {
            var bytes = await HashGetFieldAsync(hashKey, field).ConfigureAwait(false);
            return ConvertBytesToString(bytes);
        }

        public async Task<string[]> HashGetFieldsStringAsync(string hashKey, params string[] fields)
        {
            var bytes = await HashGetFieldsAsync(hashKey, fields).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<IDictionary<string, string>> HashGetAllFieldsStringAsync(string hashKey)
        {
            var results = await HashGetAllFieldsAsync(hashKey).ConfigureAwait(false);
            return results.ToDictionary(item => item.Key, item => ConvertBytesToString(item.Value));
        }

        public async Task<string[]> HashGetValuesStringAsync(string hashKey)
        {
            var bytes = await HashGetValuesAsync(hashKey).ConfigureAwait(false);
            return ConvertByteArrayToStringArray(bytes);
        }

        public async Task<int> HashGetFieldStringLengthAsync(string hashKey, string field)
        {
            CheckHashKey(hashKey);

            var bytes = await SendCommandAsync(RedisCommands.HashFieldStringLength, hashKey.ToBytes(), field.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }
    }
}