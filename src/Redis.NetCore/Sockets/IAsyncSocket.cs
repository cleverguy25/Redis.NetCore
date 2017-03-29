// <copyright file="IAsyncSocket.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace Redis.NetCore.Sockets
{
    public interface IAsyncSocket : IDisposable
    {
        ISocketAwaitable<bool> ConnectAsync();

        ISocketAwaitable<int> SendAsync(IList<ArraySegment<byte>> bufferList);

        ISocketAwaitable<int> SendAsync(ArraySegment<byte> buffer);

        int Send(IList<ArraySegment<byte>> buffers);

        int Receive(IList<ArraySegment<byte>> buffers);

        ISocketAwaitable<ArraySegment<byte>> ReceiveAsync(ArraySegment<byte> buffer);
    }
}