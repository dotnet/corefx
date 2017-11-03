// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Xunit;

namespace System.Buffers.Text.Tests
{
    public static partial class ParserTests
    {
        private const int TwoGiB = int.MaxValue;

        [Fact]
        [OuterLoop]
        public static void TestParser2GiBOverflow()
        {
            if (IntPtr.Size < 8)
                return;

            IntPtr pMemory;
            try
            {
                pMemory = Marshal.AllocHGlobal(int.MaxValue);
            }
            catch (OutOfMemoryException)
            {
                return;
            }

            try
            {
                TwoGiBOverflowHelper<int>(TwoGiBOverflowInt32TestData, pMemory);
            }
            finally
            {
                Marshal.FreeHGlobal(pMemory);
            }
        }

        private static void TwoGiBOverflowHelper<T>(IEnumerable<ParserTestData<T>> testDataCollection, IntPtr pMemory)
        {
            Assert.All<ParserTestData<T>>(testDataCollection,
                (testData) =>
                {
                    Console.WriteLine(testData);
                    unsafe
                    {
                        Span<byte> buffer = new Span<byte>((void*)pMemory, int.MaxValue);
                        ref byte memory = ref Unsafe.AsRef<byte>(pMemory.ToPointer());
                        Span<byte> span = new Span<byte>(pMemory.ToPointer(), TwoGiB);
                        span.Fill((byte)'0');

                        ReadOnlySpan<byte> utf8Span = testData.Text.ToUtf8Span();
                        byte sign = utf8Span[0];
                        if (sign == '-' || sign == '+')
                        {
                            span[0] = sign;
                            utf8Span = utf8Span.Slice(1);
                        }
                        utf8Span.CopyTo(span.Slice(TwoGiB - utf8Span.Length));

                        bool success = TryParseUtf8<T>(span, out T value, out int bytesConsumed, testData.FormatSymbol);
                        if (testData.ExpectedSuccess)
                        {
                            Assert.True(success);
                            Assert.Equal(testData.ExpectedValue, value);
                            Assert.Equal(testData.ExpectedBytesConsumed, bytesConsumed);
                        }
                        else
                        {
                            Assert.False(success);
                            Assert.Equal<T>(default, value);
                            Assert.Equal(0, bytesConsumed);
                        }
                    }

                });
        }

        private static IEnumerable<ParserTestData<int>> TwoGiBOverflowInt32TestData
        {
            get
            {
                yield return new ParserTestData<int>("0", 0, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("2", 2, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("21", 21, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("+2", 2, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("-2", -2, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("2147483647", 2147483647, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("-2147483648", -2147483648, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("2147483648", default, 'D', expectedSuccess: false);
                yield return new ParserTestData<int>("-2147483649", default, 'D', expectedSuccess: false);
                yield return new ParserTestData<int>("12345abcdefg1", 12345, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB - 8 };
                yield return new ParserTestData<int>("1234145abcdefg1", 1234145, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB - 8 };
                yield return new ParserTestData<int>("abcdefghijklmnop1", 0, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB - 17 };
                yield return new ParserTestData<int>("1147483648", 1147483648, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
                yield return new ParserTestData<int>("-1147483649", -1147483649, 'D', expectedSuccess: true) { ExpectedBytesConsumed = TwoGiB };
            }
        }
    }
}

