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
    public class RedisSortedSetClientTest
    {
        [Fact]
        public async Task SortedSetAddMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetAddMembersAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6));
                Assert.Equal(2, count);

                var score = await client.SortedSetGetScoreAsync(setKey, "Foo".ToBytes());
                Assert.Equal(5, score);

                score = await client.SortedSetGetScoreAsync(setKey, "Bar".ToBytes());
                Assert.Equal(6, score);
            }
        }

        [Fact]
        public async Task SortedSetUpdateMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetUpdateMembersAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6));
                Assert.Equal(2, count);

                count = await client.SortedSetUpdateMembersAsync(setKey, ("Foo".ToBytes(), 7), ("FooBar".ToBytes(), 6));
                Assert.Equal(0, count);

                var score = await client.SortedSetGetScoreAsync(setKey, "Foo".ToBytes());
                Assert.Equal(7, score);

                score = await client.SortedSetGetScoreAsync(setKey, "FooBar".ToBytes());
                Assert.Null(score);
            }
        }

        [Fact]
        public async Task SortedSetAddOnlyMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetAddOnlyMembersAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6));
                Assert.Equal(2, count);

                count = await client.SortedSetAddOnlyMembersAsync(setKey, ("Foo".ToBytes(), 7), ("FooBar".ToBytes(), 8));
                Assert.Equal(1, count);

                var score = await client.SortedSetGetScoreAsync(setKey, "FooBar".ToBytes());
                Assert.Equal(8, score);
            }
        }

        [Fact]
        public async Task SortedSetUpsertMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetUpsertMembersAsync);
                await client.DeleteKeyAsync(setKey);

                var count = await client.SortedSetUpsertMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6));
                Assert.Equal(2, count);

                count = await client.SortedSetUpsertMembersAsync(setKey, ("Foo".ToBytes(), 7), ("FooBar".ToBytes(), 8), ("Bar".ToBytes(), 6));
                Assert.Equal(2, count);

                var cardinality = await client.SortedSetCardinalityAsync(setKey);
                Assert.Equal(3, cardinality);
            }
        }

        [Fact]
        public async Task SortedSetCardinalityAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetCardinalityAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6));

                var count = await client.SortedSetCardinalityAsync(setKey);
                Assert.Equal(2, count);
            }
        }
    }
}
