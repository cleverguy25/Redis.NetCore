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
    }
}
