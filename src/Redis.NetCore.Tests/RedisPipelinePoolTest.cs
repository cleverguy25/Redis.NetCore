using NSubstitute;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Redis.NetCore.Constants;
using Redis.NetCore.Pipeline;
using Redis.NetCore.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Unit")]
    public class RedisPipelinePoolTest
    {
        [Fact]
        public async Task RepairPipelineTest()
        {
            var socket = Substitute.For<IAsyncSocket>();
            var pipelinePool = SetupRedisPipelinePool(_ => socket);

            TestClient.SetupSocketResponse(socket, string.Empty, string.Empty);

            IRedisPipeline pipeline;
            try
            {
                pipeline = await pipelinePool.GetPipelineAsync();
                await pipeline.SendCommandAsync(RedisCommands.Ping);
            }
            catch (Exception)
            {
                // ignore
            }

            socket = Substitute.For<IAsyncSocket>();
            TestClient.SetupSocketResponse(socket, "$2\r\nOK\r\n");

            pipeline = await pipelinePool.GetPipelineAsync();
            await pipeline.SendCommandAsync(RedisCommands.Ping);
        }

        [Fact]
        public async Task PipelineKeepaliveTest()
        {
            var socket = Substitute.For<IAsyncSocket>();
            var pipelinePool = SetupRedisPipelinePool(_ => socket);

            TestClient.SetupSocketResponse(socket, "$2\r\nOK\r\n", "$2\r\nOK\r\n");

            var pipeline = await pipelinePool.GetPipelineAsync();
            await pipeline.SendCommandAsync(RedisCommands.Get);
            await Task.Delay(30000);
            var duration = pipeline.DurationFromLastCommand;
            Assert.True(duration < 15000);
        }

        private static RedisPipelinePool SetupRedisPipelinePool(Func<EndPoint, IAsyncSocket> socketFactory)
        {
            var bufferManager = new BufferManager(15, 8192, 10, 10);
            var redisConfiguration = new RedisConfiguration
            {
                Endpoints = new[] { "localhost:32768" }
            };
            var pipelinePool = new RedisPipelinePool(redisConfiguration, bufferManager, socketFactory, 1);
            return pipelinePool;
        }
    }
}
