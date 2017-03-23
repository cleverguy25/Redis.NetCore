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
