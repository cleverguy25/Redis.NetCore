// <copyright file="IRedisClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisClient : IRedisConnectionClient, IRedisRawClient, IRedisKeyClient, IRedisStringClient, IRedisBitClient, IRedisMathClient, IRedisHashClient, IRedisHashStringClient, IRedisListClient, IRedisListStringClient, IDisposable
    {
    }
}