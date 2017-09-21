// <copyright file="RedisSocketReader.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Redis.NetCore.Sockets;

namespace Redis.NetCore.Pipeline
{
    public class RedisSocketReader : RedisBaseReader
    {
        private readonly IAsyncSocket _asyncSocket;

        public RedisSocketReader(IAsyncSocket asyncSocket, IBufferManager bufferManager)
            : base(bufferManager)
        {
            _asyncSocket = asyncSocket;
        }

        protected override async Task ReadNextResponseAsync()
        {
            if (BufferList.Count > 10)
            {
                CheckInBuffers();
            }

            if (_asyncSocket.Connected == false)
            {
                throw new RedisException("Socket is not connected.");
            }

            var buffer = await BufferManager.CheckOutAsync().ConfigureAwait(false);
            BufferList.Add(buffer);
            CurrentResponse = await _asyncSocket.ReceiveAsync(buffer);
            CurrentPosition = 0;
        }
    }
}