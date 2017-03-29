// <copyright file="IRedisBitClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisBitClient
    {
        Task<int> GetBitCountAsync(string key, int begin = 0, int end = -1);

        Task<int> GetBitPositionAsync(string key, bool bit, int begin = 0, int end = -1);

        Task<bool> SetBitAsync(string key, int index, bool bit);

        Task<bool> GetBitAsync(string key, int index);

        Task<int> PerformBitwiseAndAsync(string destinationKey, params string[] sourceKeys);

        Task<int> PerformBitwiseOrAsync(string destinationKey, params string[] sourceKeys);

        Task<int> PerformBitwiseXorAsync(string destinationKey, params string[] sourceKeys);

        Task<int> PerformBitwiseNotAsync(string destinationKey, string sourceKey);
    }
}
