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

        [Fact]
        public async Task SetDifferenceStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetDifferenceStringAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);

                await client.SetAddStringAsync(setKey1, "Foo", "Bar", "FooBar");
                await client.SetAddStringAsync(setKey2, "Bar");

                var values = await client.SetDifferenceStringAsync(setKey1, setKey2);
                Assert.Equal(2, values.Length);
                Assert.Equal("Foo", values[0]);
                Assert.Equal("FooBar", values[1]);
            }
        }

        [Fact]
        public async Task SetIntersectionStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetIntersectionStringAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);

                await client.SetAddStringAsync(setKey1, "Foo", "Bar");
                await client.SetAddStringAsync(setKey2, "Bar", "2");

                var values = await client.SetIntersectionStringAsync(setKey1, setKey2);
                Assert.Equal(1, values.Length);
                Assert.Equal("Bar", values[0]);
            }
        }

        [Fact]
        public async Task SetUnionStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetUnionStringAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);

                await client.SetAddStringAsync(setKey1, "Foo", "Bar");
                await client.SetAddStringAsync(setKey2, "Bar", "FooBar");

                var values = await client.SetUnionStringAsync(setKey1, setKey2);
                Assert.Equal(3, values.Length);
                Assert.Equal("Foo", values[0]);
                Assert.Equal("Bar", values[1]);
                Assert.Equal("FooBar", values[2]);
            }
        }
    }
}
