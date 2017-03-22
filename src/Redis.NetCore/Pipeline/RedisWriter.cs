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
            // Inline for performance, even though it makes this method
            // long and har to read.  Comments to help.
            // Write length
            var countBytes = requestData.Length.ToBytes();
            if (_currentPosition + countBytes.Length + 3 > _buffer.Count)
            {
                SaveExistingBuffer();
                await CreateNewBufferAsync().ConfigureAwait(false);
            }

            WriteByte(RedisProtocolContants.Array);
            WriteBytes(countBytes, 0, countBytes.Length);
            WriteBytes(RedisProtocolContants.LineEnding, 0, 2);
            foreach (var item in requestData)
            {
                // Write item length
                countBytes = item.Length.ToBytes();
                if (_currentPosition + countBytes.Length + 3 > _buffer.Count)
                {
                    SaveExistingBuffer();
                    await CreateNewBufferAsync().ConfigureAwait(false);
                }

                WriteByte(RedisProtocolContants.BulkString);
                WriteBytes(countBytes, 0, countBytes.Length);
                WriteBytes(RedisProtocolContants.LineEnding, 0, 2);

                // Write data
                var startPosition = 0;

                while (true)
                {
                    if (WriteData(item, ref startPosition)) break;

                    SaveExistingBuffer();
                    await CreateNewBufferAsync().ConfigureAwait(false);
                }

                // Write line ending
                startPosition = 0;

                while (true)
                {
                    if (WriteData(RedisProtocolContants.LineEnding, ref startPosition)) break;

                    SaveExistingBuffer();
                    await CreateNewBufferAsync().ConfigureAwait(false);
                }
            }
        }

        private bool WriteData(byte[] data, ref int startPosition)
        {
            var length = Math.Min(_buffer.Count - _currentPosition, data.Length - startPosition);

            WriteBytes(data, startPosition, length);
            startPosition += length;
            if (startPosition == data.Length)
            {
                return true;
            }

            return false;
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