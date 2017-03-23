﻿using System;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisClient : IRedisConnectionClient, IRedisRawClient, IRedisKeyClient, IRedisStringClient, IDisposable
    {
    }
}