// <copyright file="RedisClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>using System.Threading.Tasks;

using System.Threading.Tasks;
using Redis.NetCore.Configuration;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisClientTest
    {
        [Fact]
        public async Task NoConnectAsync()
        {
            var redisConfiguration = new RedisConfiguration
            {
                Endpoints = new[] { "localhost:1234" }
            };

            using (var client = RedisClient.CreateClient(redisConfiguration))
            {
                await Assert.ThrowsAsync<RedisException>(() => client.SetStringAsync("NoConnection", "NoOp"));
            }
        }
    }
}
