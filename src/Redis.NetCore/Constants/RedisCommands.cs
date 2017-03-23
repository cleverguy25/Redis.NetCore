namespace Redis.NetCore.Constants
{
    public static class RedisCommands
    {
        public static readonly byte[] Set = "SET".ToBytes();
        public static readonly byte[] SetNotExists = "SETNX".ToBytes();
        public static readonly byte[] SetExpiration = "SETEX".ToBytes();
        public static readonly byte[] PrecisionSetExpiration = "PSETEX".ToBytes();
        public static readonly byte[] Get = "GET".ToBytes();
        public static readonly byte[] MultipleSet = "MSET".ToBytes();
        public static readonly byte[] MultipleGet = "MGET".ToBytes();
        public static readonly byte[] GetSet = "GETSET".ToBytes();
        public static readonly byte[] Append = "APPEND".ToBytes();

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
