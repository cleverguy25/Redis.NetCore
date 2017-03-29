// <copyright file="IRedisWriter.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IRedisWriter
    {
        int BytesInBuffer { get; }

        int BufferCount { get; }

        Task WriteRedisRequestAsync(byte[][] requestData);

        List<ArraySegment<byte>> FlushBuffers();

        void CheckInBuffers();

        Task CreateNewBufferAsync();

        Task FlushWriteBufferAsync();
    }
}