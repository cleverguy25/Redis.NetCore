using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisSocketWriter : RedisBaseWriter
    {
        private readonly IAsyncSocket _socket;

        public RedisSocketWriter(IAsyncSocket socket, IBufferManager bufferManager) : base(bufferManager)
        {
            _socket = socket;
        }

        public override async Task FlushWriteBufferAsync()
        {
            if (BytesInBuffer > 0)
            {
                var bufferList = FlushBuffers();
                await _socket.SendAsync(bufferList);
                CheckInBuffers();
            }
        }
    }
}