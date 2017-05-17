using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisSetClientTest
    {
        [Fact]
        public async Task SetAddAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetAddAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SetAddAsync(setKey, "Foo".ToBytes(), "Bar".ToBytes());
                Assert.Equal(2, count);

                var isMember = await client.SetIsMemberAsync(setKey, "FooBar".ToBytes());
                Assert.False(isMember);

                isMember = await client.SetIsMemberAsync(setKey, "Bar".ToBytes());
                Assert.True(isMember);
            }
        }

        [Fact]
        public async Task SetCardinalityAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetCardinalityAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SetAddAsync(setKey, "Foo".ToBytes(), "Bar".ToBytes());
                Assert.Equal(2, count);

                count = await client.SetAddAsync(setKey, "Bar".ToBytes(), "FooBar".ToBytes());
                Assert.Equal(1, count);

                count = await client.SetCardinalityAsync(setKey);
                Assert.Equal(3, count);
            }
        }
    }
}
