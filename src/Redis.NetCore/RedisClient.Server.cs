// <copyright file="RedisClient.Server.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisServerClient
    {
        public async Task<IDictionary<string, string>[]> GetClientListAsync()
        {
            var bytes = await SendMultipleCommandAsync(RedisCommands.Client, RedisCommands.List).ConfigureAwait(false);
            var clients = bytes.ConvertByteArrayToStringArray();

            return clients.Select(GetClientProperties).ToArray();
        }

        public async Task<string> GetClientNameAsync()
        {
            var bytes = await SendCommandAsync(RedisCommands.Client, RedisCommands.GetName).ConfigureAwait(false);
            return bytes.ConvertBytesToString();
        }

        public async Task SetClientNameAsync(string clientName)
        {
            var pipelines = await _redisPipelinePool.GetPipelinesAsync().ConfigureAwait(false);
            var tasks = pipelines.Select((pipeline, i) => pipeline.SendCommandAsync(RedisCommands.Client, RedisCommands.SetName, (clientName + i).ToBytes()));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task<DateTime> GetServerTimeAsync()
        {
            var bytes = await SendMultipleCommandAsync(RedisCommands.Time).ConfigureAwait(false);
            var seconds = bytes[0].ConvertBytesToInteger();
            var milliseconds = bytes[1].ConvertBytesToInteger() / 1000;
            var dateTime = new DateTime(1970, 1, 1).AddSeconds(seconds);
            return dateTime.AddMilliseconds(milliseconds);
        }

        public async Task<int> GetDatabaseSizeAsync()
        {
            var bytes = await SendCommandAsync(RedisCommands.DatabaseSize).ConfigureAwait(false);
            return bytes.ConvertBytesToInteger();
        }

        public Task FlushDatabaseAsync(bool async = false)
        {
            return async ? SendCommandAsync(RedisCommands.FlushDatabase, RedisCommands.Async) :
                SendCommandAsync(RedisCommands.FlushDatabase);
        }

        public Task FlushAllAsync(bool async = false)
        {
            return async ? SendCommandAsync(RedisCommands.FlushAll, RedisCommands.Async) :
                SendCommandAsync(RedisCommands.FlushAll);
        }

        public async Task<IDictionary<string, IDictionary<string, string>>> GetServerInformationAsync(string section = "default")
        {
            var bytes = await SendCommandAsync(RedisCommands.Information, section.ToBytes()).ConfigureAwait(false);
            var information = bytes.ConvertBytesToString();
            var results = new Dictionary<string, IDictionary<string, string>>();
            var currentDictionary = new Dictionary<string, string>();
            var reader = new StringReader(information);
            var line = reader.ReadLine();

            do
            {
                if (string.IsNullOrEmpty(line) == false)
                {
                    if (line.Contains("#"))
                    {
                        currentDictionary = new Dictionary<string, string>();
                        results[line] = currentDictionary;
                    }
                    else
                    {
                        var parts = line.Split(':');
                        currentDictionary.Add(parts[0], parts[1]);
                    }
                }

                line = reader.ReadLine();
            }
            while (line != null);

            return results;
        }

        public Task BackgroundSaveAsync()
        {
            return SendCommandAsync(RedisCommands.BackgroundSave);
        }

        public Task SaveAsync()
        {
            return SendCommandAsync(RedisCommands.Save);
        }

        public Task BackgroundRewriteAppendOnlyFileAsync()
        {
            return SendCommandAsync(RedisCommands.BackgroundRewriteAppendOnlyFile);
        }

        public async Task<DateTime> GetLastSaveDateTimeAsync()
        {
            var bytes = await SendCommandAsync(RedisCommands.LastSave).ConfigureAwait(false);
            var seconds = bytes.ConvertBytesToInteger();
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }

        public async Task<IDictionary<string, string>> GetConfigurationAsync(string pattern)
        {
            var bytes = await SendMultipleCommandAsync(RedisCommands.Configuration, RedisCommands.Get, pattern.ToBytes()).ConfigureAwait(false);
            return bytes.ConvertBytesToDictionary();
        }

        public Task SetConfigurationAsync(string key, string value)
        {
            return SendCommandAsync(RedisCommands.Configuration, RedisCommands.Set, key.ToBytes(), value.ToBytes());
        }

        public Task RewriteConfigurationAsync()
        {
            return SendCommandAsync(RedisCommands.Configuration, RedisCommands.Rewrite);
        }

        public Task ResetConfigurationStatsAsync()
        {
            return SendCommandAsync(RedisCommands.Configuration, RedisCommands.ResetStat);
        }

        private static IDictionary<string, string> GetClientProperties(string client)
        {
            var properties = new Dictionary<string, string>();
            var keyValues = client.Split(' ');
            foreach (var keyValue in keyValues)
            {
                var parts = keyValue.Split('=');
                properties[parts[0]] = parts[1];
            }

            return properties;
        }
    }
}
