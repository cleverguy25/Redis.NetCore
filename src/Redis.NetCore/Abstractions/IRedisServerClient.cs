using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redis.NetCore.Abstractions
{
    public interface IRedisServerClient
    {
        Task<IDictionary<string, string>[]> GetClientListAsync();

        Task<string> GetClientNameAsync();

        Task SetClientNameAsync(string clientName);

        Task<DateTime> GetServerTimeAsync();
    }
}
