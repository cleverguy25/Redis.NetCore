using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisSetStringClient
    {
        Task<int> SetAddStringAsync(string setKey, params string[] members);

        Task<bool> SetIsMemberStringAsync(string setKey, string member);

        Task<string[]> SetDifferenceStringAsync(params string[] setKeys);

        Task<string[]> SetIntersectionStringAsync(params string[] setKeys);

        Task<string[]> SetUnionStringAsync(params string[] setKeys);
    }
}
