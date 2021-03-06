﻿// <copyright file="IRedisSetClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

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

        Task<bool> SetMoveMemberAsync(string sourceSet, string destinationSet, byte[] member);

        Task<byte[]> SetPopMemberAsync(string storeKey);

        Task<byte[][]> SetGetRandomMemberAsync(string storeKey, int count = 1);

        Task<int> SetRemoveMembersAsync(string storeKey, params byte[][] members);

        Task<ScanCursor> SetScanAsync(string setKey);

        Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor);

        Task<ScanCursor> SetScanAsync(string setKey, int count);

        Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor, int count);

        Task<ScanCursor> SetScanAsync(string setKey, string match);

        Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor, string match);

        Task<ScanCursor> SetScanAsync(string setKey, string match, int count);

        Task<ScanCursor> SetScanAsync(string setKey, ScanCursor cursor, string match, int count);
    }
}
