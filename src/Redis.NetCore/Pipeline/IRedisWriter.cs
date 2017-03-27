using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IRedisWriter
    {
        int BytesInBuffer { get; }

        int BufferCount { get; }

        Task WriteRedisRequestAsync(byte[][] requestData);

        List<ArraySegment<byte>> FlushBuffers();

        void CheckInBuffers();

        Task CreateNewBufferAsync();

        Task FlushWriteBufferAsync();
    }
}