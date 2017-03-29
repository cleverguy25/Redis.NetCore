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
    public class RedisRawClientTest
    {
        [Fact]
        public async Task MulitpleGetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(MulitpleGetBytesAsync);
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.SetAsync(key + "1", Encoding.UTF8.GetBytes(expected1));
                await client.SetAsync(key + "2", Encoding.UTF8.GetBytes(expected2));
                var bytes = await client.GetAsync(key + "1", key + "2", "NoKey");
                Assert.Equal(expected1, Encoding.UTF8.GetString(bytes[0]));
                Assert.Equal(expected2, Encoding.UTF8.GetString(bytes[1]));
                Assert.Equal(null, bytes[2]);
            }
        }

        [Fact]
        public async Task ParallelAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const int partitions = 5;
                var tasks = new List<Task>();
                for (var i = 0; i < partitions; i++)
                {
                    var index = i;
                    var task = Task.Run(async () => await TestClient.QuickfireAsync(client, "Parallel_partition" + index));
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }
        }

        [Fact]
        public async Task QuickFireAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                await TestClient.QuickfireAsync(client, "QuickFire");
            }
        }

        [Fact]
        public async Task SetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetBytesAsync);
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected));
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.Equal(-1, timeToLive);
            }
        }

        [Fact]
        public async Task SetNotExistsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetNotExistsBytesAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes(expected));
                Assert.Equal(true, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));

                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.Equal(-1, timeToLive);

                set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes("Bar!"));
                Assert.Equal(false, set);
                bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task SetNotExistsBytesExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetNotExistsBytesExpiredSecondsAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes(expected), 3600);
                Assert.Equal(true, set);

                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);

                set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes("Bar!"), 3600);
                Assert.Equal(false, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task SetNotExistsBytesExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetNotExistsBytesExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes(expected), TimeSpan.FromDays(1));
                Assert.Equal(true, set);

                var timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);

                set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes("Bar!"), TimeSpan.FromDays(1));
                Assert.Equal(false, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task SetExistsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExistsBytesAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetExistsAsync(key, Encoding.UTF8.GetBytes(expected));
                Assert.Equal(false, set);

                await TestClient.SetGetAsync(client, key, "Bar!");
                set = await client.SetExistsAsync(key, Encoding.UTF8.GetBytes(expected));
                Assert.Equal(true, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));

                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.Equal(-1, timeToLive);
            }
        }

        [Fact]
        public async Task SetExistsBytesExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExistsBytesExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetExistsAsync(key, Encoding.UTF8.GetBytes(expected), TimeSpan.FromMinutes(5));
                Assert.Equal(false, set);

                await TestClient.SetGetAsync(client, key, "Bar!");
                set = await client.SetExistsAsync(key, Encoding.UTF8.GetBytes(expected), TimeSpan.FromMinutes(5));
                Assert.Equal(true, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));

                var timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetExistsBytesExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExistsBytesExpiredSecondsAsync);
                await client.DeleteKeyAsync(key);
                var set = await client.SetExistsAsync(key, Encoding.UTF8.GetBytes(expected), 120);
                Assert.Equal(false, set);

                await TestClient.SetGetAsync(client, key, "Bar!");
                set = await client.SetExistsAsync(key, Encoding.UTF8.GetBytes(expected), 120);
                Assert.Equal(true, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));

                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task MultipleSetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(MultipleSetBytesAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                var data = new Dictionary<string, byte[]>
                           {
                               { key1, "Foo1".ToBytes() },
                               { key2, "Foo2".ToBytes() }
                           };
                await client.SetAsync(data);
                var value1 = await client.GetAsync(key1);
                var value2 = await client.GetAsync(key2);
                Assert.Equal("Foo1", Encoding.UTF8.GetString(value1));
                Assert.Equal("Foo2", Encoding.UTF8.GetString(value2));
            }
        }

        [Fact]
        public async Task MultipleSetNotExistsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(MultipleSetNotExistsBytesAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                await client.DeleteKeyAsync(key1);
                await client.DeleteKeyAsync(key2);
                var data = new Dictionary<string, byte[]>
                           {
                               { key1, "Foo1".ToBytes() },
                               { key2, "Foo2".ToBytes() }
                           };
                var set = await client.SetNotExistsAsync(data);
                Assert.Equal(true, set);
                var value1 = await client.GetAsync(key1);
                var value2 = await client.GetAsync(key2);
                Assert.Equal("Foo1", Encoding.UTF8.GetString(value1));
                Assert.Equal("Foo2", Encoding.UTF8.GetString(value2));

                data = new Dictionary<string, byte[]>
                           {
                               { key1, "Bar1".ToBytes() },
                               { key + "NotFound", "Bar2".ToBytes() }
                           };
                set = await client.SetNotExistsAsync(data);
                Assert.Equal(false, set);
                value1 = await client.GetAsync(key1);
                Assert.Equal("Foo1", Encoding.UTF8.GetString(value1));
            }
        }

        [Fact]
        public async Task SetExpiredTimeSpanBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredTimeSpanBytesAsync);
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected), TimeSpan.FromSeconds(1000));
                var timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetExpiredSecondsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredSecondsBytesAsync);
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected), 1000);
                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetRangeBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Hello Bar!";
                const string key = nameof(SetBytesAsync);
                await TestClient.SetGetAsync(client, key, "Hello Foo!");
                var length = await client.SetRangeAsync(key, 6, "Bar".ToBytes());
                var bytes = await client.GetAsync(key);
                Assert.Equal(10, length);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task GetRangeBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Hello Foo!";
                const string key = nameof(GetRangeBytesAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var bytes = await client.GetRangeAsync(key, 6, 8);
                Assert.Equal("Foo", Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task GetSetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GetSetBytesAsync);
                const string expected = "FooString!";
                await TestClient.SetGetAsync(client, key, expected);
                var actual = await client.GetSetAsync(key, "BarString!".ToBytes());
                Assert.Equal(expected.ToBytes(), actual);
            }
        }
    }
}