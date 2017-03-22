using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisPipeline : IRedisPipeline
    {
        private readonly IRedisReader _redisReader;
        private readonly IRedisWriter _redisWriter;
        private readonly ConcurrentQueue<RedisPipelineItem> _responseQueue = new ConcurrentQueue<RedisPipelineItem>();
        private readonly int _pipelineId;
        private readonly IAsyncSocket _socket;
        private Exception _pipelineException;
        private int _sendIsRunning = 0;
        private int _receiveIsRunning = 0;
        private int _receiveCount = 0;
        private Task _sendTask;
        private Task _receiveTask;

        public RedisPipeline(int pipelineId, IAsyncSocket socket, IRedisWriter redisWriter, IRedisReader redisReader)
        {
            _pipelineId = pipelineId;
            _socket = socket;
            _redisWriter = redisWriter;
            _redisReader = redisReader;
        }

        public bool IsErrorState => _pipelineException != null;

        public ConcurrentQueue<RedisPipelineItem> RequestQueue { get; } = new ConcurrentQueue<RedisPipelineItem>();

        public Task<byte[]> SendCommandAsync(params byte[][] requestData)
        {
            var taskCompletion = new TaskCompletionSource<byte[]>();
            var request = new RedisPipelineItem(
                                                requestData,
                                                taskCompletion.SetException,
                                                result => taskCompletion.SetResult(result.CollapseArray()));

            RequestQueue.Enqueue(request);

            return EnsureSendCommandAsync(taskCompletion);
        }

        public Task<byte[][]> SendMultipleCommandAsync(params byte[][] requestData)
        {
            var taskCompletion = new TaskCompletionSource<byte[][]>();
            var request = new RedisPipelineItem(
                                                requestData,
                                                taskCompletion.SetException,
                                                result => taskCompletion.SetResult(result));
            RequestQueue.Enqueue(request);

            return EnsureSendCommandAsync(taskCompletion);
        }

        public void SaveQueue(IRedisPipeline redisPipeline)
        {
            RedisPipelineItem currentItem;
            while (RequestQueue.TryDequeue(out currentItem))
            {
                redisPipeline.RequestQueue.Enqueue(currentItem);
            }
        }

        public void ThrowErrorForRemainingResponseQueueItems()
        {
            RedisPipelineItem currentItem;
            while (_responseQueue.TryDequeue(out currentItem))
            {
                currentItem.OnError(_pipelineException);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        private Task<T> EnsureSendCommandAsync<T>(TaskCompletionSource<T> taskCompletion)
        {
            StartSendIfNotRunning();

            return taskCompletion.Task;
        }

        private Task StartSendIfNotRunning()
        {
            if (_pipelineException != null)
            {
                return Task.CompletedTask;
            }

            if (Interlocked.CompareExchange(ref _sendIsRunning, 1, 0) == 0)
            {
                _sendTask = Task.Run(SendAndMarkDone);
                return _sendTask;
            }

            return Task.CompletedTask;
        }

        private async Task SendAndMarkDone()
        {
            await SendAsync().ConfigureAwait(false);
            Interlocked.Exchange(ref _sendIsRunning, 0);
        }

        private async Task SendAsync()
        {
            try
            {
                await _redisWriter.CreateNewBufferAsync().ConfigureAwait(false);

                RedisPipelineItem currentItem;
                while (RequestQueue.TryDequeue(out currentItem))
                {
                    await _redisWriter.WriteRedisRequestAsync(currentItem.Data).ConfigureAwait(false);
                    _responseQueue.Enqueue(currentItem);
                    FireAndForget(StartReceiveIfNotRunning());
                    if (_redisWriter.BufferCount <= 1)
                    {
                        continue;
                    }

                    await FlushWriteBuffer().ConfigureAwait(false);
                    break;
                }
                
                await FlushWriteBuffer().ConfigureAwait(false);
            }
            catch (Exception error)
            {
                _pipelineException = error;
                ThrowErrorForRemainingResponseQueueItems();
            }
            finally
            {
                _redisWriter.CheckInBuffers();
            }
        }

        private static void FireAndForget(Task task)
        {
        }

        private async Task FlushWriteBuffer()
        {
            if (_redisWriter.BytesInBuffer > 0)
            {
                var bufferList = _redisWriter.FlushBuffers();
                await _socket.SendAsync(bufferList);
                _redisWriter.CheckInBuffers();
            }
        }

        private Task StartReceiveIfNotRunning()
        {
            if (_pipelineException != null)
            {
                return Task.CompletedTask;
            }

            if (Interlocked.CompareExchange(ref _receiveIsRunning, 1, 0) == 0)
            {
                _receiveTask = Task.Run(ReceiveAndMarkDone);
                return _receiveTask;
            }

            return Task.CompletedTask;
        }

        private async Task ReceiveAndMarkDone()
        {
            await ReceiveAsync().ConfigureAwait(false);
            Interlocked.Exchange(ref _receiveIsRunning, 0);
        }

        private async Task ReceiveAsync()
        {
            if (_pipelineException != null)
            {
                ThrowErrorForRemainingResponseQueueItems();
                return;
            }

            try
            {
                RedisPipelineItem currentItem;
                while (_responseQueue.TryDequeue(out currentItem))
                {
                    var count = Interlocked.Increment(ref _receiveCount);
                    Debug.WriteLine($"Receive count {count}");
                    await _redisReader.ReadAsync(currentItem).ConfigureAwait(false);
                }
            }
            catch (Exception error)
            {
                _pipelineException = error;
                ThrowErrorForRemainingResponseQueueItems();
            }
            finally
            {
                _redisReader.CheckInBuffers();
            }
        }
    }
}