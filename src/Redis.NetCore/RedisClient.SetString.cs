using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;
using Redis.NetCore.Constants;

namespace Redis.NetCore
{
    public partial class RedisClient : IRedisSetStringClient
    {
        public Task<int> SetAddStringAsync(string setKey, params string[] members)
        {
            return SetAddAsync(setKey, members.Select(item => item.ToBytes()).ToArray());
        }

        public Task<bool> SetIsMemberStringAsync(string setKey, string member)
        {
            return SetIsMemberAsync(setKey, member.ToBytes());
        }
    }
}
