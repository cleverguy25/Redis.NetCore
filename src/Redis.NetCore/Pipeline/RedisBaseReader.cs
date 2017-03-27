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
    public abstract class RedisBaseReader : IRedisReader
    {
        protected readonly List<ArraySegment<byte>> _bufferList = new List<ArraySegment<byte>>();
        protected readonly object _lockObject = new object();
        protected readonly IBufferManager BufferManager;
        protected int CurrentPosition;
        protected ArraySegment<byte> CurrentResponse;

        protected RedisBaseReader(IBufferManager bufferManager)
        {
            BufferManager = bufferManager;
        }

        public async Task ReadAsync(RedisPipelineItem redisItem)
        {
            // Everything is inline to prevent unncessary task allocations
            // this code is called A LOT
            await ReadResponseAsync().ConfigureAwait(false);

            if (CurrentPosition >= CurrentResponse.Count)
            {
                await ReadNextResponseAsync().ConfigureAwait(false);
            }

            var firstChar = ReadFirstChar();
            if (firstChar == RedisProtocolContants.SimpleString)
            {
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)CurrentResponse;
                var currentChar = response[CurrentPosition];
                while (currentChar != RedisProtocolContants.LineFeed)
                {
                    if (currentChar != RedisProtocolContants.CarriageReturn)
                    {
                        bytes.Add(currentChar);
                    }

                    CurrentPosition++;
                    if (CurrentPosition >= CurrentResponse.Count)
                    {
                        await ReadNextResponseAsync().ConfigureAwait(false);
                        response = CurrentResponse;
                    }

                    currentChar = response[CurrentPosition];
                }

                CurrentPosition++;
                ProcessValue(redisItem, bytes.ToArray());
            }
            else if (firstChar == RedisProtocolContants.Integer)
            {
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)CurrentResponse;
                var currentChar = response[CurrentPosition];
                while (currentChar != RedisProtocolContants.LineFeed)
                {
                    if (currentChar != RedisProtocolContants.CarriageReturn)
                    {
                        bytes.Add(currentChar);
                    }

                    CurrentPosition++;
                    if (CurrentPosition >= CurrentResponse.Count)
                    {
                        await ReadNextResponseAsync().ConfigureAwait(false);
                        response = CurrentResponse;
                    }

                    currentChar = response[CurrentPosition];
                }

                CurrentPosition++;
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
                var response = (IList<byte>)CurrentResponse;
                for (var i = 0; i < length; i++)
                {
                    if (CurrentPosition >= CurrentResponse.Count)
                    {
                        await ReadNextResponseAsync().ConfigureAwait(false);
                        response = CurrentResponse;
                    }

                    var currentChar = response[CurrentPosition];
                    bytes.Add(currentChar);
                    CurrentPosition++;
                }

                for (var i = 0; i < 2; i++)
                {
                    if (CurrentPosition >= CurrentResponse.Count)
                    {
                        await ReadNextResponseAsync().ConfigureAwait(false);
                    }

                    CurrentPosition++;
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
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                }

                var bytes = new List<byte>();
                var response = (IList<byte>)CurrentResponse;
                var currentChar = response[CurrentPosition];
                while (currentChar != RedisProtocolContants.LineFeed)
                {
                    if (currentChar != RedisProtocolContants.CarriageReturn)
                    {
                        bytes.Add(currentChar);
                    }

                    CurrentPosition++;
                    if (CurrentPosition >= CurrentResponse.Count)
                    {
                        await ReadNextResponseAsync().ConfigureAwait(false);
                        response = CurrentResponse;
                    }

                    currentChar = response[CurrentPosition];
                }

                CurrentPosition++;
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
                    BufferManager.CheckIn(buffer);
                }

                _bufferList.Clear();
            }
        }

        private static void ProcessValue(RedisPipelineItem redisItem, byte[] value)
        {
            if (value == null)
            {
                redisItem.OnSuccess(null);
                return;
            }

            var result = new byte[1][];
            result[0] = value;
            redisItem.OnSuccess(result);
        }
        
        private byte ReadFirstChar()
        {
            var firstChar = CurrentResponse.Array[CurrentResponse.Offset + CurrentPosition];
            CurrentPosition++;
            return firstChar;
        }

        private async Task<byte[][]> ReadArrayAsync()
        {
            var length = await ReadLengthAsync().ConfigureAwait(false);
            var bytes = new List<byte[]>();
            for (var i = 0; i < length; i++)
            {
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                }

                var firstChar = ReadFirstChar();
                if (firstChar == RedisProtocolContants.SimpleString)
                {
                    if (CurrentPosition >= CurrentResponse.Count)
                    {
                        await ReadNextResponseAsync().ConfigureAwait(false);
                    }

                    var stringBytes = new List<byte>();
                    var response = (IList<byte>)CurrentResponse;
                    var currentChar = response[CurrentPosition];
                    while (currentChar != RedisProtocolContants.LineFeed)
                    {
                        if (currentChar != RedisProtocolContants.CarriageReturn)
                        {
                            stringBytes.Add(currentChar);
                        }

                        CurrentPosition++;
                        if (CurrentPosition >= CurrentResponse.Count)
                        {
                            await ReadNextResponseAsync().ConfigureAwait(false);
                            response = CurrentResponse;
                        }

                        currentChar = response[CurrentPosition];
                    }

                    CurrentPosition++;
                    bytes.Add(stringBytes.ToArray());
                }
                else if (firstChar == RedisProtocolContants.BulkString)
                {
                    var bulkLength = await ReadLengthAsync().ConfigureAwait(false);
                    if (bulkLength == -1)
                    {
                        bytes.Add(null);
                        continue;
                    }

                    var bulkBytes = new List<byte>();
                    var response = (IList<byte>)CurrentResponse;
                    for (var currentIndex = 0; currentIndex < bulkLength; currentIndex++)
                    {
                        if (CurrentPosition >= CurrentResponse.Count)
                        {
                            await ReadNextResponseAsync().ConfigureAwait(false);
                            response = CurrentResponse;
                        }

                        var currentChar = response[CurrentPosition];
                        bulkBytes.Add(currentChar);
                        CurrentPosition++;
                    }

                    for (var currentIndex = 0; currentIndex < 2; currentIndex++)
                    {
                        if (CurrentPosition >= CurrentResponse.Count)
                        {
                            await ReadNextResponseAsync().ConfigureAwait(false);
                        }

                        CurrentPosition++;
                    }
                    
                    bytes.Add(bulkBytes.ToArray());
                }
                else if (firstChar == RedisProtocolContants.Array)
                {
                    var value = await ReadArrayAsync().ConfigureAwait(false);
                    var collapseBytes = new List<byte> { RedisProtocolContants.Array };
                    collapseBytes.AddRange(value.Length.ToString(CultureInfo.InvariantCulture).ToBytes());
                    collapseBytes.AddRange(RedisProtocolContants.LineEnding);
                    foreach (var byteArray in value)
                    {
                        collapseBytes.Add(RedisProtocolContants.BulkString);
                        collapseBytes.AddRange(byteArray.Length.ToString(CultureInfo.InvariantCulture).ToBytes());
                        collapseBytes.AddRange(RedisProtocolContants.LineEnding);
                        collapseBytes.AddRange(byteArray);
                        collapseBytes.AddRange(RedisProtocolContants.LineEnding);
                    }

                    bytes.Add(collapseBytes.ToArray());
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
            var response = (IList<byte>)CurrentResponse;
            for (var i = 0; i < length; i++)
            {
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                    response = CurrentResponse;
                }

                var currentChar = response[CurrentPosition];
                bytes.Add(currentChar);
                CurrentPosition++;
            }

            for (var i = 0; i < 2; i++)
            {
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                }

                CurrentPosition++;
            }

            return bytes.ToArray();
        }
        
        private async Task<int> ReadLengthAsync()
        {
            if (CurrentPosition >= CurrentResponse.Count)
            {
                await ReadNextResponseAsync().ConfigureAwait(false);
            }

            var response = (IList<byte>)CurrentResponse;
            var length = 0;
            var sign = 1;

            var currentChar = response[CurrentPosition];
            if (currentChar == RedisProtocolContants.Minus)
            {
                sign = -1;
                CurrentPosition++;
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                    response = CurrentResponse;
                }
            }

            currentChar = response[CurrentPosition];
            while (currentChar != RedisProtocolContants.LineFeed)
            {
                if (currentChar != RedisProtocolContants.CarriageReturn)
                {
                    length = length * 10 + currentChar - RedisProtocolContants.Zero;
                }
                
                CurrentPosition++;
                if (CurrentPosition >= CurrentResponse.Count)
                {
                    await ReadNextResponseAsync().ConfigureAwait(false);
                    response = CurrentResponse;
                }

                currentChar = response[CurrentPosition];
            }

            CurrentPosition++;
            return length * sign;
        }

        private async Task ReadResponseAsync()
        {
            if (CurrentPosition < CurrentResponse.Count)
            {
                return;
            }

            await ReadNextResponseAsync().ConfigureAwait(false);
        }

        protected abstract Task ReadNextResponseAsync();
    }
}