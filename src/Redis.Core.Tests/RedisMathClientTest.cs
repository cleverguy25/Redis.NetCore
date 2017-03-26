using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisMathClientTest
    {
        [Fact]
        public async Task IncrementAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "10";
                const string key = nameof(IncrementAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var value = await client.IncrementAsync(key);
                Assert.Equal(11, value);
            }
        }

        [Fact]
        public async Task IncrementByAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "10";
                const string key = nameof(IncrementByAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var value = await client.IncrementAsync(key, 5);
                Assert.Equal(15, value);
            }
        }

        [Fact]
        public async Task IncrementByFloatAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "10.5";
                const string key = nameof(IncrementByFloatAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var value = await client.IncrementAsync(key, .75f);
                Assert.Equal(11.25, value);
            }
        }

        public async Task DecrementAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "10";
                const string key = nameof(DecrementAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var value = await client.DecrementAsync(key);
                Assert.Equal(9, value);
            }
        }

        [Fact]
        public async Task DecrementByAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "10";
                const string key = nameof(DecrementByAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var value = await client.DecrementAsync(key, 3);
                Assert.Equal(7, value);
            }
        }
    }
}
