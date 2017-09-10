// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;
using System.Buffers;

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
            Assert.True(Unsafe.AreSame(ref a[6], ref memory.Span.DangerousGetPinnableReference()));

            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromOwner = owner.AsMemory.Slice(6);

            Assert.Equal(4, memoryFromOwner.Length);
            Assert.True(Unsafe.AreSame(ref a[6], ref memoryFromOwner.Span.DangerousGetPinnableReference()));
        }

        [Fact]
        public static void SliceWithStartPastEnd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(a.Length);
            Assert.Equal(0, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref memory.Span.DangerousGetPinnableReference(), 1)));

            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromOwner = owner.AsMemory.Slice(a.Length);

            Assert.Equal(0, memoryFromOwner.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref memoryFromOwner.Span.DangerousGetPinnableReference(), 1)));
        }

        [Fact]
        public static void SliceWithStartAndLength()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(3, 5);
            Assert.Equal(5, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[3], ref memory.Span.DangerousGetPinnableReference()));

            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromOwner = owner.AsMemory.Slice(3, 5);

            Assert.Equal(5, memoryFromOwner.Length);
            Assert.True(Unsafe.AreSame(ref a[3], ref memoryFromOwner.Span.DangerousGetPinnableReference()));
        }

        [Fact]
        public static void SliceWithStartAndLengthUpToEnd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(4, 6);
            Assert.Equal(6, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[4], ref memory.Span.DangerousGetPinnableReference()));

            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromOwner = owner.AsMemory.Slice(4, 6);

            Assert.Equal(6, memoryFromOwner.Length);
            Assert.True(Unsafe.AreSame(ref a[4], ref memoryFromOwner.Span.DangerousGetPinnableReference()));
        }

        [Fact]
        public static void SliceWithStartAndLengthPastEnd()
        {
            int[] a = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            Memory<int> memory = new Memory<int>(a).Slice(a.Length, 0);
            Assert.Equal(0, memory.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref memory.Span.DangerousGetPinnableReference(), 1)));

            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memoryFromOwner = owner.AsMemory.Slice(a.Length, 0);

            Assert.Equal(0, memoryFromOwner.Length);
            Assert.True(Unsafe.AreSame(ref a[a.Length - 1], ref Unsafe.Subtract(ref memoryFromOwner.Span.DangerousGetPinnableReference(), 1)));
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

            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memory = owner.AsMemory;

            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(a.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(0, a.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(2, a.Length + 1 - 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(a.Length + 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.Slice(a.Length, 1));
        }
    }
}
