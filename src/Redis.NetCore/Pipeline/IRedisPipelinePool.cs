using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IRedisPipelinePool : IDisposable
    {
        Task<IRedisPipeline> GetPipelineAsync();
    }
}