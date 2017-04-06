// <copyright file="RedisKeyClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisKeyClientTest
    {
        [Fact]
        public async Task GetKeysAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GetKeysAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                await TestClient.SetGetAsync(client, key1, "Value1");
                await TestClient.SetGetAsync(client, key2, "Value2");
                var actualKeys = await client.GetKeysAsync(key + "*");
                Array.Sort(actualKeys);
                Assert.Equal(2, actualKeys.Length);
                Assert.Equal(key1, actualKeys[0]);
                Assert.Equal(key2, actualKeys[1]);
            }
        }

        [Fact]
        public async Task DeleteAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(DeleteAsync);
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
                const string existsKey = nameof(ExistAsync);
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
                const string key = nameof(MultipleExistAsync);
                const string existsKey1 = key + "1";
                await TestClient.SetGetAsync(client, existsKey1, "FooExists!");
                const string existsKey2 = key + "2";
                await TestClient.SetGetAsync(client, existsKey2, "FooExists!");

                var count = await client.ExistsAsync(existsKey1, existsKey1, "NoKey");
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public async Task SetExpiredTimeSpanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, TimeSpan.FromMinutes(5));
                Assert.Equal(true, set);

                var timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetExpiredSecondsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredTimeSpanAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, 1000);
                Assert.Equal(true, set);

                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task SetExpiredDateTimeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(SetExpiredDateTimeAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, new DateTime(2020, 1, 1));
                Assert.Equal(true, set);

                var timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);
            }
        }

        [Fact]
        public async Task PersistAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(PersistAsync);
                await client.DeleteKeyAsync(key);
                await TestClient.SetGetAsync(client, key, expected);
                var set = await client.SetExpirationAsync(key, TimeSpan.FromMinutes(5));
                Assert.Equal(true, set);

                var timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.NotNull(timeToLive);

                set = await client.PersistAsync(key);
                Assert.Equal(true, set);

                timeToLive = await client.GetPreciseTimeToLiveAsync(key);
                Assert.Equal(-1, timeToLive);
            }
        }

        [Fact]
        public async Task RenameKeyAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(RenameKeyAsync);
                await TestClient.SetGetAsync(client, key, expected);
                await client.RenameKeyAsync(key, "NewName");
                var value = await client.GetStringAsync("NewName");
                Assert.Equal(expected, value);
            }
        }

        [Fact]
        public async Task RenameKeyNotExistsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(RenameKeyNotExistsAsync);
                const string key1 = key + "1";
                const string newKey = "NewNameNotExists";
                await client.DeleteKeyAsync(newKey);
                await TestClient.SetGetAsync(client, key, expected);
                await TestClient.SetGetAsync(client, key1, "Bar!");

                var set = await client.RenameKeyNotExistsAsync(key, newKey);
                Assert.Equal(true, set);

                var value = await client.GetStringAsync(newKey);
                Assert.Equal(expected, value);

                set = await client.RenameKeyNotExistsAsync(key1, newKey);
                Assert.Equal(false, set);
            }
        }

        [Fact]
        public async Task GetRandomKeyAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(GetRandomKeyAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var randomKey = await client.GetRandomKeyAsync();
                Assert.NotNull(randomKey);
            }
        }

        [Fact]
        public async Task ScanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(ScanAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var cursor = await client.ScanAsync();
                var keys = cursor.GetKeys().ToArray();
                Assert.NotEqual(0, keys.Length);

                cursor = await client.ScanAsync(cursor);
                keys = cursor.GetKeys().ToArray();
                Assert.NotEqual(0, keys.Length);
            }
        }

        [Fact]
        public async Task TouchAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(TouchAsync);
                await TestClient.SetGetAsync(client, key, expected);
                await client.TouchAsync(key);
            }
        }

        [Fact]
        public async Task GetTypeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(GetTypeAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var type = await client.GetTypeAsync(key);
                Assert.Equal("string", type);
            }
        }
    }
}
