using System.Collections.Generic;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Configuration;
using Xunit;

namespace Redis.NetCore.Tests
{
    public static class TestClient
    {
        public static IRedisClient CreateClient()
        {
            var redisConfiguration = new RedisConfiguration
            {
                Endpoints = new[] { "localhost:32768" }
            };
            ////var redisConfiguration = new RedisConfiguration
            ////                         {
            ////                             Endpoints = new[] { "answer-cache.redis.cache.windows.net:6380" },
            ////                             Password = "X/4M1PbxfRWrxvyakNEWyYwDHfVr7QvzBpLGLnagDG4="
            ////                         };

            var client = RedisClient.CreateClient(redisConfiguration);
            return client;
        }

        public static async Task SetGetAsync(IRedisStringClient client, string key, string expected)
        {
            await client.SetStringAsync(key, expected);
            var actual = await client.GetStringAsync(key);
            Assert.Equal(expected, actual);
        }

        public static async Task QuickfireAsync(IRedisStringClient client, string label, int count = 1000)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < count; i++)
            {
                var task = client.SetStringAsync(label + i, "Value" + i);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            tasks = new List<Task>();
            var getTasksMap = new Dictionary<int, Task<string>>();
            for (var i = 0; i < count; i++)
            {
                var task = client.GetStringAsync(label + i);
                tasks.Add(task);
                getTasksMap[i] = task;
            }

            await Task.WhenAll(tasks);

            foreach (var keyPair in getTasksMap)
            {
                Assert.Equal("Value" + keyPair.Key, keyPair.Value.Result);
            }
        }
    }
}