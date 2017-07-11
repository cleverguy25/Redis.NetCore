// <copyright file="RedisClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>using System.Threading.Tasks;

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
                foreach (var item in clientList)
                {
                    Assert.True(item["name"].Contains(expected));
                }
            }
        }
    }
}
