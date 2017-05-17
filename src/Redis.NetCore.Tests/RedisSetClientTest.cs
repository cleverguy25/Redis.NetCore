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

                var count = await client.SetAddMemberAsync(setKey, "Foo".ToBytes(), "Bar".ToBytes());
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

                var count = await client.SetAddMemberAsync(setKey, "Foo".ToBytes(), "Bar".ToBytes());
                Assert.Equal(2, count);

                count = await client.SetAddMemberAsync(setKey, "Bar".ToBytes(), "FooBar".ToBytes());
                Assert.Equal(1, count);

                count = await client.SetCardinalityAsync(setKey);
                Assert.Equal(3, count);
            }
        }

        [Fact]
        public async Task SetDifferenceAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetDifferenceAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);

                await client.SetAddMemberAsync(setKey1, "Foo".ToBytes(), "Bar".ToBytes(), "FooBar".ToBytes());
                await client.SetAddMemberAsync(setKey2, "Bar".ToBytes());

                var bytes = await client.SetGetDifferenceMembersAsync(setKey1, setKey2);
                Assert.Equal(2, bytes.Length);
                Assert.Equal("Foo".ToBytes(), bytes[0]);
                Assert.Equal("FooBar".ToBytes(), bytes[1]);
            }
        }

        [Fact]
        public async Task SetIntersectionAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetIntersectionAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);

                await client.SetAddMemberAsync(setKey1, "Foo".ToBytes(), "Bar".ToBytes());
                await client.SetAddMemberAsync(setKey2, "Bar".ToBytes(), "2".ToBytes());

                var bytes = await client.SetGetIntersectionMembersAsync(setKey1, setKey2);
                Assert.Equal(1, bytes.Length);
                Assert.Equal("Bar".ToBytes(), bytes[0]);
            }
        }

        [Fact]
        public async Task SetUnionAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetUnionAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);

                await client.SetAddMemberAsync(setKey1, "Foo".ToBytes(), "Bar".ToBytes());
                await client.SetAddMemberAsync(setKey2, "Bar".ToBytes(), "FooBar".ToBytes());

                var bytes = await client.SetGetUnionMembersAsync(setKey1, setKey2);
                Assert.Equal(3, bytes.Length);
                Assert.Equal("Foo".ToBytes(), bytes[0]);
                Assert.Equal("Bar".ToBytes(), bytes[1]);
                Assert.Equal("FooBar".ToBytes(), bytes[2]);
            }
        }

        [Fact]
        public async Task SetDifferenceAndStoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetDifferenceAndStoreAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "Store";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SetAddMemberAsync(setKey1, "Foo".ToBytes(), "Bar".ToBytes(), "FooBar".ToBytes());
                await client.SetAddMemberAsync(setKey2, "Bar".ToBytes());

                var count = await client.SetStoreDifferenceMembersAsync(storeKey, setKey1, setKey2);
                Assert.Equal(2, count);

                var bytes = await client.SetGetMembersAsync(storeKey);
                Assert.Equal("Foo".ToBytes(), bytes[0]);
                Assert.Equal("FooBar".ToBytes(), bytes[1]);
            }
        }

        [Fact]
        public async Task SetIntersectionAndStoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetIntersectionAndStoreAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "Store";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SetAddMemberAsync(setKey1, "Foo".ToBytes(), "Bar".ToBytes());
                await client.SetAddMemberAsync(setKey2, "Bar".ToBytes(), "2".ToBytes());

                var count = await client.SetStoreIntersectionMembersAsync(storeKey, setKey1, setKey2);
                Assert.Equal(1, count);

                var bytes = await client.SetGetMembersAsync(storeKey);
                Assert.Equal(1, bytes.Length);
                Assert.Equal("Bar".ToBytes(), bytes[0]);
            }
        }

        [Fact]
        public async Task SetUnionAndStoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SetUnionAndStoreAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "Store";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SetAddMemberAsync(setKey1, "Foo".ToBytes(), "Bar".ToBytes());
                await client.SetAddMemberAsync(setKey2, "Bar".ToBytes(), "FooBar".ToBytes());

                var count = await client.SetStoreUnionMembersAsync(storeKey, setKey1, setKey2);
                Assert.Equal(3, count);

                var bytes = await client.SetGetMembersAsync(storeKey);
                Assert.Equal(3, bytes.Length);
                Assert.Equal("Foo".ToBytes(), bytes[0]);
                Assert.Equal("Bar".ToBytes(), bytes[1]);
                Assert.Equal("FooBar".ToBytes(), bytes[2]);
            }
        }
    }
}
