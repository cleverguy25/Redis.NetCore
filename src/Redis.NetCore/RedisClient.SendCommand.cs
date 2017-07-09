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
        private static byte[][] ComposeRequest(byte[] commandName, IEnumerable<string> values)
        {
            return ComposeRequest(commandName, values.ToBytes());
        }

        private static byte[][] ComposeRequest(byte[] commandName, params byte[][] values)
        {
            var request = new byte[values.Length + 1][];
            request[0] = commandName;

            CopyBytesToRequest(values, request, 1);

            return request;
        }

        private static byte[][] ComposeRequest(byte[] commandName, byte[] extra, IEnumerable<string> values)
        {
            return ComposeRequest(commandName, extra, values.ToBytes());
        }

        private static byte[][] ComposeRequest(byte[] commandName, byte[] extra, byte[] extra2, IEnumerable<string> values)
        {
            return ComposeRequest(commandName, extra, extra2, values.ToBytes());
        }

        private static byte[][] ComposeRequest(byte[] commandName, byte[] extra)
        {
            var request = new byte[2][];
            request[0] = commandName;
            request[1] = extra;

            return request;
        }

        private static byte[][] ComposeRequest(byte[] commandName, byte[] extra, params byte[][] values)
        {
            var request = new byte[values.Length + 2][];
            request[0] = commandName;
            request[1] = extra;

            CopyBytesToRequest(values, request, 2);

            return request;
        }

        private static byte[][] ComposeRequest(byte[] commandName, byte[] extra, byte[] extra2, params byte[][] values)
        {
            var request = new byte[values.Length + 3][];
            request[0] = commandName;
            request[1] = extra;
            request[2] = extra2;

            CopyBytesToRequest(values, request, 3);

            return request;
        }

        private static int CopyBytesToRequest(IReadOnlyList<byte[]> values, IList<byte[]> request, int offset)
        {
            for (var i = 0; i < values.Count; i++)
            {
                request[i + offset] = values[i];
            }

            return values.Count + offset;
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

        private static string[] ConvertByteArrayToStringArray(IEnumerable<byte[]> bytes)
        {
            return bytes.Select(ConvertBytesToString).ToArray();
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