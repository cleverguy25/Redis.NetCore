using System;
using System.Collections.Generic;
using System.Linq;
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

                var count = await client.SetAddMemberStringAsync(setKey, "Foo", "Bar");
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

                await client.SetAddMemberStringAsync(setKey1, "Foo", "Bar", "FooBar");
                await client.SetAddMemberStringAsync(setKey2, "Bar");

                var values = await client.SetGetDifferenceMembersStringAsync(setKey1, setKey2);
                Array.Sort(values);
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

                await client.SetAddMemberStringAsync(setKey1, "Foo", "Bar");
                await client.SetAddMemberStringAsync(setKey2, "Bar", "2");

                var values = await client.SetGetIntersectionMembersStringAsync(setKey1, setKey2);
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

                await client.SetAddMemberStringAsync(setKey1, "Foo", "Bar");
                await client.SetAddMemberStringAsync(setKey2, "Bar", "FooBar");

                var values = await client.SetGetUnionMembersStringAsync(setKey1, setKey2);
                var sortedValues = values.OrderBy(item => item).ToArray();
                Assert.Equal(3, values.Length);
                Assert.Equal("Bar", sortedValues[0]);
                Assert.Equal("Foo", sortedValues[1]);
                Assert.Equal("FooBar", sortedValues[2]);
            }
        }

        [Fact]
        public async Task SetGetMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetGetMembersStringAsync);
                await client.DeleteKeyAsync(setKey);
                await client.SetAddMemberStringAsync(setKey, "Foo", "Bar");

                var values = await client.SetGetMembersStringAsync(setKey);
                var sortedValues = values.OrderBy(item => item).ToArray();
                Assert.Equal(2, values.Length);
                Assert.Equal("Bar", sortedValues[0]);
                Assert.Equal("Foo", sortedValues[1]);
            }
        }

        [Fact]
        public async Task SetMoveStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetMoveStringAsync);
                const string sourceKey = setKey + "Source";
                const string destKey = setKey + "Destination";
                await client.DeleteKeyAsync(sourceKey);
                await client.DeleteKeyAsync(destKey);

                await client.SetAddMemberStringAsync(sourceKey, "Foo", "Bar");

                var moved = await client.SetMoveMemberStringAsync(sourceKey, destKey, "Foo");
                Assert.True(moved);

                var isMember = await client.SetIsMemberStringAsync(destKey, "Foo");
                Assert.True(isMember);

                isMember = await client.SetIsMemberStringAsync(sourceKey, "Foo");
                Assert.False(isMember);
            }
        }

        [Fact]
        public async Task SetPopStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetPopStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SetAddMemberStringAsync(setKey, "Foo", "Bar");

                var value = await client.SetPopMemberStringAsync(setKey);

                var isMember = await client.SetIsMemberStringAsync(setKey, value);
                Assert.False(isMember);
            }
        }

        [Fact]
        public async Task SetGetRandomMemberStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetGetRandomMemberStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SetAddMemberStringAsync(setKey, "Foo", "Bar");

                var values = await client.SetGetRandomMemberStringAsync(setKey, 2);

                var isMember = await client.SetIsMemberStringAsync(setKey, values[0]);
                Assert.True(isMember);
            }
        }

        [Fact]
        public async Task SetRemoveMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetRemoveMembersStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SetAddMemberStringAsync(setKey, "Foo", "Bar", "FooBar");

                var count = await client.SetRemoveMembersStringAsync(setKey, "Bar", "Foo", "Test");
                Assert.Equal(2, count);

                var members = await client.SetGetMembersStringAsync(setKey);
                Assert.Equal("FooBar", members[0]);
            }
        }
    }
}
