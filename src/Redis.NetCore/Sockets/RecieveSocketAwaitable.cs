using System;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class ReceiveSocketAwaitable : SocketAwaitable, ISocketAwaitable<ArraySegment<byte>>
    {
        public ReceiveSocketAwaitable(SocketAsyncEventArgs eventArgs) : base(eventArgs)
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