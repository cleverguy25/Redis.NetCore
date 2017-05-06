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
    }
}
