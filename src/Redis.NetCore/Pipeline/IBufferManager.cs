using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IBufferManager
    {
        Task<ArraySegment<byte>> CheckOutAsync(int timeout = 10000);

        void CheckIn(ArraySegment<byte> buffer);

        int AvailableBuffers { get; }

        int TotalBufferSize { get; }

        int ChunkSize { get; }
    }
}