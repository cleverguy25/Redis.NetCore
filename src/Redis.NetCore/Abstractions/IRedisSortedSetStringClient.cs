﻿// <copyright file="IRedisSortedSetStringClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

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
        Task<int> SortedSetAddMembersStringAsync(string setKey, params (string member, double score)[] items);

        Task<int> SortedSetAddOnlyMembersStringAsync(string setKey, params (string member, double score)[] items);

        Task<int> SortedSetUpdateMembersStringAsync(string setKey, params (string member, double score)[] items);

        Task<int> SortedSetUpsertMembersStringAsync(string setKey, params (string member, double score)[] items);

        Task<int?> SortedSetGetScoreStringAsync(string setKey, string member);

        Task<int?> SortedSetGetRankStringAsync(string setKey, string member);

        Task<int?> SortedSetGetReverseRankStringAsync(string setKey, string member);

        Task<int> SortedSetIncrementByStringAsync(string setKey, string member, int increment);

        Task<string[]> SortedSetGetRangeStringAsync(string setKey, int start, int end);

        Task<(string member, double weight)[]> SortedSetGetRangeWithScoresStringAsync(string setKey, int start, int end);

        Task<string[]> SortedSetGetReverseRangeStringAsync(string setKey, int start, int end);

        Task<(string member, double weight)[]> SortedSetGetReverseRangeWithScoresStringAsync(string setKey, int start, int end);

        Task<int> SortedSetGetLexCountAsync(string setKey, string min, string max);

        Task<string[]> SortedSetGetLexRangeAsync(string setKey, string min, string max);

        Task<string[]> SortedSetGetLexRangeAsync(string setKey, string min, string max, int offset, int count);

        Task<string[]> SortedSetGetReverseLexRangeAsync(string setKey, string min, string max);

        Task<string[]> SortedSetGetReverseLexRangeAsync(string setKey, string min, string max, int offset, int count);

        Task<string[]> SortedSetGetRangeByScoreStringAsync(string setKey, string min, string max);

        Task<string[]> SortedSetGetRangeByScoreStringAsync(string setKey, string min, string max, int offset, int count);

        Task<(string member, double weight)[]> SortedSetGetRangeByScoreWithScoresStringAsync(string setKey, string min, string max);

        Task<(string member, double weight)[]> SortedSetGetRangeByScoreWithScoresStringAsync(
            string setKey,
            string min,
            string max,
            int offset,
            int count);

        Task<string[]> SortedSetGetReverseRangeByScoreStringAsync(string setKey, string min, string max);

        Task<string[]> SortedSetGetReverseRangeByScoreStringAsync(string setKey, string min, string max, int offset, int count);

        Task<(string member, double weight)[]> SortedSetGetReverseRangeByScoreWithScoresStringAsync(string setKey, string min, string max);

        Task<(string member, double weight)[]> SortedSetGetReverseRangeByScoreWithScoresStringAsync(
            string setKey,
            string min,
            string max,
            int offset,
            int count);

        Task<int> SortedSetRemoveMembersStringAsync(string setKey, params string[] members);

        Task<int> SortedSetRemoveRangeByLexAsync(string setKey, string min, string max);
    }
}
