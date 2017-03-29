// <copyright file="SocketWrapper.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    internal sealed class SocketWrapper : ISocket
    {
        public SocketWrapper(Socket socket)
        {
            UnderlyingSocket = socket;
        }

        public Socket UnderlyingSocket { get; }

        public int Send(IList<ArraySegment<byte>> buffers)
        {
            return UnderlyingSocket.Send(buffers);
        }

        public bool SendAsync(SocketAsyncEventArgs args)
        {
            return UnderlyingSocket.SendAsync(args);
        }

        public int Receive(IList<ArraySegment<byte>> buffers)
        {
            return UnderlyingSocket.Receive(buffers);
        }

        public bool ReceiveAsync(SocketAsyncEventArgs args)
        {
            return UnderlyingSocket.ReceiveAsync(args);
        }

        public bool ConnectAsync(SocketAsyncEventArgs args)
        {
            return UnderlyingSocket.ConnectAsync(args);
        }

        public void Dispose()
        {
            UnderlyingSocket.Dispose();
        }
    }
}