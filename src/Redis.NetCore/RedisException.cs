using System;

namespace Redis.NetCore
{
    public class RedisException : Exception
    {
        public RedisException(string message) : base(message)
        {
        }

        public RedisException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
