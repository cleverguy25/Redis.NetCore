using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Redis.NetCore.Configuration;
using Xunit;

namespace Redis.NetCore.Tests
{
    public class RedisClientTest
    {
        [Fact]
        public async Task MulitpleGetBytesAsync()
        {
            using (var client = CreateClient())
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
        public async Task NoConnectAsync()
        {
            var redisConfiguration = new RedisConfiguration
                                     {
                                         Endpoints = new[] { "localhost:1234" }
                                     };

            using (var client = RedisClient.CreateClient(Options.Create(redisConfiguration)))
            {
                await Assert.ThrowsAsync<RedisException>(() => client.SetStringAsync("NoConnection", "NoOp"));
            }
        }

        [Fact]
        public async Task SetStringBytesAsync()
        {
            using (var client = CreateClient())
            {
                const string expected = "FooString!";
                await client.SetStringAsync("SetGetString", expected);
                var actual = await client.GetStringAsync("SetGetString");
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async Task SetStringExpiredAsync()
        {
            using (var client = CreateClient())
            {
                const string expected = "FooString!";
                await client.SetAsync("SetStringExpired", Encoding.UTF8.GetBytes(expected), TimeSpan.FromSeconds(1000));
                var timeToLive = await client.GetTimeToLive("SetStringExpired");
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task QuickFireAsync()
        {
            using (var client = CreateClient())
            {
                var tasks = new List<Task>();
                const int numberOperations = 2000;
                for (var i = 0; i < numberOperations; i++)
                {
                    var task = client.SetStringAsync("QuickFire" + i, "Value" + i);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);

                tasks = new List<Task>();
                var getTasksMap = new Dictionary<int, Task<string>>();
                for (var i = 0; i < numberOperations; i++)
                {
                    var task = client.GetStringAsync("QuickFire" + i);
                    tasks.Add(task);
                    getTasksMap[i] = task;
                }

                await Task.WhenAll(tasks);

                Debug.WriteLine("Processing results");
                foreach (var keyPair in getTasksMap)
                {
                    Assert.Equal("Value" + keyPair.Key, keyPair.Value.Result);
                }
            }
        }

        [Fact]
        public async Task SetBytesAsync()
        {
            using (var client = CreateClient())
            {
                const string expected = "Foo!";
                await client.SetAsync("SetGet", Encoding.UTF8.GetBytes(expected));
                var bytes = await client.GetAsync("SetGet");
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
                var timeToLive = await client.GetTimeToLive("SetGet");
                Assert.Equal(-1, timeToLive);
            }
        }

        [Fact]
        public async Task SetExpiredBytesAsync()
        {
            using (var client = CreateClient())
            {
                const string expected = "Foo!";
                await client.SetAsync("SetExpired", Encoding.UTF8.GetBytes(expected), TimeSpan.FromSeconds(1000));
                var timeToLive = await client.GetTimeToLive("SetExpired");
                Assert.NotNull(timeToLive);
            }
        }

        private static IRedisClient CreateClient()
        {
            var redisConfiguration = new RedisConfiguration
            {
                Endpoints = new[] { "localhost:32768" }
            };
            var client = RedisClient.CreateClient(Options.Create(redisConfiguration));
            return client;
        }
    }
}