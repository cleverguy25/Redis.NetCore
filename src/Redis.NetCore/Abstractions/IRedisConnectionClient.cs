// <copyright file="IRedisConnectionClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisConnectionClient
    {
        Task<string> PingAsync();

        Task<string> EchoAsync(string message);

        Task QuitAsync();

        Task SelectDatabaseAsync(int index);
    }
}
