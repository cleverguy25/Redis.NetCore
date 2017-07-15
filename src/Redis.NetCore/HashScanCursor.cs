// <copyright file="HashScanCursor.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.NetCore
{
    public class HashScanCursor : ScanCursor
    {
        public HashScanCursor()
        {
        }

        public HashScanCursor(int cursorPosition, byte[] data)
            : base(cursorPosition, data)
        {
        }

        public static Dictionary<string, byte[]> ConvertFieldValueToDictionary(byte[][] bytes)
        {
            var results = new Dictionary<string, byte[]>();
            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                var key = Encoding.UTF8.GetString(bytes[i]);
                results[key] = bytes[i + 1];
            }

            return results;
        }

        public static Dictionary<string, string> ConvertFieldValueToStringDictionary(byte[][] bytes)
        {
            var results = new Dictionary<string, string>();
            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                var key = Encoding.UTF8.GetString(bytes[i]);
                results[key] = Encoding.UTF8.GetString(bytes[i + 1]);
            }

            return results;
        }

        public IDictionary<string, byte[]> GetFields()
        {
            var bytes = GetValues().ToArray();
            return ConvertFieldValueToDictionary(bytes);
        }

        public IDictionary<string, string> GetFieldsString()
        {
            var bytes = GetValues().ToArray();
            return ConvertFieldValueToStringDictionary(bytes);
        }
    }
}