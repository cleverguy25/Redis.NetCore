using System;
using System.Collections.Generic;
using System.Linq;
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
