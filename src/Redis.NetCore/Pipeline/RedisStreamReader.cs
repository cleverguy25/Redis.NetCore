// <copyright file="RedisStreamReader.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public class RedisStreamReader : RedisBaseReader
    {
        private readonly Stream _stream;

        public RedisStreamReader(Stream stream, IBufferManager bufferManager)
            : base(bufferManager)
        {
            _stream = stream;
        }

        protected override async Task ReadNextResponseAsync()
        {
            if (BufferList.Count > 10)
            {
                CheckInBuffers();
            }

            var buffer = await BufferManager.CheckOutAsync().ConfigureAwait(false);
            BufferList.Add(buffer);
            var bytesRead = await _stream.ReadAsync(buffer.Array, buffer.Offset, buffer.Count).ConfigureAwait(false);
            CurrentResponse = new ArraySegment<byte>(buffer.Array, buffer.Offset, bytesRead);
            CurrentPosition = 0;
        }
    }
}