using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisConnectionClient
    {
        Task<string> PingAsync();

        Task<string> EchoAsync(string message);

        Task QuitAsync();

        Task SelectDatabaseAsync(int index);
    }
}
