// <copyright file="IRedisHashClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisHashStringClient
    {
        Task<bool> HashSetFieldStringAsync(string hashKey, string field, string data);

        Task HashSetFieldsStringAsync(string hashKey, IDictionary<string, string> dictionary);

        Task<bool> HashSetFieldStringNotExistsAsync(string hashKey, string field, string data);

        Task<string[]> HashGetFieldsStringAsync(string hashKey, params string[] fields);

        Task<string> HashGetFieldStringAsync(string hashKey, string field);
    }
}
