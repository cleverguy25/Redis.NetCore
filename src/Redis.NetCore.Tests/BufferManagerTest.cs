// <copyright file="BufferManagerTest.cs" company="PayScale">
// Copyright (c) PayScale. All rights reserved.
// Licensed under the APACHE 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Redis.NetCore.Pipeline;
using Xunit;

namespace Redis.NetCore.Tests
{
    [Trait("Category", "UnitTest")]
    public class BufferManagerTest
    {
        private const int SegmentSize = 8192;

        [Fact]
        public async Task CheckOutAsync()
        {
            var bufferManager = CreateTestBufferManager();
            var buffer = await bufferManager.CheckOutAsync();
            Assert.Equal(8192, buffer.Count);
            Assert.Equal(1, bufferManager.AvailableBuffers);
        }

        [Fact]
        public async Task CheckOutCheckInAsync()
        {
            var bufferManager = CreateTestBufferManager();
            var buffer = await bufferManager.CheckOutAsync();
            bufferManager.CheckIn(buffer);
            Assert.Equal(2, bufferManager.AvailableBuffers);
        }

        [Fact]
        public async Task CheckOutCreateNewSegmentAsync()
        {
            var bufferManager = CreateTestBufferManager();

            await RepeatCheckout(3, bufferManager);

            Assert.Equal(SegmentSize * 2 * 2, bufferManager.TotalBufferSize);
        }

        [Fact]
        public async Task CheckOutMaxSizeThrowsTimeoutAsync()
        {
            var bufferManager = CreateTestBufferManager();
            await RepeatCheckout(4, bufferManager);
            await Assert.ThrowsAsync<TimeoutException>(() => bufferManager.CheckOutAsync(300));
        }

        [Fact]
        public async Task CheckOutMaxSizeWaitAsync()
        {
            var bufferManager = CreateTestBufferManager();
            var buffer = await bufferManager.CheckOutAsync();
            await RepeatCheckout(3, bufferManager);
            var checkoutTask = bufferManager.CheckOutAsync();
            var checkinTask = CheckInAfterWait(buffer, bufferManager);
            await Task.WhenAll(checkoutTask, checkinTask);
            Assert.Equal(0, bufferManager.AvailableBuffers);
        }

        [Fact]
        public void CreateNewBufferManager()
        {
            const int segments = 15;

            const int initialCount = 10;
            const int expectedBufferCount = initialCount * segments;
            var bufferManager = new BufferManager(segments, SegmentSize, initialCount, 100);
            Assert.Equal(expectedBufferCount * SegmentSize, bufferManager.TotalBufferSize);
            Assert.Equal(expectedBufferCount, bufferManager.AvailableBuffers);
        }

        private static BufferManager CreateTestBufferManager()
        {
            return new BufferManager(2, SegmentSize, 1, 2);
        }

        private static async Task RepeatCheckout(int count, IBufferManager bufferManager)
        {
            for (var i = 0; i < count; i++)
            {
                await bufferManager.CheckOutAsync();
            }
        }

        private static async Task CheckInAfterWait(ArraySegment<byte> buffer, IBufferManager bufferManager)
        {
            await Task.Delay(3000);
            bufferManager.CheckIn(buffer);
        }
    }
}