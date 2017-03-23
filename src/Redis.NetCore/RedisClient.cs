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
