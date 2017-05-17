// <copyright file="RedisClient.Connection.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisConnectionClient
    {
        public async Task<string> PingAsync()
        {
            var bytes = await SendCommandAsync(RedisCommands.Ping).ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }

        public async Task<string> EchoAsync(string message)
        {
            var bytes = await SendCommandAsync(RedisCommands.Echo, message.ToBytes()).ConfigureAwait(false);
            return Encoding.UTF8.GetString(bytes);
        }

        public Task QuitAsync()
        {
            return SendCommandAsync(RedisCommands.Quit);
        }

        public Task SelectDatabaseAsync(int index)
        {
            return SendCommandAsync(RedisCommands.SelectDatabase, index.ToString().ToBytes());
        }
    }
}
