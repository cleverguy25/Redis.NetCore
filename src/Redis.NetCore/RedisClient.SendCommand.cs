// <copyright file="RedisClient.SendCommand.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
        private static byte[][] ComposeRequest(byte[] commandName, IReadOnlyList<string> values)
        {
            var request = new byte[values.Count + 1][];
            request[0] = commandName;

            for (var i = 0; i < values.Count; i++)
            {
                request[i + 1] = values[i].ToBytes();
            }

            return request;
        }

        private static byte[][] ComposeRequest(byte[] commandName, IReadOnlyList<byte[]> values)
        {
            var request = new byte[values.Count + 1][];
            request[0] = commandName;

            for (var i = 0; i < values.Count; i++)
            {
                request[i + 1] = values[i];
            }

            return request;
        }

        private static int ConvertBytesToInteger(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate(0, (current, currentByte) => (current * 10) + currentByte - RedisProtocolContants.Zero);
        }

        private static long ConvertBytesToLong(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate(0, (current, currentByte) => (current * 10) + currentByte - RedisProtocolContants.Zero);
        }

        private static bool ConvertBytesToBool(IReadOnlyList<byte> bytes)
        {
            return bytes[0] == '1';
        }

        private static string ConvertBytesToString(byte[] bytes)
        {
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }

        private static void CheckKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null or empty");
            }
        }

        private async Task<byte[]> SendCommandAsync(params byte[][] requestData)
        {
            var pipeline = await _redisPipelinePool.GetPipelineAsync().ConfigureAwait(false);
            return await pipeline.SendCommandAsync(requestData).ConfigureAwait(false);
        }

        private async Task<byte[][]> SendMultipleCommandAsync(params byte[][] requestData)
        {
            var pipeline = await _redisPipelinePool.GetPipelineAsync().ConfigureAwait(false);
            return await pipeline.SendMultipleCommandAsync(requestData).ConfigureAwait(false);
        }
    }
}