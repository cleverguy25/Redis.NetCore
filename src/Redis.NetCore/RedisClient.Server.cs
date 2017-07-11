using System;
using System.Collections.Generic;
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
            var bytes = await SendMultipleCommandAsync(RedisCommands.Time);
            var seconds = bytes[0].ConvertBytesToInteger();
            var milliseconds = bytes[1].ConvertBytesToInteger() / 1000;
            var dateTime = new DateTime(1970, 1, 1).AddSeconds(seconds);
            return dateTime.AddMilliseconds(milliseconds);
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
