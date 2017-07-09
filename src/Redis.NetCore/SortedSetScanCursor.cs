// <copyright file="ScanCursor.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public class SortedSetScanCursor : ScanCursor
    {
        public SortedSetScanCursor()
        {
        }

        public SortedSetScanCursor(int cursorPosition, byte[] data)
            : base(cursorPosition, data)
        {
        }

        public static IEnumerable<(byte[] Member, int Weight)> GetMembers(byte[][] bytes)
        {
            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                var member = bytes[i];
                var weight = bytes[i + 1].ConvertBytesToInteger();
                yield return (member, weight);
            }
        }

        public static IEnumerable<(string Member, int Weight)> GetMembersString(byte[][] bytes)
        {
            for (var i = 0; i < bytes.Length - 1; i += 2)
            {
                var member = Encoding.UTF8.GetString(bytes[i]);
                var weight = bytes[i + 1].ConvertBytesToInteger();
                yield return (member, weight);
            }
        }

        public IEnumerable<(byte[] Member, int Weight)> GetMembers()
        {
            var bytes = GetValues().ToArray();
            return GetMembers(bytes);
        }

        public IEnumerable<(string Member, int Weight)> GetMembersString()
        {
            var bytes = GetValues().ToArray();
            return GetMembersString(bytes);
        }
    }
}