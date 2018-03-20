// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ZeroLengthSequenceCompareTo_Long()
        {
            long[] a = new long[3];

            Span<long> first = new Span<long>(a, 1, 0);
            Span<long> second = new Span<long>(a, 2, 0);
            int result = first.SequenceCompareTo<long>(second);
            Assert.Equal(0, result);
        }

        [Fact]
        public static void SameSpanSequenceCompareTo_Long()
        {
            long[] a = { 4, 5, 6 };
            Span<long> span = new Span<long>(a);
            int result = span.SequenceCompareTo<long>(span);
            Assert.Equal(0, result);
        }

        [Fact]
        public static void SequenceCompareToArrayImplicit_Long()
        {
            long[] a = { 4, 5, 6 };
            Span<long> first = new Span<long>(a, 0, 3);
            int result = first.SequenceCompareTo<long>(a);
            Assert.Equal(0, result);
        }

        [Fact]
        public static void SequenceCompareToArraySegmentImplicit_Long()
        {
            long[] src = { 1, 2, 3 };
            long[] dst = { 5, 1, 2, 3, 10 };
            var segment = new ArraySegment<long>(dst, 1, 3);

            Span<long> first = new Span<long>(src, 0, 3);
            int result = first.SequenceCompareTo<long>(segment);
            Assert.Equal(0, result);
        }

        [Fact]
        public static void LengthMismatchSequenceCompareTo_Long()
        {
            long[] a = { 4, 5, 6 };
            Span<long> first = new Span<long>(a, 0, 2);
            Span<long> second = new Span<long>(a, 0, 3);
            int result = first.SequenceCompareTo<long>(second);
            Assert.True(result < 0);

            result = second.SequenceCompareTo<long>(first);
            Assert.True(result > 0);

            // one sequence is empty
            first = new Span<long>(a, 1, 0);

            result = first.SequenceCompareTo<long>(second);
            Assert.True(result < 0);

            result = second.SequenceCompareTo<long>(first);
            Assert.True(result > 0);
        }

        [Fact]
        public static void SequenceCompareToWithSingleMismatch_Long()
        {
            for (int length = 1; length < 32; length++)
            {
                for (int mismatchIndex = 0; mismatchIndex < length; mismatchIndex++)
                {
                    long[] first = new long[length];
                    long[] second = new long[length];
                    for (int i = 0; i < length; i++)
                    {
                        first[i] = second[i] = (long)(i + 1);
                    }

                    second[mismatchIndex] = (long)(second[mismatchIndex] + 1);

                    Span<long> firstSpan = new Span<long>(first);
                    ReadOnlySpan<long> secondSpan = new ReadOnlySpan<long>(second);
                    int result = firstSpan.SequenceCompareTo<long>(secondSpan);
                    Assert.True(result < 0);

                    result = secondSpan.SequenceCompareTo<long>(firstSpan);
                    Assert.True(result > 0);
                }
            }
        }

        [Fact]
        public static void SequenceCompareToNoMatch_Long()
        {
            for (int length = 1; length < 32; length++)
            {
                long[] first = new long[length];
                long[] second = new long[length];

                for (int i = 0; i < length; i++)
                {
                    first[i] = (long)(i + 1);
                    second[i] = (long)(long.MaxValue - i);
                }

                Span<long> firstSpan = new Span<long>(first);
                ReadOnlySpan<long> secondSpan = new ReadOnlySpan<long>(second);
                int result = firstSpan.SequenceCompareTo<long>(secondSpan);
                Assert.True(result < 0);

                result = secondSpan.SequenceCompareTo<long>(firstSpan);
                Assert.True(result > 0);
            }
        }

        [Fact]
        public static void MakeSureNoSequenceCompareToChecksGoOutOfRange_Long()
        {
            for (int length = 0; length < 100; length++)
            {
                long[] first = new long[length + 2];
                first[0] = 99;
                first[length + 1] = 99;
                long[] second = new long[length + 2];
                second[0] = 100;
                second[length + 1] = 100;
                Span<long> span1 = new Span<long>(first, 1, length);
                ReadOnlySpan<long> span2 = new ReadOnlySpan<long>(second, 1, length);
                int result = span1.SequenceCompareTo<long>(span2);
                Assert.Equal(0, result);
            }
        }
    }
}
