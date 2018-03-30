// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using Xunit;

namespace System.MemoryTests
{
    public static partial class MemoryTests
    {
        [Fact]
        public static void MemoryPin()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            Memory<int> memory = array;
            MemoryHandle handle = memory.Pin();
            unsafe
            {
                int* pointer = (int*)handle.Pointer;
                Assert.True(pointer != null);

                GC.Collect();

                for (int i = 0; i < memory.Length; i++)
                {
                    Assert.Equal(array[i], pointer[i]);
                }
            }
            handle.Dispose();
        }

        [Fact]
        public static void MemoryFromEmptyArrayPin()
        {
            Memory<int> memory = new int[0];
            MemoryHandle handle = memory.Pin();
            unsafe
            {
                Assert.True(handle.Pointer != null);
            }
            handle.Dispose();
        }

        [Fact]
        public static void DefaultMemoryPin()
        {
            Memory<int> memory = default;
            MemoryHandle handle = memory.Pin();
            unsafe
            {
                Assert.True(handle.Pointer == null);
            }
            handle.Dispose();
        }

        [Fact]
        public static void MemoryPinAndSlice()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            Memory<int> memory = array;
            memory = memory.Slice(1);
            MemoryHandle handle = memory.Pin();
            Span<int> span = memory.Span;
            unsafe
            {
                int* pointer = (int*)handle.Pointer;
                Assert.True(pointer != null);

                GC.Collect();

                for (int i = 0; i < memory.Length; i++)
                {
                    Assert.Equal(array[i + 1], pointer[i]);
                }

                for (int i = 0; i < memory.Length; i++)
                {
                    Assert.Equal(array[i + 1], span[i]);
                }
            }
            handle.Dispose();
        }

        [Fact]
        public static void MemoryManagerPin()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            MemoryManager<int> manager = new CustomMemoryForTest<int>(array);
            Memory<int> memory = manager.Memory;
            MemoryHandle handle = memory.Pin();
            unsafe
            {
                int* pointer = (int*)handle.Pointer;
                Assert.True(pointer != null);

                GC.Collect();

                for (int i = 0; i < memory.Length; i++)
                {
                    Assert.Equal(array[i], pointer[i]);
                }
            }
            handle.Dispose();
        }

        [Fact]
        public static void MemoryManagerPinAndSlice()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            MemoryManager<int> manager = new CustomMemoryForTest<int>(array);
            Memory<int> memory = manager.Memory.Slice(1);
            MemoryHandle handle = memory.Pin();
            Span<int> span = memory.Span;
            unsafe
            {
                int* pointer = (int*)handle.Pointer;
                Assert.True(pointer != null);

                GC.Collect();

                for (int i = 0; i < memory.Length; i++)
                {
                    Assert.Equal(array[i + 1], pointer[i]);
                }

                for (int i = 0; i < memory.Length; i++)
                {
                    Assert.Equal(array[i + 1], span[i]);
                }
            }
            handle.Dispose();
        }
    }
}
