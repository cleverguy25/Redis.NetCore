using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public class BufferManager : IBufferManager
    {
        private readonly ConcurrentStack<ArraySegment<byte>> _buffers;

        private readonly int _maxPoolSize;
        private readonly int _segmentChunks;
        private readonly ConcurrentBag<byte[]> _segments;
        private readonly int _segmentSize;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly object _lockObject = new object();
        private int _poolSize;

        public BufferManager(int segmentChunks, int chunkSize, int initialCount, int maxCount)
        {
            if (initialCount > maxCount)
            {
                throw new ArgumentException(
                                            $"Buffer Manager intial count [{initialCount}] cannot be greater than max count [{maxCount}].",
                                            nameof(initialCount));
            }

            _segmentChunks = segmentChunks;
            ChunkSize = chunkSize;
            _maxPoolSize = maxCount;

            _segmentSize = segmentChunks * chunkSize;
            _buffers = new ConcurrentStack<ArraySegment<byte>>();
            _segments = new ConcurrentBag<byte[]>();
            for (var i = 0; i < initialCount; i++)
            {
                CreateNewSegment();
            }
        }

        public int AvailableBuffers => _buffers.Count;

        public int TotalBufferSize => _segments.Count * _segmentSize;

        public int ChunkSize { get; }

        public async Task<ArraySegment<byte>> CheckOutAsync(int timeout = 10000)
        {
            while (true)
            {
                ArraySegment<byte> bytes;
                if (_buffers.TryPop(out bytes))
                {
                    return bytes;
                }

                if (CreateNewSegment())
                {
                    continue;
                }

                if (await _semaphore.WaitAsync(timeout).ConfigureAwait(false) == false)
                {
                    throw new TimeoutException($"Timeout in BufferManager, cannot increase pool beyond max size [{_maxPoolSize}]");
                }
            }
        }

        public void CheckIn(ArraySegment<byte> buffer)
        {
            _buffers.Push(buffer);

            if (_semaphore.CurrentCount != 0)
            {
                return;
            }

            lock (_lockObject)
            {
                if (_semaphore.CurrentCount == 0)
                {
                    _semaphore.Release();
                }
            }
        }

        private bool CreateNewSegment()
        {
            if (Interlocked.Increment(ref _poolSize) > _maxPoolSize)
            {
                return false;
            }

            var bytes = new byte[_segmentSize];
            _segments.Add(bytes);
            for (var i = 0; i < _segmentChunks; i++)
            {
                var chunk = new ArraySegment<byte>(bytes, i * ChunkSize, ChunkSize);
                _buffers.Push(chunk);
            }

            return true;
        }
    }
}