using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisSetClient
    {
        Task<int> SetAddMemberAsync(string setKey, params byte[][] members);

        Task<int> SetCardinalityAsync(string setKey);

        Task<bool> SetIsMemberAsync(string setKey, byte[] member);

        Task<byte[][]> SetGetDifferenceMembersAsync(params string[] setKeys);

        Task<byte[][]> SetGetIntersectionMembersAsync(params string[] setKeys);

        Task<byte[][]> SetGetUnionMembersAsync(params string[] setKeys);

        Task<int> SetStoreDifferenceMembersAsync(string storeKey, params string[] setKeys);

        Task<int> SetStoreIntersectionMembersAsync(string storeKey, params string[] setKeys);

        Task<int> SetStoreUnionMembersAsync(string storeKey, params string[] setKeys);

        Task<byte[][]> SetGetMembersAsync(string storeKey);
    }
}
