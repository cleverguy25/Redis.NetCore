using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Redis.NetCore.Sockets
{
    public class SocketAwaitable : INotifyCompletion
    {
        protected readonly SocketAsyncEventArgs EventArgs;
        private static readonly Action SocketCompleted = () => { };
        private Action _onCompleted;

        protected SocketAwaitable(SocketAsyncEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            EventArgs = eventArgs;
            eventArgs.Completed += OnSocketOperationCompleted;
        }

        public bool IsCompleted { get; set; }

        public void OnCompleted(Action continuation)
        {
            if (_onCompleted == SocketCompleted || SwapOnCompleted(continuation) == SocketCompleted)
            {
                Task.Run(continuation);
            }
        }

        public void Reset()
        {
            IsCompleted = false;
            _onCompleted = null;
        }

        protected void ThrowOnSocketError()
        {
            var error = EventArgs.SocketError;
            if (error != SocketError.Success)
            {
                throw new SocketException((int)error);
            }
        }

        private void OnSocketOperationCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            var continuation = _onCompleted ?? SwapOnCompleted(SocketCompleted);

            continuation?.Invoke();
        }

        private Action SwapOnCompleted(Action continuation)
        {
            return Interlocked.CompareExchange(ref _onCompleted, continuation, null);
        }
    }
}