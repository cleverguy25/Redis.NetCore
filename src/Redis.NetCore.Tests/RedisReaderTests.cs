// <copyright file="RedisReaderTests.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
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
            TestClient.SetupSocketResponse(socket, "$5\r", "\nBoo", "m!\r\n");

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
            TestClient.SetupSocketResponse(socket, "$", "5\r\nBoom!", "\r\n");

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
            TestClient.SetupSocketResponse(socket, "*2\r\n", "$5\r\nBoom!\r\n$", "10\r\nShakalaka!", "\r\n");

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
            TestClient.SetupSocketResponse(socket, "$5\r\nBoom!\r\n");

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
            TestClient.SetupSocketResponse(socket, "$-1\r\n");

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
            TestClient.SetupSocketResponse(socket, "+Boom!\r\n");

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
            TestClient.SetupSocketResponse(socket, ":42\r\n");

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
            TestClient.SetupSocketResponse(socket, "*2\r\n$5\r\nBoom!\r\n$10\r\nShakalaka!\r\n");

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
            TestClient.SetupSocketResponse(socket, "*2\r\n$5\r\nBoom!\r\n$-1\r\n");

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
            TestClient.SetupSocketResponse(socket, "-ERR Oh no.\r\n");

            await redisReader.ReadAsync(redisPiplineItem);
            Assert.Equal("ERR Oh no.", exception.Message);
        }

        private static RedisBaseReader CreateRedisReader(int chunkSize, IAsyncSocket socket)
        {
            var bufferManager = new BufferManager(2, chunkSize, 2, 10);
            var redisReader = new RedisSocketReader(socket, bufferManager);
            return redisReader;
        }
    }
}
