﻿// <copyright file="RedisListStringClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisListStringClientTest
    {
        [Fact]
        public async Task ListPushAndPopStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListPushAndPopStringAsync);
                await client.DeleteKeyAsync(listKey);
                var count = await client.ListPushStringAsync(listKey, "Foo!", "Bar!");
                Assert.Equal(2, count);

                var item = await client.ListPopStringAsync(listKey);
                Assert.Equal("Bar!", item);

                item = await client.ListPopStringAsync(listKey);
                Assert.Equal("Foo!", item);
            }
        }

        [Fact]
        public async Task ListPushStringIfExistsAndPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListPushStringIfExistsAndPopAsync);
                var count = await client.ListPushStringIfExistsAsync(listKey + "NotExists", "Foo!");
                Assert.Equal(0, count);

                await client.DeleteKeyAsync(listKey);
                await client.ListPushStringAsync(listKey, "Foo!");

                count = await client.ListPushStringIfExistsAsync(listKey, "Bar!");
                Assert.Equal(2, count);

                var item = await client.ListPopStringAsync(listKey);
                Assert.Equal("Bar!", item);

                item = await client.ListPopStringAsync(listKey);
                Assert.Equal("Foo!", item);
            }
        }

        [Fact]
        public async Task ListTailPushAndPopStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPushAndPopStringAsync);
                await client.DeleteKeyAsync(listKey);
                var count = await client.ListTailPushStringAsync(listKey, "Foo!", "Bar!");
                Assert.Equal(2, count);

                var item = await client.ListTailPopStringAsync(listKey);
                Assert.Equal("Bar!", item);

                item = await client.ListTailPopStringAsync(listKey);
                Assert.Equal("Foo!", item);
            }
        }

        [Fact]
        public async Task ListTailPushStringIfExistsAndPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPushStringIfExistsAndPopAsync);
                var count = await client.ListTailPushStringIfExistsAsync(listKey + "NotExists", "Foo!");
                Assert.Equal(0, count);

                await client.DeleteKeyAsync(listKey);
                await client.ListTailPushStringAsync(listKey, "Foo!");

                count = await client.ListTailPushStringIfExistsAsync(listKey, "Bar!");
                Assert.Equal(2, count);

                var item = await client.ListTailPopStringAsync(listKey);
                Assert.Equal("Bar!", item);

                item = await client.ListTailPopStringAsync(listKey);
                Assert.Equal("Foo!", item);
            }
        }

        [Fact]
        public async Task ListBlockingPopStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingPopStringAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                var popTask = client.ListBlockingPopStringAsync(0, list1, list2);
                var pushTask = client.ListPushStringAsync(list2, "Foo!");
                await Task.WhenAll(popTask, pushTask);

                var item = popTask.Result;
                Assert.Equal(list2, item.Item1);
                Assert.Equal("Foo!", item.Item2);
            }
        }

        [Fact]
        public async Task ListBlockingPopStringNilAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingPopStringNilAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                var item = await client.ListBlockingPopStringAsync(2, list1, list2);

                Assert.Null(item);
            }
        }

        [Fact]
        public async Task ListBlockingTailPopStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingTailPopStringAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                await client.ListPushStringAsync(list1, "Foo!", "Bar!");
                var item = await client.ListBlockingTailPopStringAsync(0, list1, list2);

                Assert.Equal(list1, item.Item1);
                Assert.Equal("Foo!", item.Item2);
            }
        }

        [Fact]
        public async Task ListBlockingTailPopStringNilAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingTailPopStringNilAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                var item = await client.ListBlockingTailPopStringAsync(2, list1, list2);

                Assert.Null(item);
            }
        }

        [Fact]
        public async Task ListTailPopAndPushStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPopAndPushStringAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                await client.ListPushStringAsync(list1, "Foo!", "Bar!");

                var item = await client.ListTailPopAndPushStringAsync(list1, list2);
                Assert.Equal("Foo!", item);

                item = await client.ListPopStringAsync(list2);
                Assert.Equal("Foo!", item);
            }
        }

        [Fact]
        public async Task ListBlockingTailPopAndPushStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingTailPopAndPushStringAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                await client.ListPushAsync(list1, "Foo!".ToBytes(), "Bar!".ToBytes());

                var item = await client.ListBlockingTailPopAndPushStringAsync(list1, list2, 0);
                Assert.Equal("Foo!", item);

                item = await client.ListPopStringAsync(list2);
                Assert.Equal("Foo!", item);
            }
        }

        [Fact]
        public async Task ListIndexStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListIndexStringAsync);
                await client.DeleteKeyAsync(listKey);
                var count = await client.ListPushAsync(listKey, "Foo!".ToBytes(), "Bar!".ToBytes());
                Assert.Equal(2, count);

                var item = await client.ListIndexStringAsync(listKey, -1);
                Assert.Equal("Foo!", item);

                item = await client.ListIndexStringAsync(listKey, 0);
                Assert.Equal("Bar!", item);
            }
        }

        [Fact]
        public async Task ListInsertBeforeAndAfterStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListInsertBeforeAndAfterStringAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushStringAsync(listKey, "Foo!", "Bar!");
                await client.ListInsertBeforeStringAsync(listKey, "Foo!", "InsertBefore");
                await client.ListInsertAfterStringAsync(listKey, "InsertBefore", "InsertAfter");

                var item = await client.ListIndexStringAsync(listKey, -2);
                Assert.Equal("InsertAfter", item);

                item = await client.ListIndexStringAsync(listKey, -3);
                Assert.Equal("InsertBefore", item);
            }
        }

        [Fact]
        public async Task ListRangeStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListRangeStringAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushStringAsync(listKey, "1", "2", "3");

                var items = await client.ListRangeStringAsync(listKey, 1, -1);
                Assert.Equal(2, items.Length);
                Assert.Equal("2", items[0]);
                Assert.Equal("1", items[1]);
            }
        }

        [Fact]
        public async Task ListRemoveStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListRemoveStringAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushStringAsync(listKey, "1", "2", "2", "3");

                var count = await client.ListRemoveStringAsync(listKey, 1, "2");
                Assert.Equal(1, count);

                var items = await client.ListRangeStringAsync(listKey, 0, -1);
                Assert.Equal("3", items[0]);
                Assert.Equal("2", items[1]);
                Assert.Equal("1", items[2]);
            }
        }

        [Fact]
        public async Task ListSetStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListSetStringAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushStringAsync(listKey, "1", "2", "3");

                await client.ListSetStringAsync(listKey, 1, "Update");

                var item = await client.ListIndexStringAsync(listKey, 1);
                Assert.Equal("Update", item);
            }
        }
    }
}
