// <copyright file="ISocket.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public interface ISocket : IDisposable
    {
        Socket UnderlyingSocket { get; }

        bool Connected { get; }

        bool SendAsync(SocketAsyncEventArgs args);

        int Send(IList<ArraySegment<byte>> buffers);

        bool ReceiveAsync(SocketAsyncEventArgs args);

        int Receive(IList<ArraySegment<byte>> buffers);

        bool ConnectAsync(SocketAsyncEventArgs args);
    }
}