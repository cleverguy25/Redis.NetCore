// <copyright file="ConnectSocketAwaitable.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Sockets;

namespace Redis.NetCore.Sockets
{
    public class ConnectSocketAwaitable : SocketAwaitable, ISocketAwaitable<bool>
    {
        public ConnectSocketAwaitable(SocketAsyncEventArgs eventArgs)
            : base(eventArgs)
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