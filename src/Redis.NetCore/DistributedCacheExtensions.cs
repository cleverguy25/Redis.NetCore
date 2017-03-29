// <copyright file="DistributedCacheExtensions.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Redis.NetCore.Abstractions;

namespace Redis.NetCore
{
    public static class DistributedCacheExtensions
    {
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection serviceCollection, IRedisClient client)
        {
            serviceCollection.AddSingleton((IDistributedCache)client);
            return serviceCollection;
        }
    }
}
