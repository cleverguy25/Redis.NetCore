using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Constants;
using Redis.NetCore.Pipeline;
using Redis.NetCore.Sockets;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "UnitTest")]
    public class RedisWriterTests
    {
        [Fact]
        public async Task WriteCommandAsync()
        {
            var redisWriter = await CreateRedisWriter(100);
            var request = CreateRequest(RedisCommands.Get, "Key".ToBytes());
            await redisWriter.WriteRedisRequestAsync(request);
            var result = GetWriteResult(redisWriter);
            Assert.Equal("*2\r\n$3\r\nGET\r\n$3\r\nKey\r\n", result);
        }

        [Fact]
        public async Task WriteCommandSmallBufferAsync()
        {
            var redisWriter = await CreateRedisWriter(5);
            var request = CreateRequest(RedisCommands.Get, "Key!".ToBytes());
            await redisWriter.WriteRedisRequestAsync(request);
            var result = GetWriteResult(redisWriter);
            Assert.Equal("*2\r\n$3\r\nGET\r\n$4\r\nKey!\r\n", result);
        }

        [Fact]
        public async Task WriteCommandSmallBufferBigDataAsync()
        {
            var redisWriter = await CreateRedisWriter(5);
            var request = CreateRequest(RedisCommands.Get, "THIS IS A REALLY LONG KEY.".ToBytes());
            await redisWriter.WriteRedisRequestAsync(request);
            var result = GetWriteResult(redisWriter);
            Assert.Equal("*2\r\n$3\r\nGET\r\n$26\r\nTHIS IS A REALLY LONG KEY.\r\n", result);
        }

        private static async Task<RedisWriter> CreateRedisWriter(int chunkSize)
        {
            var bufferManager = new BufferManager(2, chunkSize, 2, 10);
            var redisWriter = new RedisWriter(bufferManager);
            await redisWriter.CreateNewBufferAsync();
            return redisWriter;
        }

        private static string GetWriteResult(IRedisWriter redisWriter)
        {
            var buffers = redisWriter.FlushBuffers();
            var bytes = buffers.SelectMany(buffer => buffer.ToArray()).ToArray();
            var result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        private static byte[][] CreateRequest(params byte[][] data)
        {
            return data;
        }
    }
}