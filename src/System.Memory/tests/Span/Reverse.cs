// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;
using static System.TestHelpers;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ReverseEmpty()
        {
            var span = Span<byte>.Empty;
            span.Reverse();

            byte[] actual = { 1, 2, 3, 4 };
            byte[] expected = actual;
            span = actual;
            span.Slice(2, 0).Reverse();
            
            Assert.Equal<byte>(expected, span.ToArray());
        }

        [Fact]
        public static void ReverseEmptyWithReference()
        {
            var span = Span<string>.Empty;
            span.Reverse();

            string[] actual = { "a", "b", "c", "d" };
            string[] expected = actual;
            span = actual;
            span.Slice(2, 0).Reverse();

            Assert.Equal<string>(expected, span.ToArray());
        }

        [Fact]
        public static void ReverseByte()
        {
            byte[] actual = { 1, 2, 3, 4, 5 };
            byte[] expected = { 5, 4, 3, 2, 1 };
            Span<byte> span = actual;
            span.Reverse();
            Assert.Equal<byte>(expected, actual);
        }

        [Fact]
        public static void ReverseByteTwiceReturnsOriginal()
        {
            byte[] actual = { 1, 2, 3, 4, 5 };
            byte[] expected = actual;

            Span<byte> span = actual;
            span.Reverse();
            span.Reverse();
            Assert.Equal<byte>(expected, actual);
        }

        [Fact]
        public static void ReverseByteUnaligned()
        {
            const int length = 32;
            const int offset = 1;
            var actualFull = new byte[length];
            for (int i = 0; i < length; i++)
            {
                actualFull[i] = (byte)i;
            }
            byte[] expectedFull = actualFull;
            Array.Reverse(expectedFull, offset, length - offset - 1);

            var expectedSpan = new Span<byte>(expectedFull, offset, length - offset - 1);
            var actualSpan = new Span<byte>(actualFull, offset, length - offset - 1);
            actualSpan.Reverse();

            byte[] actual = actualSpan.ToArray();
            byte[] expected = expectedSpan.ToArray();
            Assert.Equal<byte>(expected, actual);
            Assert.Equal(expectedFull[0], actualFull[0]);
            Assert.Equal(expectedFull[length - 1], actualFull[length - 1]);
        }

        [Fact]
        public unsafe static void ReverseByteUnalignedFixed()
        {
            const int length = 32;
            const int offset = 1;
            var actualFull = new byte[length];
            for (int i = 0; i < length; i++)
            {
                actualFull[i] = (byte)i;
            }
            byte[] expectedFull = actualFull;
            Array.Reverse(expectedFull, offset, length - offset - 1);

            var expectedSpan = new Span<byte>(expectedFull, offset, length - offset - 1);
            fixed (byte* p = actualFull)
            {
                var actualSpan = new Span<byte>(p + offset, length - offset - 1);
                actualSpan.Reverse();

                byte[] actual = actualSpan.ToArray();
                byte[] expected = expectedSpan.ToArray();
                Assert.Equal<byte>(expected, actual);
                Assert.Equal(expectedFull[0], actualFull[0]);
                Assert.Equal(expectedFull[length - 1], actualFull[length - 1]);
            }
        }

        [Fact]
        public static void ReverseIntPtrOffset()
        {
            const int length = 32;
            const int offset = 2;
            var actualFull = new IntPtr[length];
            for (int i = 0; i < length; i++)
            {
                actualFull[i] = IntPtr.Zero + i;
            }
            IntPtr[] expectedFull = actualFull;
            Array.Reverse(expectedFull, offset, length - offset);
            
            var expectedSpan = new Span<IntPtr>(expectedFull, offset, length - offset);
            var actualSpan = new Span<IntPtr>(actualFull, offset, length - offset);
            actualSpan.Reverse();

            IntPtr[] actual = actualSpan.ToArray();
            IntPtr[] expected = expectedSpan.ToArray();
            Assert.Equal<IntPtr>(expected, actual);
            Assert.Equal(expectedFull[0], actualFull[0]);
            Assert.Equal(expectedFull[1], actualFull[1]);
        }

        [Fact]
        public static void ReverseValueTypeWithoutReferences()
        {
            int[] actual = { 1, 2, 3 };
            int[] expected = { 3, 2, 1 };

            var span = new Span<int>(actual);
            span.Reverse();
            Assert.Equal<int>(expected, actual);
        }

        [Fact]
        public static void ReverseValueTypeWithoutReferencesLonger()
        {
            const int length = 2048;
            int[] actual = new int[length];
            for (int i = 0; i < length; i++)
            {
                actual[i] = i;
            }
            int[] expected = actual;
            Array.Reverse(expected);

            var span = new Span<int>(actual);
            span.Reverse();
            Assert.Equal<int>(expected, actual);
        }

        [Fact]
        public static void ReverseValueTypeWithoutReferencesPointerSize()
        {
            const int length = 15;
            long[] actual = new long[length];
            for (int i = 0; i < length; i++)
            {
                actual[i] = i;
            }
            long[] expected = actual;
            Array.Reverse(expected);

            var span = new Span<long>(actual);
            span.Reverse();
            Assert.Equal<long>(expected, actual);
        }

        [Fact]
        public static void ReverseReferenceType()
        {
            string[] actual = { "a1", "b2", "c3" };
            string[] expected = { "c3", "b2", "a1" };

            var span = new Span<string>(actual);
            span.Reverse();
            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public static void ReverseReferenceTwiceReturnsOriginal()
        {
            string[] actual = { "a1", "b2", "c3" };
            string[] expected = actual;

            var span = new Span<string>(actual);
            span.Reverse();
            span.Reverse();
            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public static void ReverseReferenceTypeLonger()
        {
            const int length = 2048;
            string[] actual = new string[length];
            for (int i = 0; i < length; i++)
            {
                actual[i] = i.ToString();
            }
            string[] expected = actual;
            Array.Reverse(expected);

            var span = new Span<string>(actual);
            span.Reverse();
            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public static void ReverseEnumType()
        {
            TestEnum[] actual = { TestEnum.e0, TestEnum.e1, TestEnum.e2 };
            TestEnum[] expected = { TestEnum.e2, TestEnum.e1, TestEnum.e0 };

            var span = new Span<TestEnum>(actual);
            span.Reverse();
            Assert.Equal<TestEnum>(expected, actual);
        }

        [Fact]
        public static void ReverseValueTypeWithReferences()
        {
            TestValueTypeWithReference[] actual = {
                new TestValueTypeWithReference() { I = 1, S = "a" },
                new TestValueTypeWithReference() { I = 2, S = "b" },
                new TestValueTypeWithReference() { I = 3, S = "c" } };
            TestValueTypeWithReference[] expected = {
                new TestValueTypeWithReference() { I = 3, S = "c" },
                new TestValueTypeWithReference() { I = 2, S = "b" },
                new TestValueTypeWithReference() { I = 1, S = "a" } };

            var span = new Span<TestValueTypeWithReference>(actual);
            span.Reverse();
            Assert.Equal<TestValueTypeWithReference>(expected, actual);
        }

        // NOTE: ReverseLongerThanUintMaxValueBytes test is constrained to run on Windows and MacOSX because it causes
        //       problems on Linux due to the way deferred memory allocation works. On Linux, the allocation can
        //       succeed even if there is not enough memory but then the test may get killed by the OOM killer at the
        //       time the memory is accessed which triggers the full memory allocation.
        [Fact]
        [OuterLoop]
        [PlatformSpecific(TestPlatforms.Windows | TestPlatforms.OSX)]
        unsafe static void ReverseLongerThanUintMaxValueBytes()
        {
            if (sizeof(IntPtr) == sizeof(long))
            {
                // Arrange
                IntPtr bytes = (IntPtr)(((long)int.MaxValue) * sizeof(int));
                int length = (int)(((long)bytes) / sizeof(int));

                if (!AllocationHelper.TryAllocNative(bytes, out IntPtr memory))
                {
                    Console.WriteLine($"Span.Reverse test {nameof(ReverseLongerThanUintMaxValueBytes)} skipped (could not alloc memory).");
                    return;
                }

                try
                {
                    ref int data = ref Unsafe.AsRef<int>(memory.ToPointer());

                    for (int i = 0; i < length; i++)
                    {
                        Unsafe.Add(ref data, i) = i;
                    }

                    Span<int> span = new Span<int>(memory.ToPointer(), length);

                    // Act
                    span.Reverse();

                    // Assert using custom code for perf and to avoid allocating extra memory
                    for (int i = 0; i < length; i++)
                    {
                        var actual = Unsafe.Add(ref data, i);
                        if (actual != length - i - 1)
                        {
                            Assert.Equal(length - i - 1, actual);
                        }
                    }
                }
                finally
                {
                    AllocationHelper.ReleaseNative(ref memory);
                }
            }
        }
    }
}
