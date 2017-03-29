// <copyright file="IRedisMathClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisMathClient
    {
        Task<int> IncrementAsync(string key);

        Task<int> IncrementAsync(string key, int amount);

        Task<float> IncrementAsync(string key, float amount);

        Task<int> DecrementAsync(string key);

        Task<int> DecrementAsync(string key, int amount);
    }
}
