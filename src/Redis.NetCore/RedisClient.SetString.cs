// <copyright file="RedisClient.SetString.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisSetStringClient
    {
        public Task<int> SetAddMemberStringAsync(string setKey, params string[] members)
        {
            return SetAddMemberAsync(setKey, members.Select(item => item.ToBytes()).ToArray());
        }

        public Task<bool> SetIsMemberStringAsync(string setKey, string member)
        {
            return SetIsMemberAsync(setKey, member.ToBytes());
        }

        public async Task<string[]> SetGetDifferenceMembersStringAsync(params string[] setKeys)
        {
            var bytes = await SetGetDifferenceMembersAsync(setKeys).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SetGetIntersectionMembersStringAsync(params string[] setKeys)
        {
            var bytes = await SetGetIntersectionMembersAsync(setKeys).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SetGetUnionMembersStringAsync(params string[] setKeys)
        {
            var bytes = await SetGetUnionMembersAsync(setKeys).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public async Task<string[]> SetGetMembersStringAsync(string storeKey)
        {
            var bytes = await SetGetMembersAsync(storeKey).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public Task<bool> SetMoveMemberStringAsync(string sourceSet, string destinationSet, string member)
        {
            return SetMoveMemberAsync(sourceSet, destinationSet, member.ToBytes());
        }

        public async Task<string> SetPopMemberStringAsync(string storeKey)
        {
            var bytes = await SetPopMemberAsync(storeKey);
            return bytes.ConvertBytesToString();
        }

        public async Task<string[]> SetGetRandomMemberStringAsync(string storeKey, int count = 1)
        {
            var bytes = await SetGetRandomMemberAsync(storeKey, count);
            return bytes.ConvertByteArrayToStringArray();
        }

        public Task<int> SetRemoveMembersStringAsync(string storeKey, params string[] members)
        {
            return SetRemoveMembersAsync(storeKey, members.Select(item => item.ToBytes()).ToArray());
        }
    }
}
