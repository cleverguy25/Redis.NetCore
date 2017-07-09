﻿using System.Threading.Tasks;

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

        Task<bool> SetMoveMemberStringAsync(string sourceSet, string destinationSet, string member);

        Task<string> SetPopMemberStringAsync(string storeKey);

        Task<string[]> SetGetRandomMemberStringAsync(string storeKey, int count = 1);

        Task<int> SetRemoveMembersStringAsync(string storeKey, params string[] members);
    }
}