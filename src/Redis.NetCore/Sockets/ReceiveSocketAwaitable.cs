// <copyright file="ReceiveSocketAwaitable.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class ReceiveSocketAwaitable : SocketAwaitable, ISocketAwaitable<ArraySegment<byte>>
    {
        public ReceiveSocketAwaitable(SocketAsyncEventArgs eventArgs)
            : base(eventArgs)
        {
        }

        public ArraySegment<byte> GetResult()
        {
            ThrowOnSocketError();

            return new ArraySegment<byte>(EventArgs.Buffer, EventArgs.Offset, EventArgs.BytesTransferred);
        }

        public ISocketAwaitable<ArraySegment<byte>> GetAwaiter()
        {
            return this;
        }
    }
}