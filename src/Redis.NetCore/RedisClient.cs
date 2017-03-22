using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Redis.NetCore.Configuration;
using Redis.NetCore.Pipeline;
using Redis.NetCore.Sockets;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisClient
    {
        private readonly IRedisPipelinePool _redisPipelinePool;

        public RedisClient(IRedisPipelinePool redisPipelinePool)
        {
            _redisPipelinePool = redisPipelinePool;
        }

        public static IRedisClient CreateClient(IOptions<RedisConfiguration> redisConfiguration, int numConnections = 3)
        {
            var bufferManager = new BufferManager(15, 8192, 10, 20);
            var pipelinePool = new RedisPipelinePool(redisConfiguration.Value, bufferManager, numConnections);
            return new RedisClient(pipelinePool);
        }

        public async Task<int> GetTimeToLive(string key)
        {
            var bytes = await SendCommandAsync(RedisCommands.TimeToLive, key.ToBytes()).ConfigureAwait(false);
            var timeToLiveString = Encoding.UTF8.GetString(bytes);
            int timeToLive;
            if (int.TryParse(timeToLiveString, out timeToLive) == false)
            {
                throw new FormatException($"Cannot parse time to live [{timeToLiveString}]");
            }

            return timeToLive;
        }

        private static byte[][] ComposeRequest(byte[] commandName, IReadOnlyList<string> values)
        {
            var request = new byte[values.Count + 1][];
            request[0] = commandName;

            for (var i = 0; i < values.Count; i++)
            {
                request[i + 1] = values[i].ToBytes();
            }

            return request;
        }

        private async Task<byte[]> SendCommandAsync(params byte[][] requestData)
        {
            var pipeline = await _redisPipelinePool.GetPipelineAsync().ConfigureAwait(false);
            return await pipeline.SendCommandAsync(requestData).ConfigureAwait(false);
        }

        private async Task<byte[][]> SendMultipleCommandAsync(params byte[][] requestData)
        {
            var pipeline = await _redisPipelinePool.GetPipelineAsync().ConfigureAwait(false);
            return await pipeline.SendMultipleCommandAsync(requestData).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _redisPipelinePool?.Dispose();
        }
    }
}
