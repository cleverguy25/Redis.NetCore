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