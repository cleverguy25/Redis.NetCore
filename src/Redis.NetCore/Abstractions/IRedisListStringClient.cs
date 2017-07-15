// <copyright file="IRedisListStringClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisListStringClient
    {
        Task<int> ListPushStringAsync(string listKey, params string[] values);

        Task<int> ListPushStringIfExistsAsync(string listKey, string value);

        Task<int> ListTailPushStringAsync(string listKey, params string[] values);

        Task<int> ListTailPushStringIfExistsAsync(string listKey, string value);

        Task<string> ListPopStringAsync(string listKey);

        Task<Tuple<string, string>> ListBlockingPopStringAsync(int timeoutSeconds, params string[] listKeys);

        Task<string> ListTailPopStringAsync(string listKey);

        Task<Tuple<string, string>> ListBlockingTailPopStringAsync(int timeoutSeconds, params string[] listKeys);

        Task<string> ListTailPopAndPushStringAsync(string listKey1, string listKey2);

        Task<string> ListBlockingTailPopAndPushStringAsync(string listKey1, string listKey2, int timeoutSeconds);

        Task<string> ListIndexStringAsync(string listKey, int index);

        Task<int> ListInsertBeforeStringAsync(string listKey, string pivot, string value);

        Task<int> ListInsertAfterStringAsync(string listKey, string pivot, string value);

        Task<string[]> ListRangeStringAsync(string listKey, int start, int end);

        Task<int> ListRemoveStringAsync(string listKey, int count, string value);
    }
}
