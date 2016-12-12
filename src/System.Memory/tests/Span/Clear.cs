// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ClearEmpty()
        {
            var span = Span<byte>.Empty;
            span.Clear();
        }

        [Fact]
        public static void ClearByteLonger()
        {
            const byte initial = 5;
            var actual = new byte[2048];
            for (int i = 0; i < actual.Length; i++)
            {
                actual[i] = initial;
            }
            var expected = new byte[actual.Length];

            var span = new Span<byte>(actual);
            span.Clear();
            Assert.Equal<byte>(expected, actual);
        }

        [Fact]
        public static void ClearByteUnaligned()
        {
            const byte initial = 5;
            const int length = 32;
            var actualFull = new byte[length];
            for (int i = 0; i < length; i++)
            {
                actualFull[i] = initial;
            }
            var expectedFull = new byte[length];

            var start = 1;
            var expectedSpan = new Span<byte>(expectedFull, start, length - start - 1);
            var actualSpan = new Span<byte>(actualFull, start, length - start - 1);
            actualSpan.Clear();

            var actual = actualSpan.ToArray();
            var expected = expectedSpan.ToArray();
            Assert.Equal<byte>(expected, actual);
            Assert.Equal(initial, actualFull[0]);
            Assert.Equal(initial, actualFull[length - 1]);
        }

        [Fact]
        public unsafe static void ClearByteUnalignedFixed()
        {
            const byte initial = 5;
            const int length = 32;
            var actualFull = new byte[length];
            for (int i = 0; i < length; i++)
            {
                actualFull[i] = initial;
            }
            var expectedFull = new byte[length];

            var start = 1;
            var expectedSpan = new Span<byte>(expectedFull, start, length - start - 1);
            fixed (byte* p = actualFull)
            {
                var actualSpan = new Span<byte>(p + start, length - start - 1);
                actualSpan.Clear();

                var actual = actualSpan.ToArray();
                var expected = expectedSpan.ToArray();
                Assert.Equal<byte>(expected, actual);
                Assert.Equal(initial, actualFull[0]);
                Assert.Equal(initial, actualFull[length - 1]);
            }
        }

        [Fact]
        public static void ClearValueTypeWithoutReferences()
        {
            int[] actual = { 1, 2, 3 };
            int[] expected = { 0, 0, 0 };

            var span = new Span<int>(actual);
            span.Clear();
            Assert.Equal<int>(expected, actual);
        }

        [Fact]
        public static void ClearValueTypeWithoutReferencesLonger()
        {
            int[] actual = new int[2048];
            for (int i = 0; i < actual.Length; i++)
            {
                actual[i] = i + 1;
            }
            int[] expected = new int[actual.Length];

            var span = new Span<int>(actual);
            span.Clear();
            Assert.Equal<int>(expected, actual);
        }

        [Fact]
        public static void ClearReferenceType()
        {
            string[] actual = { "a", "b", "c" };
            string[] expected = { null, null, null };

            var span = new Span<string>(actual);
            span.Clear();
            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public static void ClearReferenceTypeLonger()
        {
            string[] actual = new string[2048];
            for (int i = 0; i < actual.Length; i++)
            {
                actual[i] = (i + 1).ToString();
            }
            string[] expected = new string[actual.Length];

            var span = new Span<string>(actual);
            span.Clear();
            Assert.Equal<string>(expected, actual);
        }

        [Fact]
        public static void ClearValueTypeWithReferences()
        {
            TestValueTypeWithReference[] actual = {
                new TestValueTypeWithReference() { I = 1, S = "a" },
                new TestValueTypeWithReference() { I = 2, S = "b" },
                new TestValueTypeWithReference() { I = 3, S = "c" } };
            TestValueTypeWithReference[] expected = {
                default(TestValueTypeWithReference),
                default(TestValueTypeWithReference),
                default(TestValueTypeWithReference) };

            var span = new Span<TestValueTypeWithReference>(actual);
            span.Clear();
            Assert.Equal<TestValueTypeWithReference>(expected, actual);
        }
    }
}
