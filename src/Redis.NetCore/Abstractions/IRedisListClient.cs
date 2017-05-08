using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisListClient
    {
        Task<int> ListPushAsync(string listKey, params byte[][] values);

        Task<int> ListPushIfExistsAsync(string listKey, byte[] value);

        Task<int> ListTailPushAsync(string listKey, params byte[][] values);

        Task<int> ListTailPushIfExistsAsync(string listKey, byte[] value);

        Task<byte[]> ListPopAsync(string listKey);

        Task<Tuple<string, byte[]>> ListBlockingPopAsync(int timeoutSeconds, params string[] listKeys);

        Task<byte[]> ListTailPopAsync(string listKey);

        Task<Tuple<string, byte[]>> ListBlockingTailPopAsync(int timeoutSeconds, params string[] listKeys);

        Task<byte[]> ListTailPopAndPushAsync(string listKey1, string listKey2);

        Task<byte[]> ListBlockingTailPopAndPushAsync(string listKey1, string listKey2, int timeoutSeconds);
    }
}
