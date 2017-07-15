// <copyright file="IRedisSetStringClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

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

        Task<bool> SetMoveMemberStringAsync(string sourceSet, string destinationSet, string member);

        Task<string> SetPopMemberStringAsync(string storeKey);

        Task<string[]> SetGetRandomMemberStringAsync(string storeKey, int count = 1);

        Task<int> SetRemoveMembersStringAsync(string storeKey, params string[] members);
    }
}
