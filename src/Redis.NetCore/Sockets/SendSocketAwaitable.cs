using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class SendSocketAwaitable : SocketAwaitable, ISocketAwaitable<int>
    {
        public SendSocketAwaitable(SocketAsyncEventArgs eventArgs) : base(eventArgs)
        {
        }

        public int GetResult()
        {
            ThrowOnSocketError();

            return EventArgs.BytesTransferred;
        }

        public ISocketAwaitable<int> GetAwaiter()
        {
            return this;
        }
    }
}