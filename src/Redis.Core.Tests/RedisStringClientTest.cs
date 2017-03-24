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
                await TestClient.SetGetAsync(client, nameof(SetStringAsync), "FooString!");
            }
        }

        [Fact]
        public async Task SetStringExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "FooString!";
                const string key = nameof(SetStringExpiredTimeSpanAsync);
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
                const string key = nameof(SetStringExpiredSecondsAsync);
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
                const string key = nameof(SetStringsAsync);
                var data = new List<KeyValuePair<string, string>>
                           {
                               new KeyValuePair<string, string>(key + "1", "Foo1"),
                               new KeyValuePair<string, string>(key + "2", "Foo2")
                           };
                await client.SetStringsAsync(data);
                var value1 = await client.GetAsync(key + "1");
                var value2 = await client.GetAsync(key + "2");
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
                const string key = nameof(SetStringNotExistsAsync);
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
        public async Task SetStringNotExistsExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetStringNotExistsExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetStringNotExistsAsync(key, expected, TimeSpan.FromHours(1));
                Assert.Equal(true, set);

                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);

                set = await client.SetStringNotExistsAsync(key, "Bar!");
                Assert.Equal(false, set);
                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);
            }
        }

        [Fact]
        public async Task SetStringNotExistsExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetStringNotExistsExpiredSecondsAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetStringNotExistsAsync(key, expected, 500);
                Assert.Equal(true, set);

                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);

                set = await client.SetStringNotExistsAsync(key, "Bar!");
                Assert.Equal(false, set);
                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);
            }
        }

        [Fact]
        public async Task SetStringExistsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetStringExistsAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetStringExistsAsync(key, expected);
                Assert.Equal(false, set);

                await TestClient.SetGetAsync(client, key, "Bar!");
                set = await client.SetStringExistsAsync(key, expected);
                Assert.Equal(true, set);
                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);

                var timeToLive = await client.GetTimeToLive(key);
                Assert.Equal(-1, timeToLive);
            }
        }

        [Fact]
        public async Task SetStringExistsExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetStringExistsExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetStringExistsAsync(key, expected, TimeSpan.FromMinutes(5));
                Assert.Equal(false, set);

                await TestClient.SetGetAsync(client, key, "Bar!");
                set = await client.SetStringExistsAsync(key, expected, TimeSpan.FromMinutes(5));
                Assert.Equal(true, set);
                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);

                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetStringExistsExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetStringExistsExpiredSecondsAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetStringExistsAsync(key, expected, 120);
                Assert.Equal(false, set);

                await TestClient.SetGetAsync(client, key, "Bar!");
                set = await client.SetStringExistsAsync(key, expected, 120);
                Assert.Equal(true, set);
                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);

                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task AppendAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(AppendAsync);
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
        public async Task StringLengthAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(StringLengthAsync);
                await TestClient.SetGetAsync(client, key, "Mu ha ha ha!");
                var length = await client.GetStringLengthAsync(key);
                Assert.Equal(12, length);

                length = await client.GetStringLengthAsync(key + "NotExists");
                Assert.Equal(0, length);
            }
        }

        [Fact]
        public async Task GetStringsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GetStringsAsync);
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.SetStringAsync(key + "1", expected1);
                await client.SetStringAsync(key + "2", expected2);
                var values = await client.GetStringsAsync(key + "1", key + "2", "NoKey");
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
                const string key = nameof(GetSetStringAsync);
                const string expected = "FooString!";
                await TestClient.SetGetAsync(client, key, expected);
                var actual = await client.GetSetStringAsync(key, "BarString!");
                Assert.Equal(expected, actual);
            }
        }
    }
}
