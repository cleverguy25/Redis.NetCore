using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [Trait("Category", "Integration")]
    public class RedisSortedSetStringClientTest
    {
        [Fact]
        public async Task SortedSetAddMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetAddMembersStringAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 5), ("Bar", 6));
                Assert.Equal(2, count);

                var score = await client.SortedSetGetScoreStringAsync(setKey, "Foo");
                Assert.Equal(5, score);

                score = await client.SortedSetGetScoreStringAsync(setKey, "Bar");
                Assert.Equal(6, score);
            }
        }

        [Fact]
        public async Task SortedSetAddOnlyMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetAddOnlyMembersStringAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 5), ("Bar", 6));
                Assert.Equal(2, count);

                count = await client.SortedSetAddOnlyMembersStringAsync(setKey, ("Foo", 7), ("FooBar", 8));
                Assert.Equal(1, count);

                var score = await client.SortedSetGetScoreStringAsync(setKey, "FooBar");
                Assert.Equal(8, score);
            }
        }

        [Fact]
        public async Task SortedSetUpsertMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetUpsertMembersStringAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetUpsertMembersStringAsync(setKey, ("Foo", 5), ("Bar", 6));
                Assert.Equal(2, count);

                count = await client.SortedSetUpsertMembersStringAsync(setKey, ("Foo", 7), ("FooBar", 8), ("Bar", 6));
                Assert.Equal(2, count);

                var cardinality = await client.SortedSetCardinalityAsync(setKey);
                Assert.Equal(3, cardinality);
            }
        }

        [Fact]
        public async Task SortedSetUpdateMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetUpdateMembersStringAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 5), ("Bar", 6));
                Assert.Equal(2, count);

                count = await client.SortedSetUpdateMembersStringAsync(setKey, ("Foo", 7), ("FooBar", 6));
                Assert.Equal(0, count);

                var score = await client.SortedSetGetScoreStringAsync(setKey, "Foo");
                Assert.Equal(7, score);

                score = await client.SortedSetGetScoreStringAsync(setKey, "FooBar");
                Assert.Null(score);
            }
        }

        [Fact]
        public async Task SortedSetIncrementByStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetIncrementByStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 5), ("Bar", 6));

                var value = await client.SortedSetIncrementByStringAsync(setKey, "Foo", 3);
                Assert.Equal(8, value);

                var score = await client.SortedSetGetScoreStringAsync(setKey, "Foo");
                Assert.Equal(8, score);
            }
        }
    }
}
