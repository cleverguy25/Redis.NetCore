using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisConnectionClientTest
    {
        [Fact]
        public async Task PingAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var response = await client.PingAsync();
                Assert.Equal("PONG", response);
            }
        }

        [Fact]
        public async Task EchoAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Bounce";
                var message = await client.EchoAsync(expected);
                Assert.Equal(expected, message);
            }
        }

        [Fact]
        public async Task QuitAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                await client.QuitAsync();
            }
        }
    }
}