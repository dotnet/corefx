// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        [Fact]
        public static void ZeroLengthSequenceEqual()
        {
            int[] a = new int[3];

            Span<int> first = new Span<int>(a, 1, 0);
            Span<int> second = new Span<int>(a, 2, 0);
            bool b = first.SequenceEqual(second);
            Assert.True(b);
        }

        [Fact]
        public static void SameSpanSequenceEqual()
        {
            int[] a = { 4, 5, 6 };
            Span<int> span = new Span<int>(a);
            bool b = span.SequenceEqual(span);
            Assert.True(b);
        }

        [Fact]
        public static void LengthMismatchSequenceEqual()
        {
            int[] a = { 4, 5, 6 };
            Span<int> first = new Span<int>(a, 0, 3);
            Span<int> second = new Span<int>(a, 0, 2);
            bool b = first.SequenceEqual(second);
            Assert.False(b);
        }

        [Fact]
        public static void OnSequenceEqualOfEqualSpansMakeSureEveryElementIsCompared()
        {
            for (int length = 0; length < 100; length++)
            {
                TIntLog log = new TIntLog();

                TInt[] first = new TInt[length];
                TInt[] second = new TInt[length];
                for (int i = 0; i < length; i++)
                {
                    first[i] = second[i] = new TInt(10 * (i + 1), log);
                }

                Span<TInt> firstSpan = new Span<TInt>(first);
                ReadOnlySpan<TInt> secondSpan = new ReadOnlySpan<TInt>(second);
                bool b = firstSpan.SequenceEqual(secondSpan);
                Assert.True(b);

                // Make sure each element of the array was compared once. (Strictly speaking, it would not be illegal for 
                // SequenceEqual to compare an element more than once but that would be a non-optimal implementation and 
                // a red flag. So we'll stick with the stricter test.)
                Assert.Equal(first.Length, log.Count);
                foreach (TInt elem in first)
                {
                    int numCompares = log.CountCompares(elem.Value, elem.Value);
                    Assert.True(numCompares == 1, $"Expected {numCompares} == 1 for element {elem.Value}.");
                }
            }
        }

        [Fact]
        public static void SequenceEqualNoMatch()
        {
            for (int length = 1; length < 32; length++)
            {
                for (int mismatchIndex = 0; mismatchIndex < length; mismatchIndex++)
                {
                    TIntLog log = new TIntLog();

                    TInt[] first = new TInt[length];
                    TInt[] second = new TInt[length];
                    for (int i = 0; i < length; i++)
                    {
                        first[i] = second[i] = new TInt(10 * (i + 1), log);
                    }

                    second[mismatchIndex] = new TInt(second[mismatchIndex].Value + 1, log);

                    Span<TInt> firstSpan = new Span<TInt>(first);
                    ReadOnlySpan<TInt> secondSpan = new ReadOnlySpan<TInt>(second);
                    bool b = firstSpan.SequenceEqual(secondSpan);
                    Assert.False(b);

                    Assert.Equal(1, log.CountCompares(first[mismatchIndex].Value, second[mismatchIndex].Value));
                }
            }
        }

        [Fact]
        public static void MakeSureNoSequenceEqualChecksGoOutOfRange()
        {
            const int GuardValue = 77777;
            const int GuardLength = 50;

            Action<int, int> checkForOutOfRangeAccess =
                delegate (int x, int y)
                {
                    if (x == GuardValue || y == GuardValue)
                        throw new Exception("Detected out of range access in IndexOf()");
                };

            for (int length = 0; length < 100; length++)
            {
                TInt[] first = new TInt[GuardLength + length + GuardLength];
                TInt[] second = new TInt[GuardLength + length + GuardLength];
                for (int i = 0; i < first.Length; i++)
                {
                    first[i] = second[i] = new TInt(GuardValue, checkForOutOfRangeAccess);
                }

                for (int i = 0; i < length; i++)
                {
                    first[GuardLength + i] = second[GuardLength + i] = new TInt(10 * (i + 1), checkForOutOfRangeAccess);
                }

                Span<TInt> firstSpan = new Span<TInt>(first, GuardLength, length);
                Span<TInt> secondSpan = new Span<TInt>(second, GuardLength, length);
                bool b = firstSpan.SequenceEqual(secondSpan);
                Assert.True(b);
            }
        }
    }
}
