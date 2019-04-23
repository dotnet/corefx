// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
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
            GCHandle[] gch = null;

            try
            {
                gch = new GCHandle[100];

                for (int i = 0; i < gch.Length * 2; ++i)
                {
                    byte[] arr = new byte[128];
                    if (i % 2 == 0)
                    {
                        gch[i / 2] = GCHandle.Alloc(arr, GCHandleType.Pinned);
                    }
                }

                GC.Collect();

                GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();

                Assert.True(memoryInfo.FragmentedBytes >= 128 * (gch.Length - 1), $"FragmentedBytes = {memoryInfo.FragmentedBytes}");
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
        }

        [Fact]
        public static void GetGCMemoryInfoHeapSize()
        {
            GCMemoryInfo memoryInfo1 = GC.GetGCMemoryInfo();

            byte[][] arr = new byte[64 * 1024][];
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = new byte[i + 1];
            }

            GCMemoryInfo memoryInfo2 = GC.GetGCMemoryInfo();

            GC.KeepAlive(arr);

            Assert.True(memoryInfo2.HeapSizeBytes > memoryInfo1.HeapSizeBytes);
        }
    }
}
