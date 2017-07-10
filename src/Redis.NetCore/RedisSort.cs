using System.Collections.Generic;
using System.Threading.Tasks;
using Redis.NetCore;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public class RedisSort
    {
        private readonly RedisClient _client;
        private readonly List<byte[]> _request = new List<byte[]>();

        public RedisSort(RedisClient client, IEnumerable<byte[]> request)
        {
            _client = client;
            _request.AddRange(request);
        }

        public RedisSort Alpha()
        {
            _request.Add(RedisCommands.Alpha);
            return this;
        }

        public RedisSort Descending()
        {
            _request.Add(RedisCommands.Descending);
            return this;
        }

        public RedisSort Limit(int offset, int count)
        {
            _request.Add(RedisCommands.Limit);
            _request.Add(offset.ToBytes());
            _request.Add(count.ToBytes());
            return this;
        }

        public RedisSort By(string pattern)
        {
            _request.Add(RedisCommands.By);
            _request.Add(pattern.ToBytes());
            return this;
        }

        public RedisSort Get(string pattern)
        {
            _request.Add(RedisCommands.Get);
            _request.Add(pattern.ToBytes());
            return this;
        }

        public Task<byte[][]> ExecuteAsync()
        {
            return _client.SendMultipleCommandAsync(_request.ToArray());
        }

        public async Task<string[]> ExecuteStringAsync()
        {
            var bytes = await _client.SendMultipleCommandAsync(_request.ToArray()).ConfigureAwait(false);
            return bytes.ConvertByteArrayToStringArray();
        }

        public Task StoreAsync(string key)
        {
            RedisClient.CheckKey(key);

            _request.Add(RedisCommands.Store);
            _request.Add(key.ToBytes());
            return _client.SendMultipleCommandAsync(_request.ToArray());
        }
    }
}