using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisSetStringClient
    {
        Task<int> SetAddMemberStringAsync(string setKey, params string[] members);

        Task<bool> SetIsMemberStringAsync(string setKey, string member);

        Task<string[]> SetGetDifferenceMembersStringAsync(params string[] setKeys);

        Task<string[]> SetGetIntersectionMembersStringAsync(params string[] setKeys);

        Task<string[]> SetGetUnionMembersStringAsync(params string[] setKeys);

        Task<string[]> SetGetMembersStringAsync(string storeKey);
    }
}
