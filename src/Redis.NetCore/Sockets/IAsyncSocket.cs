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