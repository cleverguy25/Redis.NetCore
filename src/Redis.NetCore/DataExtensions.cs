// <copyright file="DataExtensions.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public static class DataExtensions
    {
        public static byte[][] ToBytes(this IEnumerable<string> values)
        {
            return values.Select(value => value.ToBytes()).ToArray();
        }

        public static byte[] ToBytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static byte ToByte(this char value)
        {
            return BitConverter.GetBytes(value)[0];
        }

        public static byte[] ToBytes(this int value)
        {
            return ToBytes(value.ToString(CultureInfo.InvariantCulture));
        }

        public static byte[] ToBytes(this long value)
        {
            return ToBytes(value.ToString(CultureInfo.InvariantCulture));
        }

        public static byte[] ToBytes(this float value)
        {
            return ToBytes(value.ToString(CultureInfo.InvariantCulture));
        }

        public static byte[] CollapseArray(this IEnumerable<byte[]> value)
        {
            return value?.SelectMany(item => item)?.ToArray();
        }

        public static int ConvertBytesToInteger(this IEnumerable<byte> bytes)
        {
            return bytes.Aggregate(0, (current, currentByte) => (current * 10) + currentByte - RedisProtocolContants.Zero);
        }

        public static bool ConvertBytesToBool(this IReadOnlyList<byte> bytes)
        {
            return bytes[0] == '1';
        }

        public static string[] ConvertByteArrayToStringArray(this IEnumerable<byte[]> bytes)
        {
            return bytes.Select(ConvertBytesToString).ToArray();
        }

        public static string ConvertBytesToString(this byte[] bytes)
        {
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }

        public static Dictionary<string, string> ConvertBytesToDictionary(this byte[][] bytes)
        {
            var results = new Dictionary<string, string>();
            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                var key = Encoding.UTF8.GetString(bytes[i]);
                results[key] = Encoding.UTF8.GetString(bytes[i + 1]);
            }

            return results;
        }
    }
}
