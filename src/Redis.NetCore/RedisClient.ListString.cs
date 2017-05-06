using Redis.NetCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisStringClient
    {
        public Task<int> ListPushStringAsync(string listKey, params string[] values)
        {
            return ListPushAsync(listKey, values.Select(value => value.ToBytes()).ToArray());
        }

        public Task<int> ListTailPushStringAsync(string listKey, params string[] values)
        {
            return ListTailPushAsync(listKey, values.Select(value => value.ToBytes()).ToArray());
        }

        public async Task<string> ListPopStringAsync(string listKey)
        {
            var bytes = await ListPopAsync(listKey);
            return ConvertBytesToString(bytes);
        }

        public async Task<string> ListTailPopStringAsync(string listKey)
        {
            var bytes = await ListTailPopAsync(listKey);
            return ConvertBytesToString(bytes);
        }
    }
}
