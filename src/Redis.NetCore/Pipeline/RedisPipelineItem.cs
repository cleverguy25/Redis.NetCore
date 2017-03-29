// <copyright file="RedisPipelineItem.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;

namespace Redis.NetCore.Pipeline
{
    public class RedisPipelineItem
    {
        private static long _pipelineId = 0;
        private readonly Action<Exception> _onError;
        private readonly Action<byte[][]> _onSuccess;

        public RedisPipelineItem(byte[][] data, Action<Exception> onError, Action<byte[][]> onSuccess)
        {
            Data = data;
            _onError = onError;
            _onSuccess = onSuccess;
            Id = Interlocked.Increment(ref _pipelineId);
        }

        public long Id { get; } = 0;

        public byte[][] Data { get; private set; }

        public void OnError(Exception exception)
        {
            _onError?.Invoke(exception);
        }

        public void OnSuccess(byte[][] redisResponse)
        {
            _onSuccess?.Invoke(redisResponse);
        }
    }
}