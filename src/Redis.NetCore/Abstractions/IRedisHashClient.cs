// <copyright file="IRedisHashClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisHashClient
    {
        Task<bool> HashSetFieldAsync(string hashKey, string field, byte[] data);

        Task HashSetFieldsAsync(string hashKey, IDictionary<string, byte[]> dictionary);

        Task<bool> HashSetFieldNotExistsAsync(string hashKey, string field, byte[] data);

        Task<byte[][]> HashGetFieldsAsync(string hashKey, params string[] fields);

        Task<byte[]> HashGetFieldAsync(string hashKey, string field);

        Task<byte[][]> HashGetAllFieldsAsync(string hashKey);

        Task<string[]> HashGetKeysAsync(string hashKey);

        Task<byte[][]> HashGetValuesAsync(string hashKey);

        Task<int> HashGetLengthAsync(string hashKey);

        Task<int> HashDeleteFieldsAsync(string hashKey, params string[] fields);

        Task<bool> HashFieldExistsAsync(string hashKey, string field);
    }
}
