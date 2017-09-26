// <copyright file="RedisClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Redis.NetCore.Pipeline;
using Redis.NetCore.Sockets;
using System.Net;
using System.Net.Sockets;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisClient
    {
        private readonly IRedisPipelinePool _redisPipelinePool;

        public RedisClient(IRedisPipelinePool redisPipelinePool)
        {
            _redisPipelinePool = redisPipelinePool;
        }

        public static IRedisClient CreateClient(RedisConfiguration redisConfiguration, int numConnections = 3, int bufferSize = 8192, int maxBuffers = 1000, int socketTimeout = 60000, bool keepAlive = true)
        {
            var bufferManager = new BufferManager(15, bufferSize, 10, maxBuffers);
            var pipelinePool = new RedisPipelinePool(redisConfiguration, bufferManager, SocketFactory, numConnections, socketTimeout, keepAlive);
            return new RedisClient(pipelinePool);
        }

        public void Dispose()
        {
            _redisPipelinePool?.Dispose();
        }

        private static IAsyncSocket SocketFactory(EndPoint endpoint)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            return new AsyncSocket(socket.Wrap(), endpoint);
        }
    }
}
