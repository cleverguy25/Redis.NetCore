using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisReader : IRedisReader
    {
        private readonly IAsyncSocket _asyncSocket;
        private readonly List<ArraySegment<byte>> _bufferList = new List<ArraySegment<byte>>();
        private readonly IBufferManager _bufferManager;
        private readonly object _lockObject = new object();
        private int _currentPosition;
        private ArraySegment<byte> _currentResponse;

        public RedisReader(IBufferManager bufferManager, IAsyncSocket asyncSocket)
        {
            _bufferManager = bufferManager;
            _bufferManager = new BufferManager(15, 8192, 10, 20);
            _asyncSocket = asyncSocket;
        }

        public async Task ReadAsync(RedisPipelineItem redisItem)
        {
            await ReadResponseAsync().ConfigureAwait(false);

            if (_currentPosition >= _currentResponse.Count)
            {
                await ReadNextResponseAsync().ConfigureAwait(false);
            }

            var firstChar = ReadFirstCharAsync();
            if (firstChar == RedisProtocolContants.SimpleString)
            {
                var value = await ReadSimpleStringAsync().ConfigureAwait(false);
                ProcessValue(redisItem, value);
            }
            else if (firstChar == RedisProtocolContants.Integer)
            {
                var value = await ReadSimpleStringAsync().ConfigureAwait(false);
                ProcessValue(redisItem, value);
            }
            else if (firstChar == RedisProtocolContants.BulkString)
            {
                var value = await ReadBulkStringAsync().ConfigureAwait(false);
                ProcessValue(redisItem, value);
            }
            else if (firstChar == RedisProtocolContants.Array)
            {
                var value = await ReadArrayAsync().ConfigureAwait(false);
                redisItem.OnSuccess(value);
            }
            else if (firstChar == RedisProtocolContants.Error)
            {
                var errorBytes = await ReadSimpleStringAsync().ConfigureAwait(false);
                var errorText = Encoding.UTF8.GetString(errorBytes.CollapseArray());
                var exception = new RedisException(errorText);
                redisItem.OnError(exception);
            }
            else
            {
                var exception = new RedisException("Could not process Redis response.");
                throw exception; // restart the pipeline.
            }
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

        private static void ProcessValue(RedisPipelineItem redisItem, byte[][] value)
        {
            var result = new byte[1][];
            result[0] = value.CollapseArray();
            redisItem.OnSuccess(result);
        }

        private byte ReadFirstCharAsync()
        {
            var firstChar = _currentResponse.Array[_currentResponse.Offset + _currentPosition];
            _currentPosition++;
            return firstChar;
        }

        private async Task<byte[][]> ReadArrayAsync()
        {
            var length = await ReadLengthAsync().ConfigureAwait(false);
            var bytes = new List<byte[]>();
            for (var i = 0; i < length; i++)
            {
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                }

                var firstChar = ReadFirstCharAsync();
                if (firstChar == RedisProtocolContants.SimpleString)
                {
                    var value = await ReadSimpleStringAsync().ConfigureAwait(false);
                    bytes.Add(value.CollapseArray());
                }
                else if (firstChar == RedisProtocolContants.BulkString)
                {
                    var value = await ReadBulkStringAsync().ConfigureAwait(false);
                    bytes.Add(value.CollapseArray());
                }
            }

            return bytes.ToArray();
        }

        private async Task<byte[][]> ReadBulkStringAsync()
        {
            var length = await ReadLengthAsync().ConfigureAwait(false);
            if (length == -1)
            {
                return null;
            }

            var bytes = new List<byte[]>();
            while (true)
            {
                if (_currentPosition + length < _currentResponse.Count)
                {
                    var final = GetByteSection(length);
                    bytes.Add(final);
                    _currentPosition += 2;
                    return bytes.ToArray();
                }

                var count = _currentResponse.Count - _currentPosition;
                var fragment = GetByteSection(count);
                bytes.Add(fragment);
                length = length - count;
                await ReadNextResponseAsync().ConfigureAwait(false);
            }
        }
        
        private async Task<int> ReadLengthAsync()
        {
            var lengthBytes = await ReadSimpleStringAsync().ConfigureAwait(false);
            var lengthString = Encoding.UTF8.GetString(lengthBytes[0]);
            int length;
            if (int.TryParse(lengthString, out length) == false)
            {
                throw new RedisException($"[{lengthString}] is not a valid length.");
            }

            return length;
        }

        private async Task<byte[][]> ReadSimpleStringAsync()
        {
            var isLastCharCarriageReturn = false;
            var bytes = new List<byte[]>();
            while (true)
            {
                var position = GetLineBreak(isLastCharCarriageReturn);

                if (position != -1)
                {
                    var length = position - _currentPosition;
                    if (length == 0)
                    {
                        _currentPosition += 1;
                        return bytes.ToArray();
                    }

                    var final = GetByteSection(position - _currentPosition);
                    bytes.Add(final);
                    _currentPosition += 2;
                    return bytes.ToArray();
                }

                isLastCharCarriageReturn = ProcessFragment(bytes);

                await ReadNextResponseAsync().ConfigureAwait(false);
            }
        }

        private bool ProcessFragment(ICollection<byte[]> bytes)
        {
            var fragment = GetByteSection(_currentResponse.Count - _currentPosition);
            if (fragment.Length == 0)
            {
                return false;
            }

            var lastChar = fragment.Length - 1;
            var isLastCharCarriageReturn = fragment[lastChar] == RedisProtocolContants.CarriageReturn;

            if (isLastCharCarriageReturn)
            {
                var copy = new byte[lastChar];
                Array.Copy(fragment, copy, lastChar);
                fragment = copy;
            }

            bytes.Add(fragment);
            return isLastCharCarriageReturn;
        }

        private int GetLineBreak(bool isLastCharCarriageReturn)
        {
            var response = (IList<byte>)_currentResponse;
            int position;
            if (isLastCharCarriageReturn && response[_currentPosition] == RedisProtocolContants.LineFeed)
            {
                position = _currentPosition;
            }
            else
            {
                position = GetLineBreak(response, _currentPosition);
            }

            return position;
        }

        private byte[] GetByteSection(int count)
        {
            var value = new byte[count];
            Array.Copy(_currentResponse.Array, _currentResponse.Offset + _currentPosition, value, 0, count);
            _currentPosition += count;
            return value;
        }

        private int GetLineBreak(IList<byte> response, int startIndex)
        {
            var length = _currentResponse.Count;
            for (var i = startIndex; i < length - 1; i++)
            {
                if (response[i] == RedisProtocolContants.CarriageReturn
                    && response[i + 1] == RedisProtocolContants.LineFeed)
                {
                    return i;
                }
            }

            return -1;
        }

        private async Task ReadResponseAsync()
        {
            if (_currentPosition < _currentResponse.Count)
            {
                return;
            }

            await ReadNextResponseAsync().ConfigureAwait(false);
        }

        private async Task ReadNextResponseAsync()
        {
            var buffer = await _bufferManager.CheckOutAsync().ConfigureAwait(false);
            _bufferList.Add(buffer);
            _currentResponse = await _asyncSocket.ReceiveAsync(buffer);
            _currentPosition = 0;
        }
    }
}