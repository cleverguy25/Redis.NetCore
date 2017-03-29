using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class AsyncSocket : IAsyncSocket, IDisposable
    {
        private readonly ISocket _socket;
        private readonly SocketAsyncEventArgs _connectSocketEventArgs;
        private readonly ConnectSocketAwaitable _connectSocketAwaitable;

        private readonly SocketAsyncEventArgs _sendSocketEventArgs;
        private readonly SendSocketAwaitable _sendSocketAwaitable;

        private readonly SocketAsyncEventArgs _receiveSocketEventArgs;
        private readonly ReceiveSocketAwaitable _receiveSocketAwaitable;
        
        public AsyncSocket(ISocket socket, EndPoint endPoint)
        {
            _socket = socket;
            _connectSocketEventArgs = new SocketAsyncEventArgs { RemoteEndPoint = endPoint };
            _connectSocketAwaitable = new ConnectSocketAwaitable(_connectSocketEventArgs);

            _sendSocketEventArgs = new SocketAsyncEventArgs { AcceptSocket = socket.UnderlyingSocket };
            _sendSocketAwaitable = new SendSocketAwaitable(_sendSocketEventArgs);

            _receiveSocketEventArgs = new SocketAsyncEventArgs { AcceptSocket = socket.UnderlyingSocket };
            _receiveSocketAwaitable = new ReceiveSocketAwaitable(_receiveSocketEventArgs);
        }

        public ISocketAwaitable<bool> ConnectAsync()
        {
            _connectSocketAwaitable.Reset();
            _connectSocketAwaitable.IsCompleted = _socket.ConnectAsync(_connectSocketEventArgs) == false;
            return _connectSocketAwaitable;
        }
        
        public ISocketAwaitable<int> SendAsync(IList<ArraySegment<byte>> bufferList)
        {
            _sendSocketAwaitable.Reset();
            _sendSocketEventArgs.BufferList = bufferList;
            _sendSocketAwaitable.IsCompleted = _socket.SendAsync(_sendSocketEventArgs) == false;
            return _sendSocketAwaitable;
        }

        public ISocketAwaitable<int> SendAsync(ArraySegment<byte> buffer)
        {
            _sendSocketAwaitable.Reset();
            _sendSocketEventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
            _sendSocketAwaitable.IsCompleted = _socket.SendAsync(_sendSocketEventArgs) == false;
            return _sendSocketAwaitable;
        }

        public int Send(IList<ArraySegment<byte>> buffers)
        {
            return _socket.Send(buffers);
        }

        public int Receive(IList<ArraySegment<byte>> buffers)
        {
            return _socket.Receive(buffers);
        }

        public ISocketAwaitable<ArraySegment<byte>> ReceiveAsync(ArraySegment<byte> buffer)
        {
            _receiveSocketAwaitable.Reset();
            _receiveSocketEventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
            _receiveSocketAwaitable.IsCompleted = _socket.ReceiveAsync(_receiveSocketEventArgs) == false;
            return _receiveSocketAwaitable;
        }
        
        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}