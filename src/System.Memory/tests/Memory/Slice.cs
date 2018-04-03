// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.MemoryTests
{
    public static partial class MemoryTests
    {
        [Fact]
        public static void SliceWithStart()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(6);
            Assert.Equal(4, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[6], ref MemoryMarshal.GetReference(memory.Span)));

            MemoryManager<int> manager = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromManager = manager.Memory.Slice(6);

            Assert.Equal(4, memoryFromManager.Length);
            Assert.True(Unsafe.AreSame(ref a[6], ref MemoryMarshal.GetReference(memoryFromManager.Span)));
        }

        [Fact]
        public static void SliceWithStartPastEnd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(a.Length);
            Assert.Equal(0, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref MemoryMarshal.GetReference(memory.Span), 1)));

            MemoryManager<int> manager = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromManager = manager.Memory.Slice(a.Length);

            Assert.Equal(0, memoryFromManager.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref MemoryMarshal.GetReference(memoryFromManager.Span), 1)));
        }

        [Fact]
        public static void SliceWithStartAndLength()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(3, 5);
            Assert.Equal(5, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[3], ref MemoryMarshal.GetReference(memory.Span)));

            MemoryManager<int> manager = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromManager = manager.Memory.Slice(3, 5);

            Assert.Equal(5, memoryFromManager.Length);
            Assert.True(Unsafe.AreSame(ref a[3], ref MemoryMarshal.GetReference(memoryFromManager.Span)));
        }

        [Fact]
        public static void SliceWithStartAndLengthUpToEnd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(4, 6);
            Assert.Equal(6, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[4], ref MemoryMarshal.GetReference(memory.Span)));

            MemoryManager<int> manager = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromManager = manager.Memory.Slice(4, 6);

            Assert.Equal(6, memoryFromManager.Length);
            Assert.True(Unsafe.AreSame(ref a[4], ref MemoryMarshal.GetReference(memoryFromManager.Span)));
        }

        [Fact]
        public static void SliceWithStartAndLengthPastEnd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(a.Length, 0);
            Assert.Equal(0, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref MemoryMarshal.GetReference(memory.Span), 1)));

            MemoryManager<int> manager = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromManager = manager.Memory.Slice(a.Length, 0);

            Assert.Equal(0, memoryFromManager.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref MemoryMarshal.GetReference(memoryFromManager.Span), 1)));
        }

        [Fact]
        public static void SliceRangeChecks()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(a.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(0, a.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(2, a.Length + 1 - 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(a.Length + 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Memory<int>(a).Slice(a.Length, 1));

            MemoryManager<int> manager = new CustomMemoryForTest<int>(a);
            Memory<int> memory = manager.Memory;

            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(a.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(0, a.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(2, a.Length + 1 - 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(a.Length + 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(a.Length, 1));
        }

        [Fact]
        public static void SliceWithStartDefaultMemory()
        {
            Memory<int> memory = default;
            memory = memory.Slice(0);
            Assert.True(memory.Equals(default));
        }

        [Fact]
        public static void SliceWithStartAndLengthDefaultMemory()
        {
            Memory<int> memory = default;
            memory = memory.Slice(0, 0);
            Assert.True(memory.Equals(default));
        }
    }
}
