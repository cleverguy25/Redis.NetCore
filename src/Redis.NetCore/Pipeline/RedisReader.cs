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
            // Everything is inline to prevent unncessary task allocations
            // this code is called A LOT
            await ReadResponseAsync().ConfigureAwait(false);

            if (_currentPosition >= _currentResponse.Count)
            {
                await ReadNextResponseAsync().ConfigureAwait(false);
            }

            var firstChar = ReadFirstChar();
            if (firstChar == RedisProtocolContants.SimpleString)
            {
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)_currentResponse;
                var currentChar = response[_currentPosition];
                while (currentChar != RedisProtocolContants.LineFeed)
                {
                    if (currentChar != RedisProtocolContants.CarriageReturn)
                    {
                        bytes.Add(currentChar);
                    }

                    _currentPosition++;
                    if (_currentPosition >= _currentResponse.Count)
                    {
                        await ReadNextResponseAsync();
                        response = _currentResponse;
                    }

                    currentChar = response[_currentPosition];
                }

                _currentPosition++;
                ProcessValue(redisItem, bytes.ToArray());
            }
            else if (firstChar == RedisProtocolContants.Integer)
            {
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)_currentResponse;
                var currentChar = response[_currentPosition];
                while (currentChar != RedisProtocolContants.LineFeed)
                {
                    if (currentChar != RedisProtocolContants.CarriageReturn)
                    {
                        bytes.Add(currentChar);
                    }

                    _currentPosition++;
                    if (_currentPosition >= _currentResponse.Count)
                    {
                        await ReadNextResponseAsync();
                        response = _currentResponse;
                    }

                    currentChar = response[_currentPosition];
                }

                _currentPosition++;
                ProcessValue(redisItem, bytes.ToArray());
            }
            else if (firstChar == RedisProtocolContants.BulkString)
            {
                var length = await ReadLengthAsync().ConfigureAwait(false);
                if (length == -1)
                {
                    ProcessValue(redisItem, null);
                    return;
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)_currentResponse;
                for (var i = 0; i < length; i++)
                {
                    if (_currentPosition >= _currentResponse.Count)
                    {
                        await ReadNextResponseAsync();
                        response = _currentResponse;
                    }

                    var currentChar = response[_currentPosition];
                    bytes.Add(currentChar);
                    _currentPosition++;
                }

                for (var i = 0; i < 2; i++)
                {
                    if (_currentPosition >= _currentResponse.Count)
                    {
                        await ReadNextResponseAsync();
                    }

                    _currentPosition++;
                }

                ProcessValue(redisItem, bytes?.ToArray());
            }
            else if (firstChar == RedisProtocolContants.Array)
            {
                var value = await ReadArrayAsync().ConfigureAwait(false);
                redisItem.OnSuccess(value);
            }
            else if (firstChar == RedisProtocolContants.Error)
            {
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)_currentResponse;
                var currentChar = response[_currentPosition];
                while (currentChar != RedisProtocolContants.LineFeed)
                {
                    if (currentChar != RedisProtocolContants.CarriageReturn)
                    {
                        bytes.Add(currentChar);
                    }

                    _currentPosition++;
                    if (_currentPosition >= _currentResponse.Count)
                    {
                        await ReadNextResponseAsync();
                        response = _currentResponse;
                    }

                    currentChar = response[_currentPosition];
                }

                _currentPosition++;
                var errorText = Encoding.UTF8.GetString(bytes.ToArray());
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

        private static void ProcessValue(RedisPipelineItem redisItem, byte[] value)
        {
            var result = new byte[1][];
            result[0] = value;
            redisItem.OnSuccess(result);
        }
        
        private byte ReadFirstChar()
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

                var firstChar = ReadFirstChar();
                if (firstChar == RedisProtocolContants.SimpleString)
                {
                    if (_currentPosition >= _currentResponse.Count)
                    {
                        await ReadNextResponseAsync();
                    }

                    var stringBytes = new List<byte>();
                    var response = (IList<byte>)_currentResponse;
                    var currentChar = response[_currentPosition];
                    while (currentChar != RedisProtocolContants.LineFeed)
                    {
                        if (currentChar != RedisProtocolContants.CarriageReturn)
                        {
                            stringBytes.Add(currentChar);
                        }

                        _currentPosition++;
                        if (_currentPosition >= _currentResponse.Count)
                        {
                            await ReadNextResponseAsync();
                            response = _currentResponse;
                        }

                        currentChar = response[_currentPosition];
                    }

                    _currentPosition++;
                    bytes.Add(stringBytes.ToArray());
                }
                else if (firstChar == RedisProtocolContants.BulkString)
                {
                    var value = await ReadBulkStringAsync().ConfigureAwait(false);
                    bytes.Add(value);
                }
            }

            return bytes.ToArray();
        }

        private async Task<byte[]> ReadBulkStringAsync()
        {
            var length = await ReadLengthAsync().ConfigureAwait(false);
            if (length == -1)
            {
                return null;
            }

            var bytes = new List<byte>();
            var response = (IList<byte>)_currentResponse;
            for (var i = 0; i < length; i++)
            {
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                    response = _currentResponse;
                }

                var currentChar = response[_currentPosition];
                bytes.Add(currentChar);
                _currentPosition++;
            }

            for (var i = 0; i < 2; i++)
            {
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                }

                _currentPosition++;
            }

            return bytes.ToArray();
        }
        
        private async Task<int> ReadLengthAsync()
        {
            if (_currentPosition >= _currentResponse.Count)
            {
                await ReadNextResponseAsync();
            }

            var response = (IList<byte>)_currentResponse;
            var length = 0;
            var sign = 1;

            var currentChar = response[_currentPosition];
            if (currentChar == RedisProtocolContants.Minus)
            {
                sign = -1;
                _currentPosition++;
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                    response = _currentResponse;
                }
            }

            currentChar = response[_currentPosition];
            while (currentChar != RedisProtocolContants.LineFeed)
            {
                if (currentChar != RedisProtocolContants.CarriageReturn)
                {
                    length = length * 10 + currentChar - RedisProtocolContants.Zero;
                }
                
                _currentPosition++;
                if (_currentPosition >= _currentResponse.Count)
                {
                    await ReadNextResponseAsync();
                    response = _currentResponse;
                }

                currentChar = response[_currentPosition];
            }

            _currentPosition++;
            return length * sign;
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