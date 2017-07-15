// <copyright file="IRedisServerClient.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisServerClient
    {
        Task SetConfigurationAsync(string key, string value);

        Task<IDictionary<string, string>> GetConfigurationAsync(string pattern);

        Task ResetConfigurationStatsAsync();

        Task RewriteConfigurationAsync();

        Task BackgroundSaveAsync();

        Task SaveAsync();

        Task BackgroundRewriteAppendOnlyFileAsync();

        Task<DateTime> GetLastSaveDateTimeAsync();

        Task<IDictionary<string, string>[]> GetClientListAsync();

        Task<string> GetClientNameAsync();

        Task SetClientNameAsync(string clientName);

        Task<DateTime> GetServerTimeAsync();

        Task<int> GetDatabaseSizeAsync();

        Task FlushDatabaseAsync(bool async = false);

        Task FlushAllAsync(bool async = false);

        Task<IDictionary<string, IDictionary<string, string>>> GetServerInformationAsync(string section = "default");
    }
}
