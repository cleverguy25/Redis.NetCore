using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore;
using Redis.NetCore.Abstractions;

namespace Redis.NetCore
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public partial class RedisClient : IRedisSortedSetStringClient
    {
        public Task<int> SortedSetAddMembersStringAsync(string setKey, params(string member, int score)[] items)
        {
            return SortedSetAddMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int> SortedSetAddOnlyMembersStringAsync(string setKey, params (string member, int score)[] items)
        {
            return SortedSetAddOnlyMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int> SortedSetUpsertMembersStringAsync(string setKey, params (string member, int score)[] items)
        {
            return SortedSetUpsertMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int> SortedSetUpdateMembersStringAsync(string setKey, params (string member, int score)[] items)
        {
            return SortedSetUpdateMembersAsync(setKey, items.Select(item => (item.member.ToBytes(), item.score)).ToArray());
        }

        public Task<int?> SortedSetGetScoreStringAsync(string setKey, string member)
        {
            return SortedSetGetScoreAsync(setKey, member.ToBytes());
        }
    }
}
