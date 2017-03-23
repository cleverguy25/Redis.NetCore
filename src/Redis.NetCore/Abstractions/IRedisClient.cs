using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisClient : IRedisConnectionClient, IRedisRawClient, IRedisKeyClient, IRedisStringClient, IDisposable
    {
    }
}