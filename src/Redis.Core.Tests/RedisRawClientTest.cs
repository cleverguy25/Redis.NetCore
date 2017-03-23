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
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.SetAsync("SetMGet1", Encoding.UTF8.GetBytes(expected1));
                await client.SetAsync("SetMGet2", Encoding.UTF8.GetBytes(expected2));
                var bytes = await client.GetAsync("SetMGet1", "SetMGet2", "NoKey");
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
                const string key = "SetKey";
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected));
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
                var timeToLive = await client.GetTimeToLive(key);
                Assert.Equal(-1, timeToLive);
            }
        }

        [Fact]
        public async Task SetNotExistsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = "SetNotExistsKey";
                await client.DeleteKeyAsync(key);
                var set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes(expected));
                Assert.Equal(true, set);
                var bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));

                set = await client.SetNotExistsAsync(key, Encoding.UTF8.GetBytes("Bar!"));
                Assert.Equal(false, set);
                bytes = await client.GetAsync(key);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task MultipleSetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var data = new List<KeyValuePair<string, byte[]>>
                           {
                               new KeyValuePair<string, byte[]>("MultipleSetKey1", "Foo1".ToBytes()),
                               new KeyValuePair<string, byte[]>("MultipleSetKey2", "Foo2".ToBytes())
                           };
                await client.SetAsync(data);
                var value1 = await client.GetAsync("MultipleSetKey1");
                var value2 = await client.GetAsync("MultipleSetKey2");
                Assert.Equal("Foo1", Encoding.UTF8.GetString(value1));
                Assert.Equal("Foo2", Encoding.UTF8.GetString(value2));
            }
        }

        [Fact]
        public async Task SetExpiredTimeSpanBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = "SetExpired";
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected), TimeSpan.FromSeconds(1000));
                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetExpiredSecondsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = "SetExpired";
                await client.SetAsync(key, Encoding.UTF8.GetBytes(expected), 1000);
                var timeToLive = await client.GetTimeToLive(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task GetSetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = "GetSetString";
                const string expected = "FooString!";
                await TestClient.SetGetAsync(client, key, expected);
                var actual = await client.GetSetAsync(key, "BarString!".ToBytes());
                Assert.Equal(expected.ToBytes(), actual);
            }
        }
    }
}