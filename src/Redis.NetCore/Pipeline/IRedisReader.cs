using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IRedisReader
    {
        Task ReadAsync(RedisPipelineItem redisItem);

        void CheckInBuffers();
    }
}