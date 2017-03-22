using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class ConnectSocketAwaitable : SocketAwaitable, ISocketAwaitable<bool>
    {
        public ConnectSocketAwaitable(SocketAsyncEventArgs eventArgs) : base(eventArgs)
        {
        }

        public bool GetResult()
        {
            ThrowOnSocketError();

            if (EventArgs.ConnectSocket == null)
            {
                throw new SocketException((int)SocketError.NotConnected);
            }

            return true;
        }

        public ISocketAwaitable<bool> GetAwaiter()
        {
            return this;
        }
    }
}