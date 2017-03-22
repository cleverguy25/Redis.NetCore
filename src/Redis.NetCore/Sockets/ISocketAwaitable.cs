using System;
using System.Runtime.CompilerServices;

namespace Redis.NetCore.Sockets
{
    public interface ISocketAwaitable<out T> : INotifyCompletion
    {
        bool IsCompleted { get; set; }

        T GetResult();

        ISocketAwaitable<T> GetAwaiter();
    }
}