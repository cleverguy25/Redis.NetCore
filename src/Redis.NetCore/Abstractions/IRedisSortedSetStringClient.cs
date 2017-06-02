using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    [SuppressMessage("StyleCop", "SA1008", Justification = "StyleCop doesn't understand C#7 tuple return types yet.")]
    public interface IRedisSortedSetStringClient
    {
        Task<int> SortedSetAddMembersStringAsync(string setKey, params (string member, int score)[] items);

        Task<int> SortedSetAddOnlyMembersStringAsync(string setKey, params (string member, int score)[] items);

        Task<int> SortedSetUpdateMembersStringAsync(string setKey, params (string member, int score)[] items);

        Task<int> SortedSetUpsertMembersStringAsync(string setKey, params (string member, int score)[] items);

        Task<int?> SortedSetGetScoreStringAsync(string setKey, string member);
    }
}
