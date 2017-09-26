// <copyright file="IRedisPipeline.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IRedisPipeline : IDisposable
    {
        bool IsErrorState { get; }

        int DurationFromLastCommand { get; }

        ConcurrentQueue<RedisPipelineItem> RequestQueue { get; }

        void SaveQueue(IRedisPipeline redisPipeline);

        Task<byte[]> SendCommandAsync(params byte[][] requestData);

        Task<byte[][]> SendMultipleCommandAsync(params byte[][] requestData);

        void ThrowErrorForRemainingResponseQueueItems();

        Task AuthenticateAsync(string password);

        Task KeepAliveAsync();
    }
}