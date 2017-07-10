// <copyright file="IRedisKeyClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisKeyClient
    {
        Task<string[]> GetKeysAsync(string pattern);

        Task DeleteKeyAsync(params string[] keys);

        Task<int> GetTimeToLiveAsync(string key);

        Task<long> GetPreciseTimeToLiveAsync(string key);

        Task<bool> MoveAsync(string key, int databaseIndex);

        Task<byte[]> DumpAsync(string key);

        Task RestoreAsync(string key, int timeToLive, byte[] data);

        Task<int> GetObjectReferenceCountAsync(string key);

        Task<int> GetObjectIdleTimeAsync(string key);

        Task<string> GetObjectEncodingAsync(string key);

        RedisSort Sort(string key);

        Task<bool> ExistsAsync(string key);

        Task<int> ExistsAsync(params string[] keys);

        Task<bool> SetExpirationAsync(string key, TimeSpan expiration);

        Task<bool> SetExpirationAsync(string key, int seconds);

        Task<bool> SetExpirationAsync(string key, DateTime dateTime);

        Task<bool> PersistAsync(string key);

        Task RenameKeyAsync(string key, string newKey);

        Task<bool> RenameKeyNotExistsAsync(string key, string newKey);

        Task<string> GetRandomKeyAsync();

        Task<ScanCursor> ScanAsync();

        Task<ScanCursor> ScanAsync(ScanCursor cursor);

        Task<ScanCursor> ScanAsync(int count);

        Task<ScanCursor> ScanAsync(ScanCursor cursor, int count);

        Task<ScanCursor> ScanAsync(string match);

        Task<ScanCursor> ScanAsync(ScanCursor cursor, string match);

        Task<ScanCursor> ScanAsync(string match, int count);

        Task<ScanCursor> ScanAsync(ScanCursor cursor, string match, int count);

        Task TouchAsync(string key);

        Task<string> GetTypeAsync(string key);
    }
}
