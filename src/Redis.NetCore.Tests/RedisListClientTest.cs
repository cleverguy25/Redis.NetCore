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
    }
}
