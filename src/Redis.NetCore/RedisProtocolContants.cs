using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.NetCore
{
    public static class RedisProtocolContants
    {
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
