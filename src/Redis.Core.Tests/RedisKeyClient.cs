﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisKeyClient
    {
        [Fact]
        public async Task DeleteAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = "SetGetDelete";
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
                const string existsKey = "ExistsAsync";
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
                const string existsKey1 = "MultipleExistsAsync1";
                await TestClient.SetGetAsync(client, existsKey1, "FooExists!");
                const string existsKey2 = "MultipleExistsAsync2";
                await TestClient.SetGetAsync(client, existsKey2, "FooExists!");
                
                var count = await client.ExistsAsync(existsKey1, existsKey1, "NoKey");
                Assert.Equal(2, count);
            }
        }
    }
}
