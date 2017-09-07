// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Buffers;
using Xunit;

namespace System.MemoryTests
{
    //
    // Tests for internal Memory<T>.ctor(OwnedMemory<T>, int , int)
    //
    public static partial class MemoryTests
    {
        [Fact]
        public static void MemoryFromOwnedMemoryInt()
        {
            int[] a = { 91, 92, -93, 94 };
            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Memory<int> memory = owner.AsMemory;
            memory.Validate(91, 92, -93, 94);
        }

        [Fact]
        public static void MemoryFromOwnedMemoryLong()
        {
            long[] a = { 91, -92, 93, 94, -95 };
            OwnedMemory<long> owner = new CustomMemoryForTest<long>(a);
            Memory<long> memory = owner.AsMemory;
            memory.Validate(91, -92, 93, 94, -95);
        }

        [Fact]
        public static void MemoryFromOwnedMemoryObject()
        {
            object o1 = new object();
            object o2 = new object();
            object[] a = { o1, o2 };
            OwnedMemory<object> owner = new CustomMemoryForTest<object>(a);
            Memory<object> memory = owner.AsMemory;
            memory.ValidateReferenceType(o1, o2);
        }

        [Fact]
        public static void ImplicitReadOnlyMemoryFromOwnedMemory()
        {
            long[] a = { 91, -92, 93, 94, -95 };
            OwnedMemory<long> owner = new CustomMemoryForTest<long>(a);
            Memory<long> memory = owner.AsMemory;
            CastReadOnly<long>(memory, 91, -92, 93, 94, -95);
        }

        [Fact]
        public static void OwnedMemoryDispose()
        {
            int[] a = { 91, 92, -93, 94 };
            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            Assert.False(owner.IsDisposed);
            owner.Dispose();
            Assert.True(owner.IsDisposed);
        }

        [Fact]
        public static void MemoryFromOwnedMemoryAfterDispose()
        {
            int[] a = { 91, 92, -93, 94 };
            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            owner.Dispose();
            Assert.Throws<ObjectDisposedException>(() => owner.AsMemory);
        }

        [Fact]
        public static void DisposeOwnedMemoryAfterRetain()
        {
            int[] a = { 91, 92, -93, 94 };
            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            owner.Retain();
            Assert.Throws<InvalidOperationException>(() => owner.Dispose());
            owner.Release();
        }

        [Fact]
        public static void DisposeOwnedMemoryAfterRetainAndRelease()
        {
            int[] a = { 91, 92, -93, 94 };
            OwnedMemory<int> owner = new CustomMemoryForTest<int>(a);
            owner.Retain();
            owner.Release();
            owner.Dispose();
            Assert.True(owner.IsDisposed);
        }
    }
    
}

