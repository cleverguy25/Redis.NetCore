using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisListStringClient
    {
        Task<int> ListPushStringAsync(string listKey, params string[] values);

        Task<int> ListPushStringIfExistsAsync(string listKey, string value);

        Task<int> ListTailPushStringAsync(string listKey, params string[] values);

        Task<int> ListTailPushStringIfExistsAsync(string listKey, string value);

        Task<string> ListPopStringAsync(string listKey);

        Task<string> ListTailPopStringAsync(string listKey);
    }
}
