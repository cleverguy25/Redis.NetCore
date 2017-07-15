// <copyright file="IRedisHyperLogLogClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisHyperLogLogClient
    {
        Task<bool> HyperLogLogAddAsync(string hyperKey, params byte[][] elements);

        Task<bool> HyperLogLogAddStringAsync(string hyperKey, params string[] elements);

        Task<int> HyperLogLogCountAsync(params string[] keys);

        Task HyperLogLogMergeAsync(string storeKey, params string[] keys);
    }
}