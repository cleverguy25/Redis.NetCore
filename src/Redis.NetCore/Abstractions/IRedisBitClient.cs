using System;
using System.Collections.Generic;
using System.Linq;
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
