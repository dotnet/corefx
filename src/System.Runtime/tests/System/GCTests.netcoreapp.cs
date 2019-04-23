// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.Tests
{
    public static partial class GCTests
    {
        [Theory]
        [InlineData(1000)]
        [InlineData(100000)]
        public static void GetAllocatedBytesForCurrentThread(int size)
        {
            long start = GC.GetAllocatedBytesForCurrentThread();

            GC.KeepAlive(new string('a', size));

            long end = GC.GetAllocatedBytesForCurrentThread();

            Assert.True((end - start) > size, $"Allocated too little: start: {start} end: {end} size: {size}");
            Assert.True((end - start) < 5 * size, $"Allocated too much: start: {start} end: {end} size: {size}");
        }

        [Fact]
        public static void GetGCMemoryInfo()
        {
            GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();

            Assert.True(memoryInfo.HighMemoryLoadThresholdBytes > 0);
            Assert.True(memoryInfo.MemoryLoadBytes > 0);
            Assert.True(memoryInfo.TotalAvailableMemoryBytes > 0);
            Assert.True(memoryInfo.HeapSizeBytes > 0);
            Assert.True(memoryInfo.FragmentedBytes >= 0);
        }

        [Fact]
        public static void GetGCMemoryInfoFragmentation()
        {
            RemoteExecutor.Invoke(() =>
            {
                GCHandle[] gch = new GCHandle[100];

                try
                {
                    // Allows to update the value returned by GC.GetGCMemoryInfo
                    GC.Collect();

                    GCMemoryInfo memoryInfo1 = GC.GetGCMemoryInfo();

                    for (int i = 0; i < gch.Length * 2; ++i)
                    {
                        byte[] arr = new byte[128];
                        if (i % 2 == 0)
                        {
                            gch[i / 2] = GCHandle.Alloc(arr, GCHandleType.Pinned);
                        }
                    }

                    // Allows to update the value returned by GC.GetGCMemoryInfo
                    GC.Collect();

                    GCMemoryInfo memoryInfo2 = GC.GetGCMemoryInfo();

                    Assert.True(memoryInfo1.FragmentedBytes < memoryInfo2.FragmentedBytes);
                    Assert.InRange(memoryInfo2.FragmentedBytes, 128 * (gch.Length - 1), long.MaxValue);
                }
                finally
                {
                    for (int i = 0; i < gch.Length; ++i)
                    {
                        if (gch[i].IsAllocated)
                        {
                            gch[i].Free();
                        }
                    }
                }
            }).Dispose();
        }

        [Fact]
        public static void GetGCMemoryInfoHeapSize()
        {
            RemoteExecutor.Invoke(() =>
            {
                // Allows to update the value returned by GC.GetGCMemoryInfo
                GC.Collect();

                GCMemoryInfo memoryInfo1 = GC.GetGCMemoryInfo();

                byte[][] arr = new byte[64 * 1024][];
                for (int i = 0; i < arr.Length; ++i)
                {
                    arr[i] = new byte[i + 1];
                }

                // Allows to update the value returned by GC.GetGCMemoryInfo
                GC.Collect();

                GCMemoryInfo memoryInfo2 = GC.GetGCMemoryInfo();

                GC.KeepAlive(arr);

                Assert.True(memoryInfo2.HeapSizeBytes > memoryInfo1.HeapSizeBytes);
            }).Dispose();
        }
    }
}
