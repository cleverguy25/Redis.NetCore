// <copyright file="IRedisStringClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisStringClient
    {
        Task SetStringAsync(string key, string data);

        Task SetStringAsync(string key, string data, TimeSpan expiration);

        Task SetStringAsync(string key, string data, int seconds);

        Task SetStringsAsync(IDictionary<string, string> dictionary);

        Task<bool> SetStringsNotExistsAsync(IDictionary<string, string> dictionary);

        Task<bool> SetStringNotExistsAsync(string key, string data);

        Task<bool> SetStringNotExistsAsync(string key, string data, int seconds);

        Task<bool> SetStringNotExistsAsync(string key, string data, TimeSpan expiration);

        Task<bool> SetStringExistsAsync(string key, string data);

        Task<bool> SetStringExistsAsync(string key, string data, int seconds);

        Task<bool> SetStringExistsAsync(string key, string data, TimeSpan expiration);

        Task<int> SetStringRangeAsync(string key, int offset, string data);

        Task<string> GetStringAsync(string key);

        Task<string[]> GetStringsAsync(params string[] keys);

        Task<string> GetStringRangeAsync(string key, int begin, int end = -1);

        Task<string> GetSetStringAsync(string key, string data);

        Task<int> AppendStringAsync(string key, string data);

        Task<int> GetStringLengthAsync(string key);
    }
}
