// <copyright file="SocketExtensions.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

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