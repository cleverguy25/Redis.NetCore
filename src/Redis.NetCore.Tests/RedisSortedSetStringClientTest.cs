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

        [Fact]
        public async Task SortedSetGetRangeStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetRangeStringAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two", items[0]);
                Assert.Equal("three", items[1]);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeWithScoresStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeWithScoresStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetRangeWithScoresStringAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two", items[0].Member);
                Assert.Equal(2, items[0].Weight);
                Assert.Equal("three", items[1].Member);
                Assert.Equal(3, items[1].Weight);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetReverseRangeStringAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two", items[0]);
                Assert.Equal("one", items[1]);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeWithScoresStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeWithScoresStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetReverseRangeWithScoresStringAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two", items[0].Member);
                Assert.Equal(2, items[0].Weight);
                Assert.Equal("one", items[1].Member);
                Assert.Equal(1, items[1].Weight);
            }
        }

        [Fact]
        public async Task SortedSetLexCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetLexCountAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("a", 0), ("b", 0), ("c", 0), ("d", 0), ("e", 0), ("f", 0));

                var count = await client.SortedSetGetLexCountAsync(setKey, "-", "+");
                Assert.Equal(6, count);

                count = await client.SortedSetGetLexCountAsync(setKey, "(b", "[f");
                Assert.Equal(4, count);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeByLexAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeByLexAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("a", 0), ("b", 0), ("c", 0), ("d", 0), ("e", 0), ("f", 0));

                var items = await client.SortedSetGetLexRangeAsync(setKey, "-", "+");
                Assert.Equal(new[] { "a", "b", "c", "d", "e", "f" }, items);

                items = await client.SortedSetGetLexRangeAsync(setKey, "-", "+", 2, 2);
                Assert.Equal(new[] { "c", "d" }, items);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeByLexAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeByLexAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("a", 0), ("b", 0), ("c", 0), ("d", 0), ("e", 0), ("f", 0));

                var items = await client.SortedSetGetReverseLexRangeAsync(setKey, "+", "-");
                Assert.Equal(new[] { "f", "e", "d", "c", "b", "a" }, items);

                items = await client.SortedSetGetReverseLexRangeAsync(setKey, "+", "-", 2, 2);
                Assert.Equal(new[] { "d", "c" }, items);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeByScoreStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeByScoreStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetRangeByScoreStringAsync(setKey, "2", "+inf");
                Assert.Equal(2, items.Length);

                Assert.Equal("two", items[0]);
                Assert.Equal("three", items[1]);

                items = await client.SortedSetGetRangeByScoreStringAsync(setKey, "-inf", "+inf", 1, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("two", items[0]);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeByScoreWithScoresAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeByScoreWithScoresAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetRangeByScoreWithScoresStringAsync(setKey, "2", "+inf");
                Assert.Equal(2, items.Length);

                Assert.Equal("two", items[0].Member);
                Assert.Equal(2, items[0].Weight);
                Assert.Equal("three", items[1].Member);
                Assert.Equal(3, items[1].Weight);

                items = await client.SortedSetGetRangeByScoreWithScoresStringAsync(setKey, "-inf", "+inf", 1, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("two", items[0].Member);
                Assert.Equal(2, items[0].Weight);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeByScoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeByScoreAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetReverseRangeByScoreStringAsync(setKey, "+inf", "2");
                Assert.Equal(2, items.Length);

                Assert.Equal("three", items[0]);
                Assert.Equal("two", items[1]);

                items = await client.SortedSetGetReverseRangeByScoreStringAsync(setKey, "+inf", "-inf", 2, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("one", items[0]);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeByScoreWithScoresAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeByScoreWithScoresAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("one", 1), ("two", 2), ("three", 3));

                var items = await client.SortedSetGetReverseRangeByScoreWithScoresStringAsync(setKey, "+inf", "2");
                Assert.Equal(2, items.Length);

                Assert.Equal("three", items[0].Member);
                Assert.Equal(3, items[0].Weight);
                Assert.Equal("two", items[1].Member);
                Assert.Equal(2, items[1].Weight);

                items = await client.SortedSetGetReverseRangeByScoreWithScoresStringAsync(setKey, "+inf", "-inf", 2, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("one", items[0].Member);
                Assert.Equal(1, items[0].Weight);
            }
        }
    }
}