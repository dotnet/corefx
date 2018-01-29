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
        public static unsafe void TryCastSpan_TToStructLengthIsGreaterThanAlignment()
        {
            int[] a = { 100, 222 };
            Span<int> span = new Span<int>(a);
            Span<PaddedStruct> asPaddedStruct;
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
        public static unsafe void TryCastSpan_TToStructIsPadded()
        {
            int[] a = { 1, 2 };
            Span<int> span = new Span<int>(a);
            Span<PaddedStruct4> asPaddedStruct;
            Assert.True(span.TryCast(out asPaddedStruct));
        }

        /// <summary>
        /// Test that a byte-aligned type can always be casted to another byte-aligned type.
        /// </summary>
        [Fact]
        public static unsafe void TryCastSpan_TFrom3ByteStruct()
        {
            ThreeByteStruct[] a = { new ThreeByteStruct { a1 = (byte)'7', a2 = (byte)'1', a3 = (byte)'2' }, new ThreeByteStruct { a1 = (byte)'0', a2 = (byte)'1', a3 = (byte)'2' } };
            Span<ThreeByteStruct> span = new Span<ThreeByteStruct>(a);
            Span<byte> asByte;
            Assert.True(span.TryCast(out asByte));
            Assert.Equal(a[0].a1, asByte[0]);
        }

        /// <summary>
        /// A 3-byte struct should be byte aligned, so any address is aligned
        /// </summary>
        [Fact]
        public static unsafe void TryCastSpan_TTo3ByteStruct()
        {
            int[] a = { 1, 2 };
            Span<int> span = new Span<int>(a);
            Span<ThreeByteStruct> asThreeByteStruct;
            Assert.True(span.TryCast(out asThreeByteStruct));
            Assert.Equal(a[0], asThreeByteStruct[0].a1);
        }

        [Fact]
        public static unsafe void TryCastSpan_TToSameSizeAsTFrom()
        {
            Length24_2[] a = { new Length24_2 { a1 = 0, a2 = 1, a3 = 2 }, new Length24_2 { a1 = 0, a2 = 1, a3 = 2 } };
            Span<Length24_2> span = new Span<Length24_2>(a);
            Span<Length24> asLength24;
            Assert.True(span.TryCast(out asLength24));
            Assert.Equal(a[0].a1, (long)asLength24[0].a1);
        }

        [Fact]
        public static unsafe void TryCastSpan_TToSameAsTFrom()
        {
            Length24[] a = { new Length24 { a1 = 0, a2 = 1, a3 = 2 }, new Length24 { a1 = 0, a2 = 1, a3 = 2 } };
            Span<Length24> span = new Span<Length24>(a);
            Span<Length24> asLength24;
            Assert.True(span.TryCast(out asLength24));
            Assert.Equal(a[0].a1, asLength24[0].a1);
        }

        [Fact]
        public static unsafe void TryCastSpan_ToLargeStruct()
        {
            long[] a = { 0x44332211, 0x88776655 };
            Span<long> span = new Span<long>(a);
            Span<Length24> asLength24;
            Assert.True(span.TryCast(out asLength24));
        }

        [Fact]
        public static unsafe void TryCastSpan_ToEmptyStruct()
        {
            long[] a = { 0x44332211, 0x88776655 };
            Span<long> span = new Span<long>(a);
            Span<EmptyStruct> asEmpty;
            Assert.True(span.TryCast(out asEmpty));
        }

        [Fact]
        public static unsafe void TryCastSpan_FromUnalignedAddress()
        {
            // Create a span, get a pointer to it, add 1 to that pointer, then dangerously construct a new span at that unaligned pointer location
            uint[] a = { 0x44332211, 0x88776655 };
            Span<uint> span = new Span<uint>(a);
            void* pointerSpan = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span)); // this can be moved, but we're not dereferencing the pointer from here on out anyways so it doesn't matter what actually lives there
            Span<uint> offsetSpan = new Span<uint>(new IntPtr(new IntPtr(pointerSpan).ToInt64() + 1).ToPointer(), span.Length);
            Span<ushort> asUShort;
            if (RuntimeInformation.OSArchitecture == Architecture.X64 || RuntimeInformation.OSArchitecture == Architecture.X86)
                Assert.True(offsetSpan.TryCast(out asUShort));
            else
                Assert.False(offsetSpan.TryCast(out asUShort));
        }

        [Fact]
        public static void TryCastSpan_UIntToUShort()
        {
            uint[] a = { 0x44332211, 0x88776655 };
            Span<uint> span = new Span<uint>(a);
            Span<ushort> asUShort;
            Assert.True(span.TryCast(out asUShort));

            Assert.True(Unsafe.AreSame<ushort>(ref Unsafe.As<uint, ushort>(ref Unsafe.AsRef(in MemoryMarshal.GetReference(span))), ref Unsafe.AsRef(in MemoryMarshal.GetReference(asUShort))));
            asUShort.Validate<ushort>(0x2211, 0x4433, 0x6655, 0x8877);
        }

        [Fact]
        public static void TryCastSpan_ShortToLong()
        {
            short[] a = { 0x1234, 0x2345, 0x3456, 0x4567, 0x5678 };
            Span<short> span = new Span<short>(a);
            Span<long> asLong;
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
        public static unsafe void TryCastSpan_Overflow()
        {
            Span<TestStructExplicit> span = new Span<TestStructExplicit>(null, Int32.MaxValue);

            AssertThrows<OverflowException, TestStructExplicit>(span, (_span) => MemoryMarshal.Cast<TestStructExplicit, byte>(_span).DontBox());
            AssertThrows<OverflowException, TestStructExplicit>(span, (_span) => MemoryMarshal.Cast<TestStructExplicit, ulong>(_span).DontBox());
        }

        [Fact]
        public static void TryCastSpan_ToTypeContainsReferences()
        {
            Span<uint> span = new Span<uint>(Array.Empty<uint>());
            AssertThrows<ArgumentException, uint>(span, (_span) => MemoryMarshal.Cast<uint, SpanTests.StructWithReferences>(_span).DontBox());
        }

        [Fact]
        public static void TryCastSpan_FromTypeContainsReferences()
        {
            Span<SpanTests.StructWithReferences> span = new Span<SpanTests.StructWithReferences>(Array.Empty<SpanTests.StructWithReferences>());
            AssertThrows<ArgumentException, SpanTests.StructWithReferences>(span, (_span) => MemoryMarshal.Cast<SpanTests.StructWithReferences, uint>(_span).DontBox());
        }
    }
}
