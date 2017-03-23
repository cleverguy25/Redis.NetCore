using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Redis.NetCore.Pipeline;
using Redis.NetCore.Sockets;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "UnitTest")]
    public class RedisReaderTests
    {
        [Fact]
        public async Task ReadBulkStringInAwkwardChunks1Async()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "$5\r", "\nBoo", "m!\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
        }

        [Fact]
        public async Task ReadBulkStringInAwkwardChunks2Async()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "$", "5\r\nBoom!", "\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
        }

        [Fact]
        public async Task ReadBulkStringInAwkwardChunks3Async()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "*2\r\n","$5\r\nBoom!\r\n$", "10\r\nShakalaka!", "\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
        }

        [Fact]
        public async Task ReadBulkStringAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();
            
            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "$5\r\nBoom!\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
        }

        [Fact]
        public async Task ReadBulkStringAsNullAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "$-1\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal(null, responseData);
        }

        [Fact]
        public async Task ReadSimpleStringAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "+Boom!\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
        }

        [Fact]
        public async Task ReadIntegerAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, ":42\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("42", Encoding.UTF8.GetString(responseData[0]));
        }

        [Fact]
        public async Task ReadArrayAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "*2\r\n$5\r\nBoom!\r\n$10\r\nShakalaka!\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
            Assert.Equal("Shakalaka!", Encoding.UTF8.GetString(responseData[1]));
        }

        [Fact]
        public async Task ReadArrayWithNullAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();

            byte[][] responseData = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, null, response => responseData = response);
            SetupSocketResponse(socket, "*2\r\n$5\r\nBoom!\r\n$-1\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("Boom!", Encoding.UTF8.GetString(responseData[0]));
            Assert.Equal(null, responseData[1]);
        }

        [Fact]
        public async Task ReadErrorAsync()
        {
            var socket = Substitute.For<IAsyncSocket>();

            Exception exception = null;
            var redisReader = CreateRedisReader(100, socket);
            var redisPiplineItem = new RedisPipelineItem(null, (error) => exception = error, null);
            SetupSocketResponse(socket, "-ERR Oh no.\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("ERR Oh no.", exception.Message);
        }

        private static void SetupSocketResponse(IAsyncSocket socket, params string[] dataString)
        {
            
            var awaitable = Substitute.For<ISocketAwaitable<ArraySegment<byte>>>();
            awaitable.GetAwaiter().Returns(awaitable);
            awaitable.IsCompleted.Returns(true);

            var i = 0;
            awaitable.GetResult().Returns(
                                          (context) =>
                                          {
                                              var data = new ArraySegment<byte>(dataString[i].ToBytes());
                                              i++;
                                              return data;
                                          });
            
            socket.ReceiveAsync(Arg.Any<ArraySegment<byte>>()).Returns(awaitable);
        }

        private static RedisReader CreateRedisReader(int chunkSize, IAsyncSocket socket)
        {
            var bufferManager = new BufferManager(2, chunkSize, 2, 10);
            var redisReader = new RedisReader(bufferManager, socket);
            return redisReader;
        }
    }
}
