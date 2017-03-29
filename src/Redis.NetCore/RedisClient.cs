// <copyright file="RedisClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Redis.NetCore.Pipeline;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisClient
    {
        private readonly IRedisPipelinePool _redisPipelinePool;

        public RedisClient(IRedisPipelinePool redisPipelinePool)
        {
            _redisPipelinePool = redisPipelinePool;
        }

        public static IRedisClient CreateClient(RedisConfiguration redisConfiguration, int numConnections = 3, int bufferSize = 8192, int maxBuffers = 1000)
        {
            var bufferManager = new BufferManager(15, bufferSize, 10, maxBuffers);
            var pipelinePool = new RedisPipelinePool(redisConfiguration, bufferManager, numConnections);
            return new RedisClient(pipelinePool);
        }

        public void Dispose()
        {
            _redisPipelinePool?.Dispose();
        }
    }
}
