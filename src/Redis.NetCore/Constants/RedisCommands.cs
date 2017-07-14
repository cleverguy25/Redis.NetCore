// <copyright file="RedisCommands.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Redis.NetCore.Constants
{
    public static class RedisCommands
    {
        public const string And = "AND";
        public const string Or = "OR";
        public const string Xor = "XOR";
        public const string Not = "NOT";

        public static readonly byte[] Set = "SET".ToBytes();
        public static readonly byte[] SetRange = "SETRANGE".ToBytes();
        public static readonly byte[] Expiration = "EX".ToBytes();
        public static readonly byte[] PrecisionExpiration = "PX".ToBytes();
        public static readonly byte[] SetNotExists = "NX".ToBytes();
        public static readonly byte[] SetExists = "XX".ToBytes();
        public static readonly byte[] SetChanged = "CH".ToBytes();
        public static readonly byte[] Get = "GET".ToBytes();
        public static readonly byte[] GetRange = "GETRANGE".ToBytes();
        public static readonly byte[] MultipleSet = "MSET".ToBytes();
        public static readonly byte[] MultipleSetNotExists = "MSETNX".ToBytes();
        public static readonly byte[] MultipleGet = "MGET".ToBytes();
        public static readonly byte[] GetSet = "GETSET".ToBytes();
        public static readonly byte[] Append = "APPEND".ToBytes();
        public static readonly byte[] StringLength = "STRLEN".ToBytes();

        public static readonly byte[] HashSet = "HSET".ToBytes();
        public static readonly byte[] HashGet = "HGET".ToBytes();
        public static readonly byte[] HashGetAll = "HGETALL".ToBytes();
        public static readonly byte[] HashMultipleSet = "HMSET".ToBytes();
        public static readonly byte[] HashSetNotExists = "HSETNX".ToBytes();
        public static readonly byte[] HashMultipleGet = "HMGET".ToBytes();
        public static readonly byte[] HashValues = "HVALS".ToBytes();
        public static readonly byte[] HashKeys = "HKEYS".ToBytes();
        public static readonly byte[] HashDelete = "HDEL".ToBytes();
        public static readonly byte[] HashExists = "HEXISTS".ToBytes();
        public static readonly byte[] HashLength = "HLEN".ToBytes();
        public static readonly byte[] HashFieldStringLength = "HSTRLEN".ToBytes();
        public static readonly byte[] HashIncrementBy = "HINCRBY".ToBytes();
        public static readonly byte[] HashIncrementByFloat = "HINCRBYFLOAT".ToBytes();
        public static readonly byte[] HashScan = "HSCAN".ToBytes();

        public static readonly byte[] Exists = "EXISTS".ToBytes();
        public static readonly byte[] TimeToLive = "TTL".ToBytes();
        public static readonly byte[] PrecisionTimeToLive = "PTTL".ToBytes();
        public static readonly byte[] Delete = "DEL".ToBytes();
        public static readonly byte[] Move = "MOVE".ToBytes();
        public static readonly byte[] Expire = "EXPIRE".ToBytes();
        public static readonly byte[] PrecisionExpire = "PEXPIRE".ToBytes();
        public static readonly byte[] PrecisionExpireAt = "PEXPIREAT".ToBytes();
        public static readonly byte[] Persist = "PERSIST".ToBytes();
        public static readonly byte[] Keys = "KEYS".ToBytes();
        public static readonly byte[] Rename = "RENAME".ToBytes();
        public static readonly byte[] RenameNotExists = "RENAMENX".ToBytes();
        public static readonly byte[] RandomKey = "RANDOMKEY".ToBytes();
        public static readonly byte[] Scan = "SCAN".ToBytes();
        public static readonly byte[] Sort = "SORT".ToBytes();
        public static readonly byte[] Ascending = "ASC".ToBytes();
        public static readonly byte[] Descending = "DESC".ToBytes();
        public static readonly byte[] Alpha = "ALPHA".ToBytes();
        public static readonly byte[] By = "BY".ToBytes();
        public static readonly byte[] Store = "STORE".ToBytes();
        public static readonly byte[] Touch = "TOUCH".ToBytes();
        public static readonly byte[] Type = "TYPE".ToBytes();
        public static readonly byte[] Dump = "DUMP".ToBytes();
        public static readonly byte[] Restore = "RESTORE".ToBytes();
        public static readonly byte[] Object = "OBJECT".ToBytes();
        public static readonly byte[] ReferenceCount = "REFCOUNT".ToBytes();
        public static readonly byte[] Encoding = "ENCODING".ToBytes();
        public static readonly byte[] IdleTime = "IDLETIME".ToBytes();

        public static readonly byte[] Ping = "PING".ToBytes();
        public static readonly byte[] Echo = "ECHO".ToBytes();
        public static readonly byte[] Quit = "QUIT".ToBytes();
        public static readonly byte[] SelectDatabase = "SELECT".ToBytes();
        public static readonly byte[] Authenticate = "AUTH".ToBytes();

        public static readonly byte[] BitCount = "BITCOUNT".ToBytes();
        public static readonly byte[] BitOperation = "BITOP".ToBytes();
        public static readonly byte[] BitPosition = "BITPOS".ToBytes();
        public static readonly byte[] GetBit = "GETBIT".ToBytes();
        public static readonly byte[] SetBit = "SETBIT".ToBytes();

        public static readonly byte[] ListPush = "LPUSH".ToBytes();
        public static readonly byte[] ListPushIfExists = "LPUSHX".ToBytes();
        public static readonly byte[] ListTailPush = "RPUSH".ToBytes();
        public static readonly byte[] ListTailPushIfExists = "RPUSHX".ToBytes();
        public static readonly byte[] ListPop = "LPOP".ToBytes();
        public static readonly byte[] ListBlockingPop = "BLPOP".ToBytes();
        public static readonly byte[] ListTailPop = "RPOP".ToBytes();
        public static readonly byte[] ListBlockingTailPop = "BRPOP".ToBytes();
        public static readonly byte[] ListTailPopAndPush = "RPOPLPUSH".ToBytes();
        public static readonly byte[] ListBlockingTailPopAndPush = "BRPOPLPUSH".ToBytes();
        public static readonly byte[] ListIndex = "LINDEX".ToBytes();
        public static readonly byte[] ListInsert = "LINSERT".ToBytes();
        public static readonly byte[] Before = "BEFORE".ToBytes();
        public static readonly byte[] After = "AFTER".ToBytes();
        public static readonly byte[] ListLength = "LLEN".ToBytes();
        public static readonly byte[] ListRange = "LRANGE".ToBytes();
        public static readonly byte[] ListRemove = "LREM".ToBytes();
        public static readonly byte[] ListSet = "LSET".ToBytes();
        public static readonly byte[] ListTrim = "LTRIM".ToBytes();

        public static readonly byte[] SetAdd = "SADD".ToBytes();
        public static readonly byte[] SetCardinality = "SCARD".ToBytes();
        public static readonly byte[] SetIsMember = "SISMEMBER".ToBytes();
        public static readonly byte[] SetDifference = "SDIFF".ToBytes();
        public static readonly byte[] SetIntersection = "SINTER".ToBytes();
        public static readonly byte[] SetUnion = "SUNION".ToBytes();
        public static readonly byte[] SetDifferenceStore = "SDIFFSTORE".ToBytes();
        public static readonly byte[] SetIntersectionStore = "SINTERSTORE".ToBytes();
        public static readonly byte[] SetUnionStore = "SUNIONSTORE".ToBytes();
        public static readonly byte[] SetMembers = "SMEMBERS".ToBytes();
        public static readonly byte[] SetMove = "SMOVE".ToBytes();
        public static readonly byte[] SetPop = "SPOP".ToBytes();
        public static readonly byte[] SetRandomMember = "SRANDMEMBER".ToBytes();
        public static readonly byte[] SetRemove = "SREM".ToBytes();
        public static readonly byte[] SetScan = "SSCAN".ToBytes();

        public static readonly byte[] SortedSetAdd = "ZADD".ToBytes();
        public static readonly byte[] SortedSetCardinality = "ZCARD".ToBytes();
        public static readonly byte[] SortedSetScore = "ZSCORE".ToBytes();
        public static readonly byte[] SortedSetRank = "ZRANK".ToBytes();
        public static readonly byte[] SortedSetReverseRank = "ZREVRANK".ToBytes();
        public static readonly byte[] SortedSetCount = "ZCOUNT".ToBytes();
        public static readonly byte[] SortedSetLexCount = "ZLEXCOUNT".ToBytes();
        public static readonly byte[] SortedSetIncrementBy = "ZINCRBY".ToBytes();
        public static readonly byte[] SortedSetIntersectionStore = "ZINTERSTORE".ToBytes();
        public static readonly byte[] SortedSetUnionStore = "ZUNIONSTORE".ToBytes();
        public static readonly byte[] SortedSetRange = "ZRANGE".ToBytes();
        public static readonly byte[] SortedSetRangeByLex = "ZRANGEBYLEX".ToBytes();
        public static readonly byte[] SortedSetRangeByScore = "ZRANGEBYSCORE".ToBytes();
        public static readonly byte[] SortedSetReverseRange = "ZREVRANGE".ToBytes();
        public static readonly byte[] SortedSetReverseRangeByLex = "ZREVRANGEBYLEX".ToBytes();
        public static readonly byte[] SortedSetReverseRangeByScore = "ZREVRANGEBYSCORE".ToBytes();
        public static readonly byte[] SortedSetRemove = "ZREM".ToBytes();
        public static readonly byte[] SortedSetRemoveRangeByLex = "ZREMRANGEBYLEX".ToBytes();
        public static readonly byte[] SortedSetRemoveRangeByRank = "ZREMRANGEBYRANK".ToBytes();
        public static readonly byte[] SortedSetRemoveRangeByScore = "ZREMRANGEBYSCORE".ToBytes();
        public static readonly byte[] SortedSetScan = "ZSCAN".ToBytes();
        public static readonly byte[] Weight = "WEIGHTS".ToBytes();
        public static readonly byte[] WithScores = "WITHSCORES".ToBytes();
        public static readonly byte[] Limit = "LIMIT".ToBytes();

        public static readonly byte[] Aggregate = "AGGREGATE".ToBytes();
        public static readonly byte[] AggregateSum = "SUM".ToBytes();
        public static readonly byte[] AggregateMin = "MIN".ToBytes();
        public static readonly byte[] AggregateMax = "MAX".ToBytes();

        public static readonly byte[] Increment = "INCR".ToBytes();
        public static readonly byte[] IncrementBy = "INCRBY".ToBytes();
        public static readonly byte[] IncrementByFloat = "INCRBYFLOAT".ToBytes();
        public static readonly byte[] Decrement = "DECR".ToBytes();
        public static readonly byte[] DecrementBy = "DECRBY".ToBytes();

        public static readonly byte[] Configuration = "CONFIG".ToBytes();
        public static readonly byte[] Rewrite = "REWRITE".ToBytes();
        public static readonly byte[] ResetStat = "RESETSTAT".ToBytes();
        public static readonly byte[] BackgroundSave = "BGSAVE".ToBytes();
        public static readonly byte[] Save = "SAVE".ToBytes();
        public static readonly byte[] BackgroundRewriteAppendOnlyFile = "BGREWRITEAOF".ToBytes();
        public static readonly byte[] Client = "CLIENT".ToBytes();
        public static readonly byte[] GetName = "GETNAME".ToBytes();
        public static readonly byte[] SetName = "SETNAME".ToBytes();
        public static readonly byte[] Time = "TIME".ToBytes();
        public static readonly byte[] List = "LIST".ToBytes();
        public static readonly byte[] DatabaseSize = "DBSIZE".ToBytes();
        public static readonly byte[] FlushDatabase = "FLUSHDB".ToBytes();
        public static readonly byte[] FlushAll = "FLUSHALL".ToBytes();
        public static readonly byte[] Async = "ASYNC".ToBytes();
        public static readonly byte[] Information = "INFO".ToBytes();
        public static readonly byte[] LastSave = "LASTSAVE".ToBytes();

        public static readonly byte[] GeoAdd = "GEOADD".ToBytes();
        public static readonly byte[] GeoPosition = "GEOPOS".ToBytes();
        public static readonly byte[] GeoHash = "GEOHASH".ToBytes();
        public static readonly byte[] GeoDistance = "GEODIST".ToBytes();
        public static readonly byte[] GeoRadius = "GEORADIUS".ToBytes();
        public static readonly byte[] GeoRadiusByMember = "GEORADIUSBYMEMBER".ToBytes();
        public static readonly byte[] WithDistance = "WITHDIST".ToBytes();
        public static readonly byte[] WithCoordinates = "WITHCOORD".ToBytes();
        public static readonly byte[] WithHash = "WITHHASH".ToBytes();
        public static readonly byte[] Count = "COUNT".ToBytes();
        public static readonly byte[] StoreDistance = "STOREDIST".ToBytes();
    }
}