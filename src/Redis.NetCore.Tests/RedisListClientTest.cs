// <copyright file="RedisListClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisListClientTest
    {
        [Fact]
        public async Task ListPushAndPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListPushAndPopAsync);
                await client.DeleteKeyAsync(listKey);
                var count = await client.ListPushAsync(listKey, "Foo!".ToBytes(), "Bar!".ToBytes());
                Assert.Equal(2, count);

                var item = await client.ListPopAsync(listKey);
                Assert.Equal("Bar!", Encoding.UTF8.GetString(item));

                item = await client.ListPopAsync(listKey);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListPushIfExistsAndPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListPushIfExistsAndPopAsync);
                var count = await client.ListPushIfExistsAsync(listKey + "NotExists", "Foo!".ToBytes());
                Assert.Equal(0, count);

                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "Foo!".ToBytes());

                count = await client.ListPushIfExistsAsync(listKey, "Bar!".ToBytes());
                Assert.Equal(2, count);

                var item = await client.ListPopAsync(listKey);
                Assert.Equal("Bar!", Encoding.UTF8.GetString(item));

                item = await client.ListPopAsync(listKey);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListTailPushAndPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPushAndPopAsync);
                await client.DeleteKeyAsync(listKey);
                var count = await client.ListTailPushAsync(listKey, "Foo!".ToBytes(), "Bar!".ToBytes());
                Assert.Equal(2, count);

                var item = await client.ListTailPopAsync(listKey);
                Assert.Equal("Bar!", Encoding.UTF8.GetString(item));

                item = await client.ListTailPopAsync(listKey);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListTailPushIfExistsAndPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPushIfExistsAndPopAsync);
                await client.DeleteKeyAsync(listKey + "NotExists");
                var count = await client.ListTailPushIfExistsAsync(listKey + "NotExists", "Foo!".ToBytes());
                Assert.Equal(0, count);

                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "Foo!".ToBytes());

                count = await client.ListTailPushIfExistsAsync(listKey, "Bar!".ToBytes());
                Assert.Equal(2, count);

                var item = await client.ListTailPopAsync(listKey);
                Assert.Equal("Bar!", Encoding.UTF8.GetString(item));

                item = await client.ListTailPopAsync(listKey);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListBlockingPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingPopAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                var popTask = client.ListBlockingPopAsync(0, list1, list2);
                var pushTask = client.ListPushAsync(list2, "Foo!".ToBytes());
                await Task.WhenAll(popTask, pushTask);

                var item = popTask.Result;
                Assert.Equal(list2, item.Item1);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item.Item2));
            }
        }

        [Fact]
        public async Task ListBlockingPopNilAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingPopNilAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                var item = await client.ListBlockingPopAsync(2, list1, list2);

                Assert.Null(item);
            }
        }

        [Fact]
        public async Task ListBlockingTailPopAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingTailPopAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                await client.ListPushStringAsync(list1, "Foo!", "Bar!");
                var item = await client.ListBlockingTailPopAsync(0, list1, list2);

                Assert.Equal(list1, item.Item1);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item.Item2));
            }
        }

        [Fact]
        public async Task ListBlockingTailPopNilAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingTailPopNilAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                var item = await client.ListBlockingTailPopAsync(2, list1, list2);

                Assert.Null(item);
            }
        }

        [Fact]
        public async Task ListTailPopAndPushAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPopAndPushAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                await client.ListPushAsync(list1, "Foo!".ToBytes(), "Bar!".ToBytes());

                var item = await client.ListTailPopAndPushAsync(list1, list2);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));

                item = await client.ListPopAsync(list2);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListTailPopAndPushNullAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListTailPopAndPushAsync);

                var item = await client.ListTailPopAndPushAsync(listKey + "NotExists", listKey);
                Assert.Null(item);
            }
        }

        [Fact]
        public async Task ListBlockingTailPopAndPushAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListBlockingTailPopAndPushAsync);
                const string list1 = listKey + "1";
                const string list2 = listKey + "2";
                await client.DeleteKeyAsync(list1, list2);
                await client.ListPushAsync(list1, "Foo!".ToBytes(), "Bar!".ToBytes());

                var item = await client.ListBlockingTailPopAndPushAsync(list1, list2, 0);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));

                item = await client.ListPopAsync(list2);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListIndexAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListIndexAsync);
                await client.DeleteKeyAsync(listKey);
                var count = await client.ListPushAsync(listKey, "Foo!".ToBytes(), "Bar!".ToBytes());
                Assert.Equal(2, count);

                var item = await client.ListIndexAsync(listKey, -1);
                Assert.Equal("Foo!", Encoding.UTF8.GetString(item));

                item = await client.ListIndexAsync(listKey, 0);
                Assert.Equal("Bar!", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListInsertBeforeAndAfterAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListInsertBeforeAndAfterAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "Foo!".ToBytes(), "Bar!".ToBytes());
                await client.ListInsertBeforeAsync(listKey, "Foo!".ToBytes(), "InsertBefore".ToBytes());
                await client.ListInsertAfterAsync(listKey, "InsertBefore".ToBytes(), "InsertAfter".ToBytes());

                var item = await client.ListIndexAsync(listKey, -2);
                Assert.Equal("InsertAfter", Encoding.UTF8.GetString(item));

                item = await client.ListIndexAsync(listKey, -3);
                Assert.Equal("InsertBefore", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListLengthAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListInsertBeforeAndAfterAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "Foo!".ToBytes(), "Bar!".ToBytes());
                var count = await client.ListLength(listKey);
                Assert.Equal(2, count);
            }
        }

        [Fact]
        public async Task ListRangeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListRangeAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "1".ToBytes(), "2".ToBytes(), "3".ToBytes());

                var items = await client.ListRangeAsync(listKey, 1, -1);
                Assert.Equal(2, items.Length);
                Assert.Equal("2", Encoding.UTF8.GetString(items[0]));
                Assert.Equal("1", Encoding.UTF8.GetString(items[1]));
            }
        }

        [Fact]
        public async Task ListRemoveAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListRemoveAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "1".ToBytes(), "2".ToBytes(), "2".ToBytes(), "3".ToBytes());

                var count = await client.ListRemoveAsync(listKey, 1, "2".ToBytes());
                Assert.Equal(1, count);

                var items = await client.ListRangeAsync(listKey, 0, -1);
                Assert.Equal("3", Encoding.UTF8.GetString(items[0]));
                Assert.Equal("2", Encoding.UTF8.GetString(items[1]));
                Assert.Equal("1", Encoding.UTF8.GetString(items[2]));
            }
        }

        [Fact]
        public async Task ListSetAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListSetAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "1".ToBytes(), "2".ToBytes(), "2".ToBytes(), "3".ToBytes());

                await client.ListSetAsync(listKey, 1, "Update".ToBytes());

                var item = await client.ListIndexAsync(listKey, 1);
                Assert.Equal("Update", Encoding.UTF8.GetString(item));
            }
        }

        [Fact]
        public async Task ListTrimAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string listKey = nameof(ListSetAsync);
                await client.DeleteKeyAsync(listKey);
                await client.ListPushAsync(listKey, "1".ToBytes(), "2".ToBytes(), "3".ToBytes());

                await client.ListTrimAsync(listKey, 0, 1);

                var item = await client.ListIndexAsync(listKey, 0);
                Assert.Equal("3", Encoding.UTF8.GetString(item));
            }
        }
    }
}
