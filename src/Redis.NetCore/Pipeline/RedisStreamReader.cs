using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisStreamReader : RedisBaseReader
    {
        private readonly Stream _stream;

        public RedisStreamReader(Stream stream, IBufferManager bufferManager) : base(bufferManager)
        {
            _stream = stream;
        }

        protected override async Task ReadNextResponseAsync()
        {
            if (_bufferList.Count > 10)
            {
                CheckInBuffers();
            }

            var buffer = await BufferManager.CheckOutAsync().ConfigureAwait(false);
            _bufferList.Add(buffer);
            var bytesRead = await _stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
            CurrentResponse = new ArraySegment<byte>(buffer.Array, buffer.Offset, bytesRead);
            CurrentPosition = 0;
        }
    }
}