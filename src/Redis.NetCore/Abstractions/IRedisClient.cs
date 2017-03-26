using System;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisClient : IRedisConnectionClient, IRedisRawClient, IRedisKeyClient, IRedisStringClient, IRedisBitClient, IDisposable
    {
    }
}