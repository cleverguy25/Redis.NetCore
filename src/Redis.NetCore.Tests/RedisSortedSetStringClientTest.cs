// <copyright file="RedisSortedSetStringClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    [SuppressMessage("StyleCop", "SA1009", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
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

                Assert.Equal("two", items[0].member);
                Assert.Equal(2, items[0].weight);
                Assert.Equal("three", items[1].member);
                Assert.Equal(3, items[1].weight);
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

                Assert.Equal("two", items[0].member);
                Assert.Equal(2, items[0].weight);
                Assert.Equal("one", items[1].member);
                Assert.Equal(1, items[1].weight);
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

                Assert.Equal("two", items[0].member);
                Assert.Equal(2, items[0].weight);
                Assert.Equal("three", items[1].member);
                Assert.Equal(3, items[1].weight);

                items = await client.SortedSetGetRangeByScoreWithScoresStringAsync(setKey, "-inf", "+inf", 1, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("two", items[0].member);
                Assert.Equal(2, items[0].weight);
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

                Assert.Equal("three", items[0].member);
                Assert.Equal(3, items[0].weight);
                Assert.Equal("two", items[1].member);
                Assert.Equal(2, items[1].weight);

                items = await client.SortedSetGetReverseRangeByScoreWithScoresStringAsync(setKey, "+inf", "-inf", 2, 1);
                Assert.Equal(1, items.Length);

                Assert.Equal("one", items[0].member);
                Assert.Equal(1, items[0].weight);
            }
        }

        [Fact]
        public async Task SortedSetGetRankStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetRankStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 3), ("Bar", 6), ("FooBar", 8));

                var rank = await client.SortedSetGetRankStringAsync(setKey, "Bar");
                Assert.Equal(1, rank);
            }
        }

        [Fact]
        public async Task SortedSetGetReverseRankStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetGetReverseRankStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 3), ("Bar", 6), ("FooBar", 8));

                var rank = await client.SortedSetGetReverseRankStringAsync(setKey, "FooBar");
                Assert.Equal(0, rank);
            }
        }

        [Fact]
        public async Task SortedSetRemoveMembersStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetRemoveMembersStringAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersStringAsync(setKey, ("Foo", 5), ("Bar", 6), ("FooBar", 8));

                var count = await client.SortedSetRemoveMembersStringAsync(setKey, "Foo", "FooBar");
                Assert.Equal(2, count);

                var items = await client.SortedSetGetRangeStringAsync(setKey, 0, -1);
                Assert.Equal(new[] { "Bar" }, items);
            }
        }

        [Fact]
        public async Task SortedSetRemoveRangeByLexAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetRemoveRangeByLexAsync);
                await client.DeleteKeyAsync(setKey);

                await client.SortedSetAddMembersAsync(setKey, ("Foo".ToBytes(), 0), ("Bar".ToBytes(), 0), ("FooBar".ToBytes(), 0));

                var count = await client.SortedSetRemoveRangeByLexAsync(setKey, "[Foo", "+");
                Assert.Equal(2, count);

                var items = await client.SortedSetGetRangeAsync(setKey, 0, -1);
                Assert.Equal(new[] { "Bar".ToBytes() }, items);
            }
        }

        [Fact]
        public async Task SortedSetScanStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetScanStringAsync);
                var data = TestClient.SetupTestSetItems();
                await client.SortedSetAddMembersAsync(setKey, data.ToArray());

                var cursor = await client.SortedSetScanAsync(setKey);
                var items = cursor.GetMembersString();
                CheckItems(items);

                cursor = await client.SortedSetScanAsync(setKey, cursor);
                items = cursor.GetMembersString();
                CheckItems(items);
            }
        }

        [Fact]
        public async Task SortedSetScanWithCountStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetScanWithCountStringAsync);
                var data = TestClient.SetupTestSetItems();
                await client.SortedSetAddMembersAsync(setKey, data.ToArray());

                var cursor = await client.SortedSetScanAsync(setKey, 5);
                var items = cursor.GetMembersString();
                CheckItems(items);

                cursor = await client.SortedSetScanAsync(setKey, cursor, 5);
                items = cursor.GetMembersString();
                CheckItems(items);
            }
        }

        [Fact]
        public async Task SortedSetScanWithMatchStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetScanWithMatchStringAsync);
                var data = TestClient.SetupTestSetItems();
                await client.SortedSetAddMembersAsync(setKey, data.ToArray());

                var cursor = await client.SortedSetScanAsync(setKey, "match*");
                var items = cursor.GetMembersString();
                CheckItems(items);

                cursor = await client.SortedSetScanAsync(setKey, cursor, "match*");
                items = cursor.GetMembersString();
                CheckItems(items);
            }
        }

        [Fact]
        public async Task SortedSetScanWithMatchAndCountStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string setKey = nameof(SortedSetScanWithMatchStringAsync);
                var data = TestClient.SetupTestSetItems();
                await client.SortedSetAddMembersAsync(setKey, data.ToArray());

                var cursor = await client.SortedSetScanAsync(setKey, "match*", 5);
                var items = cursor.GetMembersString();
                CheckItems(items);

                cursor = await client.SortedSetScanAsync(setKey, cursor, "match*", 5);
                items = cursor.GetMembersString();
                CheckItems(items);
            }
        }

        private static void CheckItems(IEnumerable<(string member, double weight)> items)
        {
            foreach (var item in items)
            {
                var lastChar = item.member.Last();
                var weight = item.weight.ToString(CultureInfo.InvariantCulture).Last();
                Assert.Equal(weight, (char)lastChar);
            }
        }
    }
}