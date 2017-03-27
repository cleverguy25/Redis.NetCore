using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Redis.NetCore.Configuration;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisPipelinePool : IRedisPipelinePool
    {
        private readonly IBufferManager _bufferManager;
        private readonly int _capacity;
        private readonly RedisConfiguration _configuration;
        private readonly int _maxIndex;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private int _currentIndex;
        private List<Task<IRedisPipeline>> _pipelines;

        public RedisPipelinePool(RedisConfiguration configuration, IBufferManager bufferManager, int capacity = 3)
        {
            _configuration = configuration;
            _bufferManager = bufferManager;
            _capacity = capacity;
            _maxIndex = _capacity - 1;
        }

        public Task<IRedisPipeline> GetPipelineAsync()
        {
            if (_pipelines == null)
            {
                return InitializeAsync().Unwrap();
            }

            return GetNextPipelineAsync();
        }

        public void Dispose()
        {
            if (_pipelines == null)
            {
                return;
            }

            foreach (var pipelineTask in _pipelines)
            {
                pipelineTask.Result.Dispose();
            }
        }

        private async Task<Task<IRedisPipeline>> InitializeAsync()
        {
            if (_pipelines != null)
            {
                return GetNextPipelineAsync();
            }

            await _semaphore.WaitAsync().ConfigureAwait(false);
            return await InitializePipelinesAsync().ConfigureAwait(false);
        }

        private Task<IRedisPipeline> GetNextPipelineAsync()
        {
            while (true)
            {
                var currentIndex = GetPipelineIndex();
                var pipelineTask = _pipelines[currentIndex];
                if (pipelineTask == null)
                {
                    continue;
                }

                if (pipelineTask.Result.IsErrorState == false)
                {
                    return pipelineTask;
                }

                _pipelines[currentIndex] = null;
                return ReplaceErrorPipelineAsync(currentIndex, pipelineTask.Result);
            }
        }

        private async Task<Task<IRedisPipeline>> InitializePipelinesAsync()
        {
            try
            {
                if (_pipelines != null)
                {
                    return GetNextPipelineAsync();
                }

                var endpoint = await ResolveEndpointAsync().ConfigureAwait(false);
                var pipelines = new List<Task<IRedisPipeline>>();
                for (var i = 0; i < _capacity; i++)
                {
                    var pipeline = await ConnectPipelineAsync(i, endpoint, _configuration.Password).ConfigureAwait(false);
                    pipelines.Add(Task.FromResult(pipeline));
                }

                _pipelines = pipelines;

                return GetNextPipelineAsync();
            }
            catch (Exception error)
            {
                throw new RedisException("Error connecting to server.", error);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<IRedisPipeline> ReplaceErrorPipelineAsync(int currentIndex, IRedisPipeline pipeline)
        {
            var endpoint = await ResolveEndpointAsync().ConfigureAwait(false);
            var newPipeline = await ConnectPipelineAsync(currentIndex, endpoint, _configuration.Password).ConfigureAwait(false);
            _pipelines[currentIndex] = Task.FromResult(newPipeline);

            try
            {
                pipeline.ThrowErrorForRemainingResponseQueueItems();
                pipeline.SaveQueue(newPipeline);
                pipeline.Dispose();
            }
            catch (Exception)
            {
                // Ignore
            }

            return newPipeline;
        }

        private async Task<IRedisPipeline> ConnectPipelineAsync(int index, EndPoint endpoint, string password)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var asyncSocket = new AsyncSocket(socket.Wrap(), endpoint);
            await asyncSocket.ConnectAsync();
            var stream = new AsyncSocketStream(asyncSocket);
            var redisReader = new RedisStreamReader(stream, _bufferManager);
            var redisWriter = new RedisStreamWriter(stream, _bufferManager);
            var pipeline =  new RedisPipeline(index, asyncSocket, stream, redisWriter, redisReader);
            if (string.IsNullOrWhiteSpace(password) == false)
            {
                await pipeline.AuthenticateAsync(password);
            }

            return pipeline;
        }

        private async Task<EndPoint> ResolveEndpointAsync()
        {
            var endpointConfiguration = _configuration.Endpoints[0];
            var endpoint = await EndpointResolution.GetEndpointAsync(endpointConfiguration).ConfigureAwait(false);
            return endpoint;
        }

        private int GetPipelineIndex()
        {
            int currentIndex, nextIndex;
            do
            {
                currentIndex = _currentIndex;
                nextIndex = currentIndex == _maxIndex ? 0 : currentIndex + 1;
            } while (Interlocked.CompareExchange(ref _currentIndex, nextIndex, currentIndex) != currentIndex);

            return currentIndex;
        }
    }
}