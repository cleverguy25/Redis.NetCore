using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisKeyClientTest
    {
        [Fact]
        public async Task DeleteAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(DeleteAsync);
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected));
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
                await client.DeleteKeyAsync(key);
                bytes = await client.GetAsync(key);
                Assert.Equal(null, bytes);
            }
        }

        [Fact]
        public async Task ExistAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string existsKey = nameof(ExistAsync);
                await TestClient.SetGetAsync(client, existsKey, "FooExists!");
                var exists = await client.ExistsAsync(existsKey);
                Assert.Equal(true, exists);

                const string noExistsKey = "NoExistsAsync";
                exists = await client.ExistsAsync(noExistsKey);
                Assert.Equal(false, exists);
            }
        }

        [Fact]
        public async Task MultipleExistAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(MultipleExistAsync);
                const string existsKey1 = key + "1";
                await TestClient.SetGetAsync(client, existsKey1, "FooExists!");
                const string existsKey2 = key + "2";
                await TestClient.SetGetAsync(client, existsKey2, "FooExists!");
                
                var count = await client.ExistsAsync(existsKey1, existsKey1, "NoKey");
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public async Task SetExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, TimeSpan.FromMinutes(5));
                Assert.Equal(true, set);

                var timeToLive = await client.GetPreciseTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        public async Task SetExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, 1000);
                Assert.Equal(true, set);

                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        public async Task SetExpiredDateTimeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredDateTimeAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, new DateTime(2020, 1, 1));
                Assert.Equal(true, set);

                var timeToLive = await client.GetPreciseTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

    }
}
