using System;
using System.Collections.Generic;
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
    }
}
