// <copyright file="RedisHyperLogLogClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>using System.Threading.Tasks;

using System.Threading.Tasks;
using Redis.NetCore.Configuration;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisHyperLogLogClientTest
    {
        [Fact]
        public async Task HyperLogLogAddCountAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(HyperLogLogAddCountAsync);
                await client.DeleteKeyAsync(key);
                var altered = await client.HyperLogLogAddAsync(key, "foo".ToBytes(), "bar".ToBytes(), "zap".ToBytes());
                Assert.Equal(true, altered);

                altered = await client.HyperLogLogAddAsync(key, "zap".ToBytes(), "zap".ToBytes(), "zap".ToBytes());
                Assert.Equal(false, altered);

                var count = await client.HyperLogLogCountAsync(key);
                Assert.Equal(3, count);
            }
        }

        [Fact]
        public async Task HyperLogLogAddCountStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(HyperLogLogAddCountStringAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                await client.DeleteKeyAsync(key1, key2);
                var altered = await client.HyperLogLogAddStringAsync(key1, "foo", "bar", "zap");
                Assert.Equal(true, altered);

                altered = await client.HyperLogLogAddStringAsync(key2, "zap", "zap", "zap");
                Assert.Equal(true, altered);

                var count = await client.HyperLogLogCountAsync(key1, key2);
                Assert.Equal(3, count);
            }
        }

        [Fact]
        public async Task HyperLogLogMergeAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string key = nameof(HyperLogLogMergeAsync);
                const string key1 = key + "1";
                const string key2 = key + "2";
                const string key3 = key + "3";
                await client.DeleteKeyAsync(key1, key2, key3);
                var altered = await client.HyperLogLogAddStringAsync(key1, "foo", "bar", "zap", "a");
                Assert.Equal(true, altered);

                altered = await client.HyperLogLogAddStringAsync(key2, "a", "b", "c", "foo");
                Assert.Equal(true, altered);

                await client.HyperLogLogMergeAsync(key3, key1, key2);

                var count = await client.HyperLogLogCountAsync(key3);
                Assert.Equal(6, count);
            }
        }
    }
}
