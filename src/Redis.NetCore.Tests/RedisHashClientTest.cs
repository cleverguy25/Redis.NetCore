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
    public class RedisHashClientTest
    {
        [Fact]
        public async Task HashMulitpleGetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashMulitpleGetBytesAsync);
                const string hashKey = "Hash" + field;
                const string expected1 = "Foo!";
                const string expected2 = "Bar!";
                await client.HashSetFieldAsync(hashKey, field + "1", Encoding.UTF8.GetBytes(expected1));
                await client.HashSetFieldAsync(hashKey, field + "2", Encoding.UTF8.GetBytes(expected2));
                var bytes = await client.HashGetFieldsAsync(hashKey, field + "1", field + "2", "NoKey");
                Assert.Equal(expected1, Encoding.UTF8.GetString(bytes[0]));
                Assert.Equal(expected2, Encoding.UTF8.GetString(bytes[1]));
                Assert.Equal(null, bytes[2]);
            }
        }

        [Fact]
        public async Task HashSetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string field = nameof(HashSetBytesAsync);
                const string hashKey = "Hash" + field;
                await client.HashSetFieldAsync(hashKey, field, Encoding.UTF8.GetBytes(expected));
                var bytes = await client.HashGetFieldAsync(hashKey, field);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task HashSetNotExistsBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string field = nameof(HashSetNotExistsBytesAsync);
                const string hashKey = "Hash" + field;
                await client.DeleteKeyAsync(hashKey);
                var set = await client.HashSetFieldNotExistsAsync(hashKey, field, Encoding.UTF8.GetBytes(expected));
                Assert.Equal(true, set);
                var bytes = await client.HashGetFieldAsync(hashKey, field);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));

                set = await client.HashSetFieldNotExistsAsync(hashKey, field, Encoding.UTF8.GetBytes("Bar!"));
                Assert.Equal(false, set);
                bytes = await client.HashGetFieldAsync(hashKey, field);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
            }
        }

        [Fact]
        public async Task HashMultipleSetBytesAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashMultipleSetBytesAsync);
                const string hashKey = "Hash" + field;
                const string field1 = field + "1";
                const string field2 = field + "2";
                var data = new Dictionary<string, byte[]>
                           {
                               { field1, "Foo1".ToBytes() },
                               { field2, "Foo2".ToBytes() }
                           };
                await client.HashSetFieldsAsync(hashKey, data);
                var value1 = await client.HashGetFieldAsync(hashKey, field1);
                var value2 = await client.HashGetFieldAsync(hashKey, field2);
                Assert.Equal("Foo1", Encoding.UTF8.GetString(value1));
                Assert.Equal("Foo2", Encoding.UTF8.GetString(value2));
            }
        }

        [Fact]
        public async Task HashDeleteAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string expected = "Foo!";
                const string field = nameof(HashDeleteAsync);
                const string hashKey = "Hash" + field;
                await client.HashSetFieldAsync(hashKey, field, Encoding.UTF8.GetBytes(expected));
                var bytes = await client.HashGetFieldAsync(hashKey, field);
                Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
                await client.HashDeleteFieldsAsync(hashKey, field);
                bytes = await client.HashGetFieldAsync(hashKey, field);
                Assert.Equal(null, bytes);
            }
        }

        [Fact]
        public async Task HashExistAsync()
        {
            using (var client = TestClient.CreateClient())
            {
                const string field = nameof(HashExistAsync);
                const string hashKey = "Hash" + field;
                await client.HashSetFieldAsync(hashKey, field, "FooExists!".ToBytes());
                var exists = await client.HashFieldExistsAsync(hashKey, field);
                Assert.Equal(true, exists);

                const string noExistsField = "NoExistsAsync";
                exists = await client.HashFieldExistsAsync(hashKey, noExistsField);
                Assert.Equal(false, exists);
            }
        }
    }
}