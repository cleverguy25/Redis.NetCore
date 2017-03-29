using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
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

                var endpoint = await EndpointResolution.GetEndpointAsync(_configuration.Endpoints[0]).ConfigureAwait(false);
                var pipelines = new List<Task<IRedisPipeline>>();
                for (var i = 0; i < _capacity; i++)
                {
                    var pipeline = await ConnectPipelineAsync(i, endpoint.Item1, endpoint.Item2).ConfigureAwait(false);
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
            var endpoint = await EndpointResolution.GetEndpointAsync(_configuration.Endpoints[0]).ConfigureAwait(false);
            var newPipeline = await ConnectPipelineAsync(currentIndex, endpoint.Item1, endpoint.Item2).ConfigureAwait(false);
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

        private async Task<IRedisPipeline> ConnectPipelineAsync(int index, string host, EndPoint endpoint)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var asyncSocket = new AsyncSocket(socket.Wrap(), endpoint);
            await asyncSocket.ConnectAsync();
            var pipeline = await CreatePipelineAsync(index, host, asyncSocket).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(_configuration.Password) == false)
            {
                await pipeline.AuthenticateAsync(_configuration.Password).ConfigureAwait(false);
            }

            return pipeline;
        }

        private Task<IRedisPipeline> CreatePipelineAsync(int index, string host, AsyncSocket asyncSocket)
        {
            return _configuration.UseSsl ? 
                CreateSslPipelineAsync(index, host, asyncSocket) : Task.FromResult(CreateSocketPipeline(index, asyncSocket));
        }

        private IRedisPipeline CreateSocketPipeline(int index, AsyncSocket asyncSocket)
        {
            var redisReader = new RedisSocketReader(asyncSocket, _bufferManager);
            var redisWriter = new RedisSocketWriter(asyncSocket, _bufferManager);
            return new RedisPipeline(index, asyncSocket, null, redisWriter, redisReader);
        }

        private async Task<IRedisPipeline> CreateSslPipelineAsync(int index, string host, AsyncSocket asyncSocket)
        {
            var stream = new AsyncSocketStream(asyncSocket);
            var sslStream = new SslStream(stream);
            var passThrough = new PassThroughStream(sslStream); // required for .net 4.62
            await sslStream.AuthenticateAsClientAsync(host).ConfigureAwait(false);
            var redisReader = new RedisStreamReader(passThrough, _bufferManager);
            var redisWriter = new RedisStreamWriter(passThrough, _bufferManager);
            return new RedisPipeline(index, asyncSocket, passThrough, redisWriter, redisReader);
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