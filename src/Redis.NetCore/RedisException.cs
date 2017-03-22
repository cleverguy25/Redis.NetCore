using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
