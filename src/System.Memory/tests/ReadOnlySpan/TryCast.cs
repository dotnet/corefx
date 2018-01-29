// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.TestHelpers;

namespace System.SpanTests
{
    public static partial class MemoryMarshalTests
    {
        /// <summary>
        /// Tests an attempted cast from a 4-byte aligned type to a 4-byte aligned struct that is >4 total bytes in length.
        /// </summary>
        [Fact]
        public static unsafe void TryCastReadOnlySpan_TToStructLengthIsGreaterThanAlignment()
        {
            int[] a = { 100, 222 };
            ReadOnlySpan<int> span = new ReadOnlySpan<int>(a);
            ReadOnlySpan<PaddedStruct> asPaddedStruct;
            if (RuntimeInformation.OSArchitecture == Architecture.X64 ||
                RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                Assert.True(span.TryCast(out asPaddedStruct));
                Assert.Equal(span[1], asPaddedStruct[0].a2);
            }
            else
            {
                Assert.False(span.TryCast(out asPaddedStruct));
            }
        }

        /// <summary>
        /// Check that a TryCast from a 4-byte type to a struct with inserted padding up to a total length of 4 bytes
        /// will always succeed.
        /// </summary>
        [Fact]
        public static unsafe void TryCastReadOnlySpan_TToStructIsPadded()
        {
            int[] a = { 1, 2 };
            ReadOnlySpan<int> span = new ReadOnlySpan<int>(a);
            ReadOnlySpan<PaddedStruct4> asPaddedStruct;
            Assert.True(span.TryCast(out asPaddedStruct));
        }

        /// <summary>
        /// Test that a byte-aligned type can always be casted to another byte-aligned type.
        /// </summary>
        [Fact]
        public static unsafe void TryCastReadOnlySpan_TFrom3ByteStruct()
        {
            ThreeByteStruct[] a = { new ThreeByteStruct { a1 = (byte)'7', a2 = (byte)'1', a3 = (byte)'2' }, new ThreeByteStruct { a1 = (byte)'0', a2 = (byte)'1', a3 = (byte)'2' } };
            ReadOnlySpan<ThreeByteStruct> span = new ReadOnlySpan<ThreeByteStruct>(a);
            ReadOnlySpan<byte> asByte;
            Assert.True(span.TryCast(out asByte));
            Assert.Equal(a[0].a1, asByte[0]);
        }

        /// <summary>
        /// A 3-byte struct should be byte aligned, so any address is aligned
        /// </summary>
        [Fact]
        public static unsafe void TryCastReadOnlySpan_TTo3ByteStruct()
        {
            int[] a = { 1, 2};
            ReadOnlySpan<int> span = new ReadOnlySpan<int>(a);
            ReadOnlySpan<ThreeByteStruct> asThreeByteStruct;
            Assert.True(span.TryCast(out asThreeByteStruct));
            Assert.Equal(a[0], asThreeByteStruct[0].a1);
        }

        [Fact]
        public static unsafe void TryCastReadOnlySpan_TToSameSizeAsTFrom()
        {
            Length24_2[] a = { new Length24_2 { a1 = 0, a2 = 1, a3 = 2 }, new Length24_2 { a1 = 0, a2 = 1, a3 = 2 } };
            ReadOnlySpan<Length24_2> span = new ReadOnlySpan<Length24_2>(a);
            ReadOnlySpan<Length24> asLength24;
            Assert.True(span.TryCast(out asLength24));
            Assert.Equal(a[0].a1, (long)asLength24[0].a1);
        }

        [Fact]
        public static unsafe void TryCastReadOnlySpan_TToSameAsTFrom()
        {
            Length24[] a = { new Length24 { a1 = 0, a2 = 1, a3 = 2 }, new Length24 { a1 = 0, a2 = 1, a3 = 2 } };
            ReadOnlySpan<Length24> span = new ReadOnlySpan<Length24>(a);
            ReadOnlySpan<Length24> asLength24;
            Assert.True(span.TryCast(out asLength24));
            Assert.Equal(a[0].a1, asLength24[0].a1);
        }

        [Fact]
        public static unsafe void TryCastReadOnlySpan_ToLargeStruct()
        {
            long[] a = { 0x44332211, 0x88776655 };
            ReadOnlySpan<long> span = new ReadOnlySpan<long>(a);
            ReadOnlySpan<Length24> asLength24;
            Assert.True(span.TryCast(out asLength24));
        }

        [Fact]
        public static unsafe void TryCastReadOnlySpan_ToEmptyStruct()
        {
            long[] a = { 0x44332211, 0x88776655 };
            Span<long> span = new Span<long>(a);
            Span<EmptyStruct> asEmpty;
            Assert.True(span.TryCast(out asEmpty));
        }

        [Fact]
        public static unsafe void TryCastReadOnlySpan_FromUnalignedAddress()
        {
            // Create a span, get a pointer to it, add 1 to that pointer, then dangerously construct a new span at that unaligned pointer location
            uint[] a = { 0x44332211, 0x88776655 };
            ReadOnlySpan<uint> span = new ReadOnlySpan<uint>(a);
            void* pointerSpan = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)); // this can be moved, but we're not dereferencing the pointer from here on out anyways so it doesn't matter what actually lives there
            ReadOnlySpan<uint> offsetSpan = new ReadOnlySpan<uint>(new IntPtr(new IntPtr(pointerSpan).ToInt64() + 1).ToPointer(), span.Length);
            ReadOnlySpan<ushort> asUShort;
            if (RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.X86)
                Assert.True(offsetSpan.TryCast(out asUShort));
            else
                Assert.False(offsetSpan.TryCast(out asUShort));
        }

        [Fact]
        public static void TryCastReadOnlySpan_UIntToUShort()
        {
            uint[] a = { 0x44332211, 0x88776655 };
            ReadOnlySpan<uint> span = new ReadOnlySpan<uint>(a);
            ReadOnlySpan<ushort> asUShort;
            Assert.True(span.TryCast(out asUShort));

            Assert.True(Unsafe.AreSame<ushort>(ref Unsafe.As<uint, ushort>(ref Unsafe.AsRef(in MemoryMarshal.GetReference(span))), ref Unsafe.AsRef(in MemoryMarshal.GetReference(asUShort))));
            asUShort.Validate<ushort>(0x2211, 0x4433, 0x6655, 0x8877);
        }

        [Fact]
        public static void TryCastReadOnlySpan_ShortToLong()
        {
            short[] a = { 0x1234, 0x2345, 0x3456, 0x4567, 0x5678 };
            ReadOnlySpan<short> span = new ReadOnlySpan<short>(a);
            ReadOnlySpan<long> asLong;
            if (RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                Assert.True(span.TryCast(out asLong));
                Assert.True(Unsafe.AreSame<long>(ref Unsafe.As<short, long>(ref MemoryMarshal.GetReference(span)), ref MemoryMarshal.GetReference(asLong)));
                asLong.Validate<long>(0x4567345623451234);
            }
            else
            {
                Assert.False(span.TryCast(out asLong));
            }
        }

        [Fact]
        public static unsafe void TryCastReadOnlySpan_Overflow()
        {
            ReadOnlySpan<TestStructExplicit> span = new ReadOnlySpan<TestStructExplicit>(null, Int32.MaxValue);

            AssertThrows<OverflowException, TestStructExplicit>(span, (_span) => MemoryMarshal.Cast<TestStructExplicit, byte>(_span).DontBox());
            AssertThrows<OverflowException, TestStructExplicit>(span, (_span) => MemoryMarshal.Cast<TestStructExplicit, ulong>(_span).DontBox());
        }

        [Fact]
        public static void TryCastReadOnlySpan_ToTypeContainsReferences()
        {
            ReadOnlySpan<uint> span = new ReadOnlySpan<uint>(Array.Empty<uint>());
            AssertThrows<ArgumentException, uint>(span, (_span) => MemoryMarshal.Cast<uint, SpanTests.StructWithReferences>(_span).DontBox());
        }

        [Fact]
        public static void TryCastReadOnlySpan_FromTypeContainsReferences()
        {
            ReadOnlySpan<SpanTests.StructWithReferences> span = new ReadOnlySpan<SpanTests.StructWithReferences>(Array.Empty<SpanTests.StructWithReferences>());
            AssertThrows<ArgumentException, SpanTests.StructWithReferences>(span, (_span) => MemoryMarshal.Cast<SpanTests.StructWithReferences, uint>(_span).DontBox());
        }
    }
}
