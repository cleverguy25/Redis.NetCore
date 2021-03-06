﻿// <copyright file="AsyncSocketStream.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.NetCore.Sockets
{
    public class AsyncSocketStream : Stream
    {
        private readonly IAsyncSocket _socket;

        public AsyncSocketStream(IAsyncSocket socket)
        {
            _socket = socket;
        }

        public override bool CanRead { get; } = true;

        public override bool CanSeek { get; } = false;

        public override bool CanWrite { get; } = true;

        public override long Length
        {
            get { throw new NotSupportedException($"{nameof(Length)} is not supported."); }
        }

        public override long Position
        {
            get { throw new NotSupportedException($"{nameof(Position)} is not supported."); }

            set { throw new NotSupportedException($"{nameof(Position)} is not supported."); }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var list = new List<ArraySegment<byte>> { new ArraySegment<byte>(buffer, offset, count) };
            return _socket.Receive(list);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var bufferSegment = new ArraySegment<byte>(buffer, offset, count);
            var receiveBuffer = await _socket.ReceiveAsync(bufferSegment);
            return receiveBuffer.Count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException($"{nameof(Seek)} is not supported.");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException($"{nameof(SetLength)} is not supported.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var list = new List<ArraySegment<byte>> { new ArraySegment<byte>(buffer, offset, count) };
            _socket.Send(list);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var bufferSegment = new ArraySegment<byte>(buffer, offset, count);
            await _socket.SendAsync(bufferSegment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _socket.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}