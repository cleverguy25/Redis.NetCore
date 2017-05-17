using System.Linq;
using System.Threading.Tasks;
using Redis.NetCore.Abstractions;

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

        public async Task<string[]> SetDifferenceStringAsync(params string[] setKeys)
        {
            var bytes = await SetDifferenceAsync(setKeys).ConfigureAwait(false);
            return bytes.Select(ConvertBytesToString).ToArray();
        }

        public async Task<string[]> SetIntersectionStringAsync(params string[] setKeys)
        {
            var bytes = await SetIntersectionAsync(setKeys).ConfigureAwait(false);
            return bytes.Select(ConvertBytesToString).ToArray();
        }

        public async Task<string[]> SetUnionStringAsync(params string[] setKeys)
        {
            var bytes = await SetUnionAsync(setKeys).ConfigureAwait(false);
            return bytes.Select(ConvertBytesToString).ToArray();
        }
    }
}
