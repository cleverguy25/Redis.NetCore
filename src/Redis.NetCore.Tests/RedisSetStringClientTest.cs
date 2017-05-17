using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisSetStringClientTest
    {
        [Fact]
        public async Task SetAddStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetAddStringAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SetAddStringAsync(setKey, "Foo", "Bar");
                Assert.Equal(2, count);

                var isMember = await client.SetIsMemberStringAsync(setKey, "FooBar");
                Assert.False(isMember);

                isMember = await client.SetIsMemberStringAsync(setKey, "Bar");
                Assert.True(isMember);
            }
        }
    }
}
