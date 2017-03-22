using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    internal sealed class SocketWrapper : ISocket
    {
        private readonly Socket _socket;

        public SocketWrapper(Socket socket)
        {
            _socket = socket;
        }

        public bool SendAsync(SocketAsyncEventArgs args)
        {
            return _socket.SendAsync(args);
        }

        public bool ReceiveAsync(SocketAsyncEventArgs args)
        {
            return _socket.ReceiveAsync(args);
        }

        public bool ConnectAsync(SocketAsyncEventArgs args)
        {
            return _socket.ConnectAsync(args);
        }
        
        public void Dispose()
        {
            _socket.Dispose();
        }

        public Socket UnderlyingSocket
        {
            get { return _socket; }
        }
    }
}
