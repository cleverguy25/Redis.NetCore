using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public class RedisWriter : IRedisWriter
    {
        private readonly List<ArraySegment<byte>> _bufferList = new List<ArraySegment<byte>>();

        private readonly IBufferManager _bufferManager;
        private readonly object _lockObject = new object();
        private readonly List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();
        private ArraySegment<byte> _buffer;
        private int _currentPosition;

        public RedisWriter(IBufferManager bufferManager)
        {
            _bufferManager = bufferManager;
        }

        public int BytesInBuffer { get; private set; }

        public int BufferCount => _sendBufferList.Count;

        public async Task WriteRedisRequestAsync(byte[][] requestData)
        {
            await WriteCountLineAsync(RedisProtocolContants.Array, requestData.Length).ConfigureAwait(false);
            foreach (var item in requestData)
            {
                await WriteCountLineAsync(RedisProtocolContants.BulkString, item.Length).ConfigureAwait(false);
                await WriteDataAsync(item).ConfigureAwait(false);
                await WriteDataAsync(RedisProtocolContants.LineEnding).ConfigureAwait(false);
            }
        }

        public List<ArraySegment<byte>> FlushBuffers()
        {
            SaveExistingBuffer();
            var list = _sendBufferList.ToList();
            _sendBufferList.Clear();
            BytesInBuffer = 0;
            return list;
        }

        public void CheckInBuffers()
        {
            lock (_lockObject)
            {
                foreach (var buffer in _bufferList)
                {
                    _bufferManager.CheckIn(buffer);
                }

                _bufferList.Clear();
            }
        }

        public async Task CreateNewBufferAsync()
        {
            _buffer = await _bufferManager.CheckOutAsync().ConfigureAwait(false);
            _bufferList.Add(_buffer);
            _currentPosition = 0;
        }

        private void SaveExistingBuffer()
        {
            var arraySegment = new ArraySegment<byte>(_buffer.Array, _buffer.Offset, _currentPosition);
            _sendBufferList.Add(arraySegment);
            _currentPosition = 0;
        }

        private Task WriteCountLineAsync(byte symbol, int count)
        {
            var countBytes = count.ToBytes();
            return WriteLengthAsync(symbol, countBytes);
        }

        private async Task WriteLengthAsync(byte symbol, byte[] data)
        {
            if (_currentPosition + data.Length + 3 > _buffer.Count)
            {
                SaveExistingBuffer();
                await CreateNewBufferAsync().ConfigureAwait(false);
            }

            WriteByte(symbol);
            WriteBytes(data, 0, data.Length);
            WriteBytes(RedisProtocolContants.LineEnding, 0, 2);
        }

        private async Task WriteDataAsync(byte[] data)
        {
            var startPosition = 0;

            while (true)
            {
                var length = Math.Min(_buffer.Count - _currentPosition, data.Length - startPosition);

                WriteBytes(data, startPosition, length);
                startPosition += length;
                if (startPosition == data.Length)
                {
                    break;
                }

                SaveExistingBuffer();
                await CreateNewBufferAsync().ConfigureAwait(false);
            }
        }

        private void WriteByte(byte value)
        {
            _buffer.Array[_buffer.Offset + _currentPosition] = value;
            _currentPosition++;
            BytesInBuffer++;
        }

        private void WriteBytes(byte[] value, int startPosition, int length)
        {
            Array.Copy(value, startPosition, _buffer.Array, _buffer.Offset + _currentPosition, length);
            _currentPosition += length;
            BytesInBuffer += length;
        }
    }
}