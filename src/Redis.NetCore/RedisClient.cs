using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Redis.NetCore.Pipeline;
using Redis.NetCore.Sockets;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisClient
    {
        private readonly IRedisPipelinePool _redisPipelinePool;

        public RedisClient(IRedisPipelinePool redisPipelinePool)
        {
            _redisPipelinePool = redisPipelinePool;
        }

        public static IRedisClient CreateClient(RedisConfiguration redisConfiguration, int numConnections = 3)
        {
            var bufferManager = new BufferManager(15, 8192, 10, 20);
            var pipelinePool = new RedisPipelinePool(redisConfiguration, bufferManager, numConnections);
            return new RedisClient(pipelinePool);
        }

        

        public void Dispose()
        {
            _redisPipelinePool?.Dispose();
        }
    }
}
