// <copyright file="RedisProtocolContants.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Redis.NetCore.Constants
{
    public static class RedisProtocolContants
    {
        public static readonly byte Minus = '-'.ToByte();
        public static readonly byte Zero = '0'.ToByte();
        public static readonly byte SimpleString = '+'.ToByte();
        public static readonly byte Integer = ':'.ToByte();
        public static readonly byte Error = '-'.ToByte();
        public static readonly byte Array = '*'.ToByte();
        public static readonly byte BulkString = '$'.ToByte();
        public static readonly byte CarriageReturn = '\r'.ToByte();
        public static readonly byte LineFeed = '\n'.ToByte();
        public static readonly byte[] LineEnding = "\r\n".ToBytes();
    }
}
