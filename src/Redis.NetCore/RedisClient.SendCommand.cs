using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore
{
    public partial class RedisClient
    {
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

        private static byte[][] ComposeRequest(byte[] commandName, IReadOnlyList<byte[]> values)
        {
            var request = new byte[values.Count + 1][];
            request[0] = commandName;

            for (var i = 0; i < values.Count; i++)
            {
                request[i + 1] = values[i];
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

    }
}
