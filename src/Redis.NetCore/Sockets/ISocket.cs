using System;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public interface ISocket : IDisposable
    {
        Socket UnderlyingSocket { get; }
        bool SendAsync(SocketAsyncEventArgs args);
        bool ReceiveAsync(SocketAsyncEventArgs args);
        bool ConnectAsync(SocketAsyncEventArgs args);
    }
}