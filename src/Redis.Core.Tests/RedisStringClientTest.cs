using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisStringClientTest
    {
        [Fact]
        public async Task SetStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                await TestClient.SetGetAsync(client, "SetGetString", "FooString!");
            }
        }

        [Fact]
        public async Task SetStringExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "FooString!";
                const string key = "SetStringExpired";
                await client.SetStringAsync(key, expected, TimeSpan.FromSeconds(1000));
                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetStringExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "FooString!";
                const string key = "SetStringExpired";
                await client.SetStringAsync(key, expected, 1000);
                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetStringsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var data = new List<KeyValuePair<string, string>>
                           {
                               new KeyValuePair<string, string>("MultipleSetStringKey1", "Foo1"),
                               new KeyValuePair<string, string>("MultipleSetStringKey2", "Foo2")
                           };
                await client.SetStringsAsync(data);
                var value1 = await client.GetAsync("MultipleSetStringKey1");
                var value2 = await client.GetAsync("MultipleSetStringKey2");
                Assert.Equal("Foo1", Encoding.UTF8.GetString(value1));
                Assert.Equal("Foo2", Encoding.UTF8.GetString(value2));
            }
        }

        [Fact]
        public async Task SetStringNotExistsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = "SetStringNotExistsKey";
                await client.DeleteKeyAsync(key);
                var set = await client.SetStringNotExistsAsync(key, expected);
                Assert.Equal(true, set);

                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);

                set = await client.SetStringNotExistsAsync(key, "Bar!");
                Assert.Equal(false, set);
                value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);
            }
        }

        [Fact]
        public async Task AppendAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                
                const string key = "AppendString";
                await client.DeleteKeyAsync(key);
                var length = await client.AppendStringAsync(key, "Boom ");
                Assert.Equal(5, length);

                length = await client.AppendStringAsync(key, "Shakalaka!");
                Assert.Equal(15, length);

                var value = await client.GetStringAsync(key);
                Assert.Equal("Boom Shakalaka!", value);
            }
        }

        [Fact]
        public async Task GetStringsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.SetStringAsync("SetMGetString1", expected1);
                await client.SetStringAsync("SetMGetString2", expected2);
                var values = await client.GetStringsAsync("SetMGetString1", "SetMGetString2", "NoKey");
                Assert.Equal(expected1, values[0]);
                Assert.Equal(expected2, values[1]);
                Assert.Equal(null, values[2]);
            }
        }

        [Fact]
        public async Task GetSetStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = "GetSetString";
                const string expected = "FooString!";
                await TestClient.SetGetAsync(client, key, expected);
                var actual = await client.GetSetStringAsync(key, "BarString!");
                Assert.Equal(expected, actual);
            }
        }
    }
}
