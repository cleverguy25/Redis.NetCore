// <copyright file="RedisClient.Raw.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisHashClient
    {
        public async Task<bool> HashSetFieldAsync(string hashKey, string field, byte[] data)
        {
            CheckHashKey(hashKey);
            CheckField(field);

            var bytes = await SendCommandAsync(RedisCommands.HashSet, hashKey.ToBytes(), field.ToBytes(), data).ConfigureAwait(false);
            return bytes[0] == '1';
        }

        public Task HashSetFieldsAsync(string hashKey, IDictionary<string, byte[]> dictionary)
        {
            CheckHashKey(hashKey);

            var data = new List<byte[]> { hashKey.ToBytes() };
            foreach (var keyValue in dictionary)
            {
                data.Add(keyValue.Key.ToBytes());
                data.Add(keyValue.Value);
            }

            var request = ComposeRequest(RedisCommands.HashMultipleSet, data);
            return SendMultipleCommandAsync(request);
        }

        public async Task<bool> HashSetFieldNotExistsAsync(string hashKey, string field, byte[] data)
        {
            CheckHashKey(hashKey);
            CheckField(field);

            var bytes = await SendCommandAsync(RedisCommands.HashSetNotExists, hashKey.ToBytes(), field.ToBytes(), data).ConfigureAwait(false);
            return bytes[0] == '1';
        }

        public Task<byte[][]> HashGetFieldsAsync(string hashKey, params string[] fields)
        {
            CheckHashKey(hashKey);

            if (fields == null)
            {
                return MultipleNullResultTask;
            }

            if (fields.Length == 0)
            {
                return MultipleEmptyResultTask;
            }

            var request = ComposeRequest(RedisCommands.HashMultipleGet, hashKey.ToBytes(), fields);
            return SendMultipleCommandAsync(request);
        }

        public Task<byte[]> HashGetFieldAsync(string hashKey, string field)
        {
            CheckHashKey(hashKey);

            if (field == null)
            {
                return NullResultTask;
            }

            if (field.Length == 0)
            {
                return EmptyResultTask;
            }

            return SendCommandAsync(RedisCommands.HashGet, hashKey.ToBytes(), field.ToBytes());
        }

        public async Task<IDictionary<string, byte[]>> HashGetAllFieldsAsync(string hashKey)
        {
            CheckHashKey(hashKey);

            var request = ComposeRequest(RedisCommands.HashGetAll, hashKey.ToBytes());
            var bytes = await SendMultipleCommandAsync(request);
            var results = HashScanCursor.ConvertFieldValueToDictionary(bytes);

            return results;
        }

        public async Task<string[]> HashGetKeysAsync(string hashKey)
        {
            CheckHashKey(hashKey);

            var request = ComposeRequest(RedisCommands.HashKeys, hashKey.ToBytes());
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes.Select(ConvertBytesToString).ToArray();
        }

        public Task<byte[][]> HashGetValuesAsync(string hashKey)
        {
            CheckHashKey(hashKey);

            var request = ComposeRequest(RedisCommands.HashValues, hashKey.ToBytes());
            return SendMultipleCommandAsync(request);
        }

        public async Task<int> HashGetLengthAsync(string hashKey)
        {
            CheckHashKey(hashKey);

            var bytes = await SendCommandAsync(RedisCommands.HashLength, hashKey.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<int> HashDeleteFieldsAsync(string hashKey, params string[] fields)
        {
            CheckHashKey(hashKey);

            if (fields == null || fields.Length == 0)
            {
                return 0;
            }

            var request = ComposeRequest(RedisCommands.HashDelete, hashKey.ToBytes(), fields);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes[0]);
        }

        public async Task<bool> HashFieldExistsAsync(string hashKey, string field)
        {
            CheckHashKey(hashKey);
            CheckField(field);

            var bytes = await SendCommandAsync(RedisCommands.HashExists, hashKey.ToBytes(), field.ToBytes()).ConfigureAwait(false);
            return ConvertBytesToBool(bytes);
        }

        public async Task<int> HashIncrementAsync(string hashKey, string field, int amount)
        {
            CheckKey(hashKey);
            CheckField(field);

            var amountBytes = amount.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.HashIncrementBy, hashKey.ToBytes(), field.ToBytes(), amountBytes).ConfigureAwait(false);
            return ConvertBytesToInteger(bytes);
        }

        public async Task<float> HashIncrementAsync(string hashKey, string field, float amount)
        {
            CheckKey(hashKey);
            CheckField(field);

            var amountBytes = amount.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.HashIncrementByFloat, hashKey.ToBytes(), field.ToBytes(), amountBytes).ConfigureAwait(false);
            var stringValue = Encoding.UTF8.GetString(bytes);
            if (float.TryParse(stringValue, out float value))
            {
                return value;
            }

            throw new RedisException($"Could not parse [{stringValue}] for Hash [{hashKey}] and field [{field}]");
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey)
        {
            CheckKey(hashKey);

            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), ZeroBit).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, ScanCursor cursor)
        {
            CheckKey(hashKey);

            var cursorPositionBytes = cursor.CursorPosition.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), cursorPositionBytes).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), ZeroBit, "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, ScanCursor cursor, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToString(CultureInfo.InvariantCulture).ToBytes();
            var cursorPositionBytes = cursor.CursorPosition.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), cursorPositionBytes, "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, string match)
        {
            CheckKey(hashKey);

            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), ZeroBit, "MATCH".ToBytes(), match.ToBytes()).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, ScanCursor cursor, string match)
        {
            CheckKey(hashKey);

            var cursorPositionBytes = cursor.CursorPosition.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), cursorPositionBytes, "MATCH".ToBytes(), match.ToBytes()).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, string match, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), ZeroBit, "MATCH".ToBytes(), match.ToBytes(), "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        public async Task<HashScanCursor> HashScanAsync(string hashKey, ScanCursor cursor, string match, int count)
        {
            CheckKey(hashKey);

            var countBytes = count.ToString(CultureInfo.InvariantCulture).ToBytes();
            var cursorPositionBytes = cursor.CursorPosition.ToString(CultureInfo.InvariantCulture).ToBytes();
            var bytes = await SendMultipleCommandAsync(RedisCommands.HashScan, hashKey.ToBytes(), cursorPositionBytes, "MATCH".ToBytes(), match.ToBytes(), "COUNT".ToBytes(), countBytes).ConfigureAwait(false);
            return ConvertToHashScanCursor(bytes);
        }

        private static void CheckHashKey(string hashKey)
        {
            if (string.IsNullOrEmpty(hashKey))
            {
                throw new ArgumentNullException(nameof(hashKey), "hashKey cannot be null or empty");
            }
        }

        private static void CheckField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentNullException(nameof(field), "field cannot be null or empty");
            }
        }

        private static HashScanCursor ConvertToHashScanCursor(byte[][] bytes)
        {
            var cursorPosition = ConvertBytesToInteger(bytes[0]);
            return new HashScanCursor(cursorPosition, bytes[1]);
        }
    }
}