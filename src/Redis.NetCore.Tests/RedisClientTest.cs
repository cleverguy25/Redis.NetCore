using System;
using System.Collections.Generic;
using System.Linq;
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
