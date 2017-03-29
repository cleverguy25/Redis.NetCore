// <copyright file="DataExtensions.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Redis.NetCore
{
    public static class DataExtensions
    {
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

        public static byte[] CollapseArray(this IEnumerable<byte[]> value)
        {
            return value?.SelectMany(item => item)?.ToArray();
        }
    }
}
