using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

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
