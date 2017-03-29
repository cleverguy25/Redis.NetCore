// <copyright file="SendSocketAwaitable.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class SendSocketAwaitable : SocketAwaitable, ISocketAwaitable<int>
    {
        public SendSocketAwaitable(SocketAsyncEventArgs eventArgs)
            : base(eventArgs)
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