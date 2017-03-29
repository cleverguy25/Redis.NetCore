// <copyright file="IBufferManager.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace Redis.NetCore.Pipeline
{
    public interface IBufferManager
    {
        int AvailableBuffers { get; }

        int TotalBufferSize { get; }

        int ChunkSize { get; }

        Task<ArraySegment<byte>> CheckOutAsync(int timeout = 10000);

        void CheckIn(ArraySegment<byte> buffer);
    }
}