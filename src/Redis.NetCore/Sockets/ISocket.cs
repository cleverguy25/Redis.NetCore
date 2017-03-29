using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public interface ISocket : IDisposable
    {
        Socket UnderlyingSocket { get; }

        bool SendAsync(SocketAsyncEventArgs args);

        int Send(IList<ArraySegment<byte>> buffers);

        bool ReceiveAsync(SocketAsyncEventArgs args);

        int Receive(IList<ArraySegment<byte>> buffers);

        bool ConnectAsync(SocketAsyncEventArgs args);
    }
}