// <copyright file="TestClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

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

        public static Dictionary<string, string> SetupTestHashFields(string fieldName = "field", int count = 20)
        {
            var fields = new Dictionary<string, string>();
            for (var i = 0; i < 20; i++)
            {
                fields[$"{fieldName}{i}"] = $"value{i}";
            }

            return fields;
        }
    }
}