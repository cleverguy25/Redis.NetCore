using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
        public byte[] Get(string key)
        {
            return GetAsync(key).GetAwaiter().GetResult();
        }
        
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            return options.AbsoluteExpirationRelativeToNow.HasValue
                ? SetAsync(key, value, options.AbsoluteExpirationRelativeToNow.Value) : SetAsync(key, value);
        }

        public void Refresh(string key)
        {
            // TODO: clevel implement IDistributedCache Refresh
        }

        public Task RefreshAsync(string key)
        {
            return Task.CompletedTask; // TODO: clevel implement IDistributedCache RefreshAsync
        }

        public void Remove(string key)
        {
            RemoveAsync(key).GetAwaiter().GetResult();
        }

        public Task RemoveAsync(string key)
        {
            return DeleteKeyAsync(key);
        }
    }
}
