using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisSetClient
    {
        Task<int> SetAddAsync(string setKey, params byte[][] members);

        Task<int> SetCardinalityAsync(string setKey);

        Task<bool> SetIsMemberAsync(string setKey, byte[] member);

        Task<byte[][]> SetDifferenceAsync(params string[] setKeys);

        Task<byte[][]> SetIntersectionAsync(params string[] setKeys);

        Task<byte[][]> SetUnionAsync(params string[] setKeys);
    }
}
