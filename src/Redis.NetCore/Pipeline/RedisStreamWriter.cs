// <copyright file="RedisStreamWriter.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public class RedisStreamWriter : RedisBaseWriter
    {
        private readonly Stream _stream;

        public RedisStreamWriter(Stream stream, IBufferManager bufferManager)
            : base(bufferManager)
        {
            _stream = stream;
        }

        public override async Task FlushWriteBufferAsync()
        {
            if (BytesInBuffer > 0)
            {
                var bufferList = FlushBuffers();
                foreach (var buffer in bufferList)
                {
                    await _stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count, CancellationToken.None).ConfigureAwait(false);
                }

                CheckInBuffers();
            }
        }
    }
}