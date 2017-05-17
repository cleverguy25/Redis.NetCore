using System;
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

        Task<byte[]> ListIndexAsync(string listKey, int index);

        Task<int> ListInsertBeforeAsync(string listKey, byte[] pivot, byte[] value);

        Task<int> ListInsertAfterAsync(string listKey, byte[] pivot, byte[] value);

        Task<int> ListLength(string listKey);

        Task<byte[][]> ListRangeAsync(string listKey, int start, int end);

        Task<int> ListRemoveAsync(string listKey, int count, byte[] value);

        Task ListSetAsync(string listKey, int index, byte[] value);

        Task ListTrimAsync(string listKey, int start, int end);
    }
}
