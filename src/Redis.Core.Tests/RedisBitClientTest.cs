using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisBitClientTest
    {
        [Fact]
        public async Task GetBitCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "foobar";
                const string key = nameof(GetBitCountAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var count = await client.GetBitCountAsync(key);
                Assert.Equal(26, count);
            }
        }

        [Fact]
        public async Task GetBitCountRangeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "foobar";
                const string key = nameof(GetBitCountRangeAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var count = await client.GetBitCountAsync(key, 0, 1);
                Assert.Equal(10, count);
            }
        }

        [Fact]
        public async Task PerformBitOperationAndAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(PerformBitOperationAndAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                const string key3 = key + "3";
                await TestClient.SetGetAsync(client, key1, "foobar");
                await TestClient.SetGetAsync(client, key2, "abcdef");
                var count = await client.PerformBitwiseAndAsync(key3, key1, key2);
                Assert.Equal(6, count);
                var value = await client.GetStringAsync(key3);
                Assert.Equal("`bc`ab", value);
            }
        }

        [Fact]
        public async Task PerformBitOperationOrAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(PerformBitOperationOrAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                const string key3 = key + "3";
                await TestClient.SetGetAsync(client, key1, "foobar");
                await TestClient.SetGetAsync(client, key2, "abcdef");
                var count = await client.PerformBitwiseOrAsync(key3, key1, key2);
                Assert.Equal(6, count);
                var value = await client.GetStringAsync(key3);
                Assert.Equal("goofev", value);
            }
        }

        [Fact]
        public async Task PerformBitOperationXorAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(PerformBitOperationXorAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                const string key3 = key + "3";
                await TestClient.SetGetAsync(client, key1, "foobar");
                await TestClient.SetGetAsync(client, key2, "abcdef");
                var count = await client.PerformBitwiseXorAsync(key3, key1, key2);
                Assert.Equal(6, count);
                var value = await client.GetStringAsync(key3);
                Assert.Equal("\a\r\f\u0006\u0004\u0014", value);
            }
        }

        [Fact]
        public async Task PerformBitOperationNotAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(PerformBitOperationNotAsync);
                const string destinationKey = key + "dest";
                await TestClient.SetGetAsync(client, key, "foobar");
                var count = await client.PerformBitwiseNotAsync(destinationKey, key);
                Assert.Equal(6, count);
                var value = await client.GetStringAsync(destinationKey);
                Assert.Equal("������", value);
            }
        }
    }
}
