using System;
using System.Collections.Generic;

namespace Redis.NetCore.Sockets
{
    public interface IAsyncSocket : IDisposable
    {
        ISocketAwaitable<bool> ConnectAsync();

        ISocketAwaitable<int> SendAsync(IList<ArraySegment<byte>> bufferList);

        ISocketAwaitable<ArraySegment<byte>> ReceiveAsync(ArraySegment<byte> buffer);
    }
}