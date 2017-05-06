using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisListClient
    {
        Task<int> ListPushAsync(string listKey, params byte[][] values);

        Task<int> ListTailPushAsync(string listKey, params byte[][] values);

        Task<byte[]> ListPopAsync(string listKey);

        Task<byte[]> ListTailPopAsync(string listKey);
    }
}
