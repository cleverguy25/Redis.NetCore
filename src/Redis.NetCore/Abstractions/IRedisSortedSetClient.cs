using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public interface IRedisSortedSetClient
    {
        Task<int> SortedSetAddMembersAsync(string setKey, params (byte[] member, int score)[] items);

        Task<int> SortedSetAddOnlyMembersAsync(string setKey, params (byte[] member, int score)[] items);

        Task<int> SortedSetUpdateMembersAsync(string setKey, params (byte[] member, int score)[] items);

        Task<int> SortedSetUpsertMembersAsync(string setKey, params (byte[] member, int score)[] items);

        Task<int> SortedSetCardinalityAsync(string setKey);

        Task<int?> SortedSetGetScoreAsync(string setKey, byte[] member);

        Task<int> SortedSetGetCountAsync(string setKey, string min, string max);

        Task<int> SortedSetIncrementByAsync(string setKey, byte[] member, int increment);

        Task<int> SortedSetStoreIntersectionMembersAsync(string storeKey, string[] sets, RedisAggregate aggregate = RedisAggregate.Sum);

        Task<int> SortedSetStoreIntersectionMembersAsync(string storeKey, (string set, int weight)[] sets, RedisAggregate aggregate = RedisAggregate.Sum);

        Task<int> SortedSetStoreUnionMembersAsync(string storeKey, string[] sets, RedisAggregate aggregate = RedisAggregate.Sum);

        Task<int> SortedSetStoreUnionMembersAsync(string storeKey, (string set, int weight)[] sets, RedisAggregate aggregate = RedisAggregate.Sum);
    }
}
