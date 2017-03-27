using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisSocketReader : RedisBaseReader
    {
        private readonly IAsyncSocket _asyncSocket;

        public RedisSocketReader(IAsyncSocket asyncSocket, IBufferManager bufferManager) : base(bufferManager)
        {
            _asyncSocket = asyncSocket;
        }

        protected override async Task ReadNextResponseAsync()
        {
            if (_bufferList.Count > 10)
            {
                CheckInBuffers();
            }

            var buffer = await BufferManager.CheckOutAsync().ConfigureAwait(false);
            _bufferList.Add(buffer);
            CurrentResponse = await _asyncSocket.ReceiveAsync(buffer);
            CurrentPosition = 0;
        }
    }
}