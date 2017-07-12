// <copyright file="RedisKeyClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisKeyClientTest
    {
        private Dictionary<int, string> _sortMap = new Dictionary<int, string>()
                                                   {
                                                       { 0, "zero" },
                                                       { 1, "one" },
                                                       { 2, "two" },
                                                       { 3, "three" },
                                                       { 4, "four" },
                                                       { 5, "five" },
                                                       { 6, "six" },
                                                       { 7, "seven" },
                                                       { 8, "eight" },
                                                       { 9, "nine" },
                                                       { 10, "ten" },
                                                   };

        [Fact]
        public async Task DumpAndRestoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(DumpAndRestoreAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var data = await client.DumpAsync(key);
                await client.DeleteKeyAsync(key);
                var exists = await client.ExistsAsync(key);
                Assert.False(exists);
                await client.RestoreAsync(key, 100000, data);
                var value = await client.GetStringAsync(key);
                Assert.Equal(expected, value);
                var timeToLive = await client.GetTimeToLiveAsync(key);
                Assert.True(timeToLive > 0);
            }
        }

        [Fact]
        public async Task GetObjectAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(GetObjectAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "Foo", "Bar");
                var encoding = await client.GetObjectEncodingAsync(key);
                var idleTime = await client.GetObjectIdleTimeAsync(key);
                var referenceCount = await client.GetObjectReferenceCountAsync(key);
                Assert.False(string.IsNullOrEmpty(encoding));
                Assert.True(idleTime >= 0);
                Assert.True(referenceCount > 0);
            }
        }

        [Fact]
        public async Task SortAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                var bytes = await client.Sort(key).ExecuteAsync();
                CheckSortedNumberArray(bytes, 0);
            }
        }

        [Fact]
        public async Task SortStoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortStoreAsync);
                const string storeKey = key + "Store";
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                await client.Sort(key).StoreAsync(storeKey);
                var bytes = await client.ListRangeAsync(storeKey, 0, 10);
                CheckSortedNumberArray(bytes, 0);
            }
        }

        [Fact]
        public async Task SortGetAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortGetAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                for (var i = 0; i <= 10; i++)
                {
                    await client.SetStringAsync($"object_{i}", _sortMap[i]);
                }

                var bytes = await client.Sort(key).Get("object_*").ExecuteStringAsync();
                Assert.Equal(new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" }, bytes);
            }
        }

        [Fact]
        public async Task SortByAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortByAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                for (var i = 0; i <= 10; i++)
                {
                    await client.SetStringAsync($"weight_{i}", (10 - i).ToString(CultureInfo.InvariantCulture));
                }

                var bytes = await client.Sort(key).By("weight_*").ExecuteAsync();
                CheckSortedNumberDescendingArray(bytes, 0);
            }
        }

        [Fact]
        public async Task SortWithLimitAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortWithLimitAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                const int offset = 3;
                const int count = 5;
                var bytes = await client.Sort(key).Limit(offset, count).ExecuteAsync();
                Assert.Equal(count, bytes.Length);
                CheckSortedNumberArray(bytes, offset);
            }
        }

        [Fact]
        public async Task SortDescendingAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortDescendingAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                var bytes = await client.Sort(key).Descending().ExecuteAsync();
                CheckSortedNumberDescendingArray(bytes, 0);
            }
        }

        [Fact]
        public async Task SortDescendingWithLimitAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortDescendingWithLimitAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "7", "5", "6", "8", "1", "3", "2", "4", "10", "0", "9");
                const int offset = 3;
                const int count = 5;
                var bytes = await client.Sort(key).Limit(offset, count).Descending().ExecuteAsync();
                Assert.Equal(count, bytes.Length);
                CheckSortedNumberDescendingArray(bytes, offset);
            }
        }

        [Fact]
        public async Task SortAlphaAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortAlphaAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "one", "two", "three", "four", "five");
                var bytes = await client.Sort(key).Alpha().ExecuteStringAsync();
                Assert.Equal(new[] { "five", "four", "one", "three", "two" }, bytes);
            }
        }

        [Fact]
        public async Task SortAlphaWithLimitAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortAlphaWithLimitAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "one", "two", "three", "four", "five");
                const int offset = 2;
                const int count = 2;
                var bytes = await client.Sort(key).Alpha().Limit(offset, count).ExecuteStringAsync();
                Assert.Equal(count, bytes.Length);
                Assert.Equal(new[] { "one", "three" }, bytes);
            }
        }

        [Fact]
        public async Task SortAlphaDescendingAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortAlphaDescendingAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "one", "two", "three", "four", "five");
                var bytes = await client.Sort(key).Alpha().Descending().ExecuteStringAsync();
                Assert.Equal(new[] { "two", "three", "one", "four", "five" }, bytes);
            }
        }

        [Fact]
        public async Task SortAlphaDescendingWithLimitAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(SortAlphaDescendingWithLimitAsync);
                await client.DeleteKeyAsync(key);
                await client.ListPushStringAsync(key, "one", "two", "three", "four", "five");
                const int offset = 2;
                const int count = 2;
                var bytes = await client.Sort(key).Alpha().Descending().Limit(offset, count).ExecuteStringAsync();
                Assert.Equal(count, bytes.Length);
                Assert.Equal(new[] { "one", "four" }, bytes);
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
        public async Task ScanAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(ScanAsync);
                await TestClient.SetGetAsync(client, key, expected);
                var cursor = await client.ScanAsync();
                var keys = cursor.GetStringValues().ToArray();
                Assert.NotEqual(0, keys.Length);

                cursor = await client.ScanAsync(cursor);
                keys = cursor.GetStringValues().ToArray();
                Assert.NotEqual(0, keys.Length);
            }
        }

        [Fact]
        public async Task ScanWithCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(ScanWithCountAsync);
                await AddKeys(10, client, key, expected);

                var cursor = await client.ScanAsync(5);
                var keys = cursor.GetStringValues().ToArray();
                Assert.True(keys.Length >= 5);

                cursor = await client.ScanAsync(cursor, 5);
                keys = cursor.GetStringValues().ToArray();
                Assert.True(keys.Length > 0);
            }
        }

        [Fact]
        public async Task ScanWithMatchAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(ScanWithMatchAsync);
                await AddKeys(100, client, key, expected);

                const string match = key + "*";
                var cursor = await client.ScanAsync(match);
                var keys = cursor.GetStringValues().ToArray();
                CheckKeys(keys, key);

                cursor = await client.ScanAsync(cursor, match);
                keys = cursor.GetStringValues().ToArray();
                CheckKeys(keys, key);
            }
        }

        [Fact]
        public async Task ScanWithMatchAndCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string key = nameof(ScanWithMatchAndCountAsync);
                await AddKeys(100, client, key, expected);

                const string match = key + "*";
                var cursor = await client.ScanAsync(match, 5);
                var keys = cursor.GetStringValues().ToArray();
                CheckKeys(keys, key);

                for (var i = 0; i < 15; i++)
                {
                    cursor = await client.ScanAsync(cursor, match, 5);
                    keys = cursor.GetStringValues().ToArray();
                    CheckKeys(keys, key);
                }
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

        private static void CheckKeys(IEnumerable<string> keys, string key)
        {
            foreach (var scanKey in keys)
            {
                Assert.Contains(key, scanKey);
            }
        }

        private static async Task AddKeys(int count, IRedisClient client, string key, string expected)
        {
            for (var i = 0; i < count; i++)
            {
                await TestClient.SetGetAsync(client, key + i, expected);
            }
        }

        private static void CheckSortedNumberArray(IReadOnlyList<byte[]> bytes, int offset)
        {
            for (var i = 0; i < bytes.Count; i++)
            {
                Assert.Equal((i + offset).ToString(CultureInfo.InvariantCulture).ToBytes(), bytes[i]);
            }
        }

        private static void CheckSortedNumberDescendingArray(IReadOnlyList<byte[]> bytes, int offset)
        {
            for (var i = 0; i < bytes.Count; i++)
            {
                Assert.Equal((bytes.Count + offset - 1 - i).ToString(CultureInfo.InvariantCulture).ToBytes(), bytes[i]);
            }
        }
    }
}