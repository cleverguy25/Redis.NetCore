namespace Redis.NetCore.Constants
{
    public static class RedisCommands
    {
        public static readonly byte[] Set = "SET".ToBytes();
        public static readonly byte[] Expiration = "EX".ToBytes();
        public static readonly byte[] PrecisionExpiration = "PX".ToBytes();
        public static readonly byte[] SetNotExists = "NX".ToBytes();
        public static readonly byte[] SetExists = "XX".ToBytes();
        public static readonly byte[] Get = "GET".ToBytes();
        public static readonly byte[] MultipleSet = "MSET".ToBytes();
        public static readonly byte[] MultipleSetNotExists = "MSETNX".ToBytes();
        public static readonly byte[] MultipleGet = "MGET".ToBytes();
        public static readonly byte[] GetSet = "GETSET".ToBytes();
        public static readonly byte[] Append = "APPEND".ToBytes();
        public static readonly byte[] StringLength = "STRLEN".ToBytes();

        public static readonly byte[] Exists = "EXISTS".ToBytes();
        public static readonly byte[] TimeToLive = "TTL".ToBytes();
        public static readonly byte[] Delete = "DEL".ToBytes();
        public static readonly byte[] Move = "MOVE".ToBytes();

        public static readonly byte[] Ping = "PING".ToBytes();
        public static readonly byte[] Echo = "ECHO".ToBytes();
        public static readonly byte[] Quit = "QUIT".ToBytes();
        public static readonly byte[] SelectDatabase = "SELECT".ToBytes();
    }
}
