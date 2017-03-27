namespace Redis.NetCore.Constants
{
    public static class RedisCommands
    {
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

        public static readonly byte[] Exists = "EXISTS".ToBytes();
        public static readonly byte[] TimeToLive = "TTL".ToBytes();
        public static readonly byte[] PrecisionTimeToLive = "PTTL".ToBytes();
        public static readonly byte[] Delete = "DEL".ToBytes();
        public static readonly byte[] Move = "MOVE".ToBytes();
        public static readonly byte[] Expire = "EXPIRE".ToBytes();
        public static readonly byte[] PrecisionExpire = "PEXPIRE".ToBytes();
        public static readonly byte[] PrecisionExpireAt = "PEXPIREAT".ToBytes();
        public static readonly byte[] Persist = "PERSIST".ToBytes();

        public static readonly byte[] Ping = "PING".ToBytes();
        public static readonly byte[] Echo = "ECHO".ToBytes();
        public static readonly byte[] Quit = "QUIT".ToBytes();
        public static readonly byte[] SelectDatabase = "SELECT".ToBytes();

        public static readonly byte[] BitCount = "BITCOUNT".ToBytes();
        public static readonly byte[] BitOperation = "BITOP".ToBytes();
        public static readonly byte[] BitPosition = "BITPOS".ToBytes();
        public static readonly byte[] GetBit = "GETBIT".ToBytes();
        public static readonly byte[] SetBit = "SETBIT".ToBytes();
        public const string And = "AND";
        public const string Or = "OR";
        public const string Xor = "XOR";
        public const string Not = "NOT";

        public static readonly byte[] Increment = "INCR".ToBytes();
        public static readonly byte[] IncrementBy = "INCRBY".ToBytes();
        public static readonly byte[] IncrementByFloat = "INCRBYFLOAT".ToBytes();
        public static readonly byte[] Decrement = "DECR".ToBytes();
        public static readonly byte[] DecrementBy = "DECRBY".ToBytes();
    }
}
