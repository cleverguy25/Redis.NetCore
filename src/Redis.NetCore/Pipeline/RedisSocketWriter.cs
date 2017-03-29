// <copyright file="RedisSocketWriter.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisSocketWriter : RedisBaseWriter
    {
        private readonly IAsyncSocket _socket;

        public RedisSocketWriter(IAsyncSocket socket, IBufferManager bufferManager)
            : base(bufferManager)
        {
            _socket = socket;
        }

        public override async Task FlushWriteBufferAsync()
        {
            if (BytesInBuffer > 0)
            {
                var bufferList = FlushBuffers();
                await _socket.SendAsync(bufferList);
                CheckInBuffers();
            }
        }
    }
}