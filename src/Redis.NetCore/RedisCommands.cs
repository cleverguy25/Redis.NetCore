using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.NetCore
{
    public static class RedisCommands
    {
        public static readonly byte[] Set = "SET".ToBytes();
        public static readonly byte[] SetExpiration = "SETEX".ToBytes();
        public static readonly byte[] PrecisionSetExpiration = "PSETEX".ToBytes();
        public static readonly byte[] Get = "GET".ToBytes();
        public static readonly byte[] MultipleGet = "MGET".ToBytes();
        public static readonly byte[] TimeToLive = "TTL".ToBytes();
    }
}
