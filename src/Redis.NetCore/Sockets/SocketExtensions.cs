using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public static class SocketExtensions
    {
        public static ISocket Wrap(this Socket socket)
        {
            return new SocketWrapper(socket);
        }
    }
}