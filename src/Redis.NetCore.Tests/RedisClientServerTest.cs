// <copyright file="RedisClientServerTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>using System.Threading.Tasks;

using System;
using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Configuration;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisClientServerTest
    {
        [Fact]
        public async Task SetGetClientNameAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "TestClient";
                await client.SetClientNameAsync(expected);
                var clientName = await client.GetClientNameAsync();
                Assert.True(clientName.Contains(expected));
            }
        }

        [Fact]
        public async Task GetServerTimeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var dateTime = await client.GetServerTimeAsync();
                Assert.Equal(dateTime.Date, dateTime.Date);
            }
        }

        [Fact]
        public async Task GetClientListAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "TestClient";
                await client.SetClientNameAsync(expected);
                var clientList = await client.GetClientListAsync();
                Assert.True(clientList.Length > 0);
            }
        }

        [Fact]
        public async Task GetInformationAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var information = await client.GetServerInformationAsync();
                var memory = information["# Memory"]["used_memory"];
                Assert.True(int.Parse(memory) > 0);
            }
        }

        [Fact]
        public async Task BackgroundSaveAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var lastSave = await client.GetLastSaveDateTimeAsync();
                await client.BackgroundSaveAsync();
                for (var i = 0; i < 20; i++)
                {
                    var newLastSave = await client.GetLastSaveDateTimeAsync();
                    if (newLastSave > lastSave)
                    {
                        return;
                    }

                    await Task.Delay(1000);
                }
            }

            throw new InvalidOperationException("Background save did not complete in time.");
        }

        [Fact]
        public async Task ConfigurationAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                var configuration = await client.GetConfigurationAsync("*max-*-entries*");
                Assert.Equal(3, configuration.Count);

                const string key = "hash-max-ziplist-entries";
                var originalValue = configuration[key];
                await client.SetConfigurationAsync(key, "256");
                configuration = await client.GetConfigurationAsync(key);
                Assert.Equal("256", configuration[key]);
                await client.SetConfigurationAsync(key, originalValue);
            }
        }
    }
}
