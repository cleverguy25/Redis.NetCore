﻿// <copyright file="RedisClient.Bit.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisBitClient
    {
        private static readonly byte[] OneBit = "1".ToBytes();
        private static readonly byte[] ZeroBit = "0".ToBytes();

        public async Task<int> GetBitCountAsync(string key, int begin = 0, int end = -1)
        {
            CheckKey(key);

            var beginBytes = begin.ToBytes();
            var endBytes = end.ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.BitCount, key.ToBytes(), beginBytes, endBytes).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<int> GetBitPositionAsync(string key, bool bit, int begin = 0, int end = -1)
        {
            CheckKey(key);

            var bitBytes = bit ? OneBit : ZeroBit;
            var beginBytes = begin.ToBytes();
            var endBytes = end.ToBytes();
            var bytes =
                await SendCommandAsync(RedisCommands.BitPosition, key.ToBytes(), bitBytes, beginBytes, endBytes).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public async Task<bool> SetBitAsync(string key, int index, bool bit)
        {
            CheckKey(key);

            var bitBytes = bit ? OneBit : ZeroBit;
            var indexBytes = index.ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.SetBit, key.ToBytes(), indexBytes, bitBytes).ConfigureAwait(false);
            return bytes.ConvertBytesToBool();
        }

        public async Task<bool> GetBitAsync(string key, int index)
        {
            CheckKey(key);

            var indexBytes = index.ToBytes();
            var bytes = await SendCommandAsync(RedisCommands.GetBit, key.ToBytes(), indexBytes).ConfigureAwait(false);
            return bytes.ConvertBytesToBool();
        }

        public Task<int> PerformBitwiseAndAsync(string destinationKey, params string[] sourceKeys)
        {
            return PerformOperation(RedisCommands.And, destinationKey, sourceKeys);
        }

        public Task<int> PerformBitwiseOrAsync(string destinationKey, params string[] sourceKeys)
        {
            return PerformOperation(RedisCommands.Or, destinationKey, sourceKeys);
        }

        public Task<int> PerformBitwiseXorAsync(string destinationKey, params string[] sourceKeys)
        {
            return PerformOperation(RedisCommands.Xor, destinationKey, sourceKeys);
        }

        public Task<int> PerformBitwiseNotAsync(string destinationKey, string sourceKey)
        {
            return PerformOperation(RedisCommands.Not, destinationKey, sourceKey);
        }

        private async Task<int> PerformOperation(string operation, string destinationKey, params string[] sourceKeys)
        {
            CheckKey(destinationKey);

            var keys = new List<string> { operation, destinationKey };
            keys.AddRange(sourceKeys);
            var request = ComposeRequest(RedisCommands.BitOperation, keys);
            var bytes = await SendMultipleCommandAsync(request).ConfigureAwait(false);
            return bytes[0].ConvertBytesToInteger();
        }
    }
}