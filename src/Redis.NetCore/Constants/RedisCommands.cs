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
        public static readonly byte[] Touch = "TOUCH".ToBytes();
        public static readonly byte[] Type = "TYPE".ToBytes();

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

        public static readonly byte[] Increment = "INCR".ToBytes();
        public static readonly byte[] IncrementBy = "INCRBY".ToBytes();
        public static readonly byte[] IncrementByFloat = "INCRBYFLOAT".ToBytes();
        public static readonly byte[] Decrement = "DECR".ToBytes();
        public static readonly byte[] DecrementBy = "DECRBY".ToBytes();
    }
}