using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public interface IRedisPipeline : IDisposable
    {
        void SaveQueue(IRedisPipeline redisPipeline);

        Task<byte[]> SendCommandAsync(params byte[][] requestData);

        Task<byte[][]> SendMultipleCommandAsync(params byte[][] requestData);

        bool IsErrorState { get; }

        ConcurrentQueue<RedisPipelineItem> RequestQueue { get; }

        void ThrowErrorForRemainingResponseQueueItems();
    }
}