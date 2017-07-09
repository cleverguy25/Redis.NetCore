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

        [Fact]
        public async Task SortedSetCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetCountAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 3), ("Bar".ToBytes(), 6), ("FooBar".ToBytes(), 8));

                var count = await client.SortedSetGetCountAsync(setKey, "3", "6");
                Assert.Equal(2, count);

                count = await client.SortedSetGetCountAsync(setKey, "(3", "6");
                Assert.Equal(1, count);

                count = await client.SortedSetGetCountAsync(setKey, "3", "(6");
                Assert.Equal(1, count);

                count = await client.SortedSetGetCountAsync(setKey, "-inf", "+inf");
                Assert.Equal(3, count);
            }
        }

        [Fact]
        public async Task SortedSetGetRankAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRankAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 3), ("Bar".ToBytes(), 6), ("FooBar".ToBytes(), 8));

                var rank = await client.SortedSetGetRankAsync(setKey, "Bar".ToBytes());
                Assert.Equal(1, rank);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRankAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRankAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 3), ("Bar".ToBytes(), 6), ("FooBar".ToBytes(), 8));

                var rank = await client.SortedSetGetReverseRankAsync(setKey, "FooBar".ToBytes());
                Assert.Equal(0, rank);
            }
        }

        [Fact]
        public async Task SortedSetIncrementByAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetIncrementByAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6));

                var value = await client.SortedSetIncrementByAsync(setKey, "Foo".ToBytes(), 3);
                Assert.Equal(8, value);

                var score = await client.SortedSetGetScoreAsync(setKey, "Foo".ToBytes());
                Assert.Equal(8, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreIntersectionMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreIntersectionMembersAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var count = await client.SortedSetStoreIntersectionMembersAsync(storeKey, new[] { setKey1, setKey2 });
                Assert.Equal(2, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(4, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreIntersectionMembersWithAggregateAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreIntersectionMembersWithAggregateAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 2), ("two".ToBytes(), 3), ("three".ToBytes(), 4));

                var count = await client.SortedSetStoreIntersectionMembersAsync(storeKey, new[] { setKey1, setKey2 }, RedisAggregate.Min);
                Assert.Equal(2, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(1, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(2, score);

                await client.DeleteKeyAsync(storeKey);

                count = await client.SortedSetStoreIntersectionMembersAsync(storeKey, new[] { setKey1, setKey2 }, RedisAggregate.Max);
                Assert.Equal(2, count);

                score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(3, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreIntersectionMembersWithAggregateAndWeightsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreIntersectionMembersWithAggregateAndWeightsAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 2), ("two".ToBytes(), 3), ("three".ToBytes(), 4));

                var count = await client.SortedSetStoreIntersectionMembersAsync(storeKey, new[] { (setKey1, 2), (setKey2, 3) }, RedisAggregate.Min);
                Assert.Equal(2, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(4, score);

                await client.DeleteKeyAsync(storeKey);

                count = await client.SortedSetStoreIntersectionMembersAsync(storeKey, new[] { (setKey1, 2), (setKey2, 3) }, RedisAggregate.Max);
                Assert.Equal(2, count);

                score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(6, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(9, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreIntersectionMembersWeightsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreIntersectionMembersWeightsAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var count = await client.SortedSetStoreIntersectionMembersAsync(storeKey, new[] { (setKey1, 2), (setKey2, 3) });
                Assert.Equal(2, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(5, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(10, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreUnionMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreUnionMembersAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var count = await client.SortedSetStoreUnionMembersAsync(storeKey, new[] { setKey1, setKey2 });
                Assert.Equal(3, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(4, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "three".ToBytes());
                Assert.Equal(3, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreUnionMembersWithAggregateAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreUnionMembersWithAggregateAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 2), ("two".ToBytes(), 3), ("three".ToBytes(), 4));

                var count = await client.SortedSetStoreUnionMembersAsync(storeKey, new[] { setKey1, setKey2 }, RedisAggregate.Min);
                Assert.Equal(3, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(1, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "three".ToBytes());
                Assert.Equal(4, score);

                await client.DeleteKeyAsync(storeKey);

                count = await client.SortedSetStoreUnionMembersAsync(storeKey, new[] { setKey1, setKey2 }, RedisAggregate.Max);
                Assert.Equal(3, count);

                score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(3, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "three".ToBytes());
                Assert.Equal(4, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreUnionMembersWithAggregateAndWeightsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreUnionMembersWithAggregateAndWeightsAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 2), ("two".ToBytes(), 3), ("three".ToBytes(), 4));

                var count = await client.SortedSetStoreUnionMembersAsync(storeKey, new[] { (setKey1, 2), (setKey2, 3) }, RedisAggregate.Min);
                Assert.Equal(3, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(2, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(4, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "three".ToBytes());
                Assert.Equal(12, score);

                await client.DeleteKeyAsync(storeKey);

                count = await client.SortedSetStoreUnionMembersAsync(storeKey, new[] { (setKey1, 2), (setKey2, 3) }, RedisAggregate.Max);
                Assert.Equal(3, count);

                score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(6, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(9, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "three".ToBytes());
                Assert.Equal(12, score);
            }
        }

        [Fact]
        public async Task SortedSetStoreUnionMembersWeightsAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetStoreUnionMembersWeightsAsync);
                const string setKey1 = setKey + "1";
                const string setKey2 = setKey + "2";
                const string storeKey = setKey + "3";
                await client.DeleteKeyAsync(setKey1);
                await client.DeleteKeyAsync(setKey2);
                await client.DeleteKeyAsync(storeKey);

                await client.SortedSetAddMembersAsync(setKey1, ("one".ToBytes(), 1), ("two".ToBytes(), 2));
                await client.SortedSetAddMembersAsync(setKey2, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var count = await client.SortedSetStoreUnionMembersAsync(storeKey, new[] { (setKey1, 2), (setKey2, 3) });
                Assert.Equal(3, count);

                var score = await client.SortedSetGetScoreAsync(storeKey, "one".ToBytes());
                Assert.Equal(5, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "two".ToBytes());
                Assert.Equal(10, score);

                score = await client.SortedSetGetScoreAsync(storeKey, "three".ToBytes());
                Assert.Equal(9, score);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetRangeAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two".ToBytes(), items[0]);
                Assert.Equal("three".ToBytes(), items[1]);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeWithScoresAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeWithScoresAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetRangeWithScoresAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two".ToBytes(), items[0].Member);
                Assert.Equal(2, items[0].Weight);
                Assert.Equal("three".ToBytes(), items[1].Member);
                Assert.Equal(3, items[1].Weight);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetReverseRangeAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two".ToBytes(), items[0]);
                Assert.Equal("one".ToBytes(), items[1]);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeWithScoresAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeWithScoresAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetReverseRangeWithScoresAsync(setKey, 1, -1);
                Assert.Equal(2, items.Length);

                Assert.Equal("two".ToBytes(), items[0].Member);
                Assert.Equal(2, items[0].Weight);
                Assert.Equal("one".ToBytes(), items[1].Member);
                Assert.Equal(1, items[1].Weight);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeByScoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeByScoreAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetRangeByScoreAsync(setKey, "2", "+inf");
                Assert.Equal(2, items.Length);

                Assert.Equal("two".ToBytes(), items[0]);
                Assert.Equal("three".ToBytes(), items[1]);

                items = await client.SortedSetGetRangeByScoreAsync(setKey, "-inf", "+inf", 1, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("two".ToBytes(), items[0]);
            }
        }

        [Fact]
        public async Task SortedSetGetRangeByScoreWithScoresAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRangeByScoreWithScoresAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetRangeByScoreWithScoresAsync(setKey, "2", "+inf");
                Assert.Equal(2, items.Length);

                Assert.Equal("two".ToBytes(), items[0].Member);
                Assert.Equal(2, items[0].Weight);
                Assert.Equal("three".ToBytes(), items[1].Member);
                Assert.Equal(3, items[1].Weight);

                items = await client.SortedSetGetRangeByScoreWithScoresAsync(setKey, "-inf", "+inf", 1, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("two".ToBytes(), items[0].Member);
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

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetReverseRangeByScoreAsync(setKey, "+inf", "2");
                Assert.Equal(2, items.Length);

                Assert.Equal("three".ToBytes(), items[0]);
                Assert.Equal("two".ToBytes(), items[1]);

                items = await client.SortedSetGetReverseRangeByScoreAsync(setKey, "+inf", "-inf", 2, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("one".ToBytes(), items[0]);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRangeByScoreWithScoresAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRangeByScoreWithScoresAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("one".ToBytes(), 1), ("two".ToBytes(), 2), ("three".ToBytes(), 3));

                var items = await client.SortedSetGetReverseRangeByScoreWithScoresAsync(setKey, "+inf", "2");
                Assert.Equal(2, items.Length);

                Assert.Equal("three".ToBytes(), items[0].Member);
                Assert.Equal(3, items[0].Weight);
                Assert.Equal("two".ToBytes(), items[1].Member);
                Assert.Equal(2, items[1].Weight);

                items = await client.SortedSetGetReverseRangeByScoreWithScoresAsync(setKey, "+inf", "-inf", 2, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("one".ToBytes(), items[0].Member);
                Assert.Equal(1, items[0].Weight);
            }
        }

        [Fact]
        public async Task SortedSetRemoveMembersAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetRemoveMembersAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6), ("FooBar".ToBytes(), 8));

                var count = await client.SortedSetRemoveMembersAsync(setKey, "Foo".ToBytes(), "FooBar".ToBytes());
                Assert.Equal(2, count);

                var items = await client.SortedSetGetRangeAsync(setKey, 0, -1);
                Assert.Equal(new[] { "Bar".ToBytes() }, items);
            }
        }

        [Fact]
        public async Task SortedSetRemoveRangeByScoreAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetRemoveRangeByScoreAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6), ("FooBar".ToBytes(), 8));

                var count = await client.SortedSetRemoveRangeByScoreAsync(setKey, "6", "+inf");
                Assert.Equal(2, count);

                var items = await client.SortedSetGetRangeAsync(setKey, 0, -1);
                Assert.Equal(new[] { "Foo".ToBytes() }, items);
            }
        }

        [Fact]
        public async Task SortedSetRemoveRangeByRankAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetRemoveRangeByRankAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 5), ("Bar".ToBytes(), 6), ("FooBar".ToBytes(), 8));

                var count = await client.SortedSetRemoveRangeByRankAsync(setKey, 0, 1);
                Assert.Equal(2, count);

                var items = await client.SortedSetGetRangeAsync(setKey, 0, -1);
                Assert.Equal(new[] { "FooBar".ToBytes() }, items);
            }
        }
    }
}