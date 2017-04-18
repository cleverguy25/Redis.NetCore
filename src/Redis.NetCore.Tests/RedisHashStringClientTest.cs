// <copyright file="RedisRawClientTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "Integration")]
    public class RedisHashStringClientTest
    {
        [Fact]
        public async Task HashMulitpleGetStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashMulitpleGetStringAsync);
                const string hashKey = "Hash" + field;
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.HashSetFieldStringAsync(hashKey, field + "1", expected1);
                await client.HashSetFieldStringAsync(hashKey, field + "2", expected2);
                var values = await client.HashGetFieldsStringAsync(hashKey, field + "1", field + "2", "NoKey");
                Assert.Equal(expected1, values[0]);
                Assert.Equal(expected2, values[1]);
                Assert.Equal(null, values[2]);
            }
        }

        [Fact]
        public async Task HashGetAllStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashGetAllStringAsync);
                const string hashKey = "Hash" + field;
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.HashSetFieldStringAsync(hashKey, field + "1", expected1);
                await client.HashSetFieldStringAsync(hashKey, field + "2", expected2);
                var values = await client.HashGetAllFieldsStringAsync(hashKey);
                Assert.Equal(field + "1", values[0]);
                Assert.Equal(expected1, values[1]);
                Assert.Equal(field + "2", values[2]);
                Assert.Equal(expected2, values[3]);
            }
        }

        [Fact]
        public async Task HashGetValuesStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashGetAllStringAsync);
                const string hashKey = "Hash" + field;
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.HashSetFieldStringAsync(hashKey, field + "1", expected1);
                await client.HashSetFieldStringAsync(hashKey, field + "2", expected2);
                var values = await client.HashGetValuesStringAsync(hashKey);
                Assert.Equal(expected1, values[0]);
                Assert.Equal(expected2, values[1]);
            }
        }

        [Fact]
        public async Task HashSetStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string field = nameof(HashSetStringAsync);
                const string hashKey = "Hash" + field;
                await client.HashSetFieldStringAsync(hashKey, field, expected);
                var value = await client.HashGetFieldStringAsync(hashKey, field);
                Assert.Equal(expected, value);
            }
        }

        [Fact]
        public async Task HashSetNotExistsStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string field = nameof(HashSetNotExistsStringAsync);
                const string hashKey = "Hash" + field;
                await client.DeleteKeyAsync(hashKey);
                var set = await client.HashSetFieldStringNotExistsAsync(hashKey, field, expected);
                Assert.Equal(true, set);
                var value = await client.HashGetFieldStringAsync(hashKey, field);
                Assert.Equal(expected, value);

                set = await client.HashSetFieldStringNotExistsAsync(hashKey, field, "Bar!");
                Assert.Equal(false, set);
                value = await client.HashGetFieldStringAsync(hashKey, field);
                Assert.Equal(expected, value);
            }
        }

        [Fact]
        public async Task HashMultipleSetStringAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashMultipleSetStringAsync);
                const string hashKey = "Hash" + field;
                const string field1 = field + "1";
                const string field2 = field + "2";
                var data = new Dictionary<string, string>
                           {
                               { field1, "Foo1" },
                               { field2, "Foo2" }
                           };
                await client.HashSetFieldsStringAsync(hashKey, data);
                var value1 = await client.HashGetFieldStringAsync(hashKey, field1);
                var value2 = await client.HashGetFieldStringAsync(hashKey, field2);
                Assert.Equal("Foo1", value1);
                Assert.Equal("Foo2", value2);
            }
        }
    }
}