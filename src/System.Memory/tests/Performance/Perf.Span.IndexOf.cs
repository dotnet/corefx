// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_IndexOf
    {
        private const int InnerCount = 100000;

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void SpanIndexOfChar(int size)
        {
            Span<char> charSpan = new char[size];
            charSpan[size / 2] = '5';

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= charSpan.IndexOf('5');
                    }
                }
            }
            Assert.Equal(size / 2, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void SpanIndexOfCharAsBytes(int size)
        {
            Span<char> charSpan = new char[size];
            charSpan[size / 2] = '5';
            Span<byte> byteSpan = MemoryMarshal.AsBytes(charSpan);

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= byteSpan.IndexOf<byte>(53);        // '5' = 53
                    }
                }
            }
            Assert.Equal(size > 1 ? size : 0, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void StringIndexOfChar(int size)
        {
            string str = new string('0', size / 2) + "5";
            if (size > 1)
            {
                str += new string('0', size / 2 - 1);
            }

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= str.IndexOf('5');
                    }
                }
            }
            Assert.Equal(size / 2, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1, StringComparison.CurrentCulture)]
        [InlineData(1, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(1, StringComparison.InvariantCulture)]
        [InlineData(1, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(1, StringComparison.Ordinal)]
        [InlineData(1, StringComparison.OrdinalIgnoreCase)]
        [InlineData(10, StringComparison.CurrentCulture)]
        [InlineData(10, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(10, StringComparison.InvariantCulture)]
        [InlineData(10, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(10, StringComparison.Ordinal)]
        [InlineData(10, StringComparison.OrdinalIgnoreCase)]
        [InlineData(100, StringComparison.CurrentCulture)]
        [InlineData(100, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(100, StringComparison.InvariantCulture)]
        [InlineData(100, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(100, StringComparison.Ordinal)]
        [InlineData(100, StringComparison.OrdinalIgnoreCase)]
        [InlineData(1000, StringComparison.CurrentCulture)]
        [InlineData(1000, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(1000, StringComparison.InvariantCulture)]
        [InlineData(1000, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(1000, StringComparison.Ordinal)]
        [InlineData(1000, StringComparison.OrdinalIgnoreCase)]
        public void SpanIndexOf_StringComparison_ASCII(int size, StringComparison comparisonType)
        {
            string str = new string('z', 10);
            str += new string('a', size / 2);
            if (comparisonType == StringComparison.CurrentCultureIgnoreCase || 
                comparisonType == StringComparison.InvariantCultureIgnoreCase || 
                comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                str += 'Z';
            }
            else
            {
                str += 'z';
            }
            if (size > 1)
            {
                str += new string('b', size / 2 - 1);
            }
            str += new string('z', 10);
            ReadOnlySpan<char> spanSource = str.AsSpan(10, size);
            ReadOnlySpan<char> spanValue = "z".AsSpan();

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= spanSource.IndexOf(spanValue, comparisonType);
                    }
                }
            }
            Assert.Equal(size / 2, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1, StringComparison.CurrentCulture)]
        [InlineData(1, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(1, StringComparison.InvariantCulture)]
        [InlineData(1, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(1, StringComparison.Ordinal)]
        [InlineData(1, StringComparison.OrdinalIgnoreCase)]
        [InlineData(10, StringComparison.CurrentCulture)]
        [InlineData(10, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(10, StringComparison.InvariantCulture)]
        [InlineData(10, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(10, StringComparison.Ordinal)]
        [InlineData(10, StringComparison.OrdinalIgnoreCase)]
        [InlineData(100, StringComparison.CurrentCulture)]
        [InlineData(100, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(100, StringComparison.InvariantCulture)]
        [InlineData(100, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(100, StringComparison.Ordinal)]
        [InlineData(100, StringComparison.OrdinalIgnoreCase)]
        [InlineData(1000, StringComparison.CurrentCulture)]
        [InlineData(1000, StringComparison.CurrentCultureIgnoreCase)]
        [InlineData(1000, StringComparison.InvariantCulture)]
        [InlineData(1000, StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(1000, StringComparison.Ordinal)]
        [InlineData(1000, StringComparison.OrdinalIgnoreCase)]
        public void SpanIndexOf_StringComparison_Unicode(int size, StringComparison comparisonType)
        {
            string str = new string('ž', 10);
            str += new string('ā', size / 2);
            if (comparisonType == StringComparison.CurrentCultureIgnoreCase || 
                comparisonType == StringComparison.InvariantCultureIgnoreCase || 
                comparisonType == StringComparison.OrdinalIgnoreCase)
            {
                str += 'Ž';
            }
            else
            {
                str += 'ž';
            }
            if (size > 1)
            {
                str += new string('ă', size / 2 - 1);
            }
            str += new string('ž', 10);
            ReadOnlySpan<char> spanSource = str.AsSpan(10, size);
            ReadOnlySpan<char> spanValue = "ž".AsSpan();

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= spanSource.IndexOf(spanValue, comparisonType);
                    }
                }
            }
            Assert.Equal(size / 2, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void SpanLastIndexOfChar(int size)
        {
            Span<char> charSpan = new char[size];
            charSpan[size / 2] = '5';

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= charSpan.LastIndexOf('5');
                    }
                }
            }
            Assert.Equal(size / 2, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void SpanLastIndexOfCharAsBytes(int size)
        {
            Span<char> charSpan = new char[size];
            charSpan[size / 2] = '5';
            Span<byte> byteSpan = MemoryMarshal.AsBytes(charSpan);

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= byteSpan.LastIndexOf<byte>(53);        // '5' = 53
                    }
                }
            }
            Assert.Equal(size > 1 ? size : 0, index);
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void StringLastIndexOfChar(int size)
        {
            string str = new string('0', size / 2) + "5";
            if (size > 1)
            {
                str += new string('0', size / 2 - 1);
            }

            int index = 0;
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        index |= str.LastIndexOf('5');
                    }
                }
            }
            Assert.Equal(size / 2, index);
        }

        private static string GenerateInputString(char source, int count, char replaceChar, int replacePos)
        {
            char[] str = new char[count];
            for (int i = 0; i < count; i++)
            {
                str[i] = replaceChar;
            }
            str[replacePos] = replaceChar;

            return new string(str);
        }

        public static IEnumerable<object[]> s_indexTestData = new List<object[]>
        {
            new object[] { "string1", "string2", StringComparison.InvariantCulture },
            new object[] { "foobardzsdzs", "rddzs", StringComparison.InvariantCulture },
            new object[] { "StrIng", "string", StringComparison.OrdinalIgnoreCase },
            new object[] { "\u3060", "\u305F", StringComparison.InvariantCulture },
            new object[] { "ABCDE", "c", StringComparison.InvariantCultureIgnoreCase },
            new object[] { "More Test's", "Tests", StringComparison.OrdinalIgnoreCase },
            new object[] { "Hello WorldbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbareallyreallylongHello WorldbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbareallyreallylongHello Worldbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbareallyreallylong!xyz", "~", StringComparison.Ordinal },
            new object[] { "Hello WorldbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbareallyreallylongHello WorldbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbareallyreallylongHello Worldbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbareallyreallylong!xyz", "w", StringComparison.OrdinalIgnoreCase },
            new object[] { "Hello Worldbbbbbbbbbbbbbbcbbbbbbbbbbbbbbbbbbba!", "y", StringComparison.Ordinal },
            new object[] { GenerateInputString('A', 10, '5', 5), "5", StringComparison.InvariantCulture },
            new object[] { GenerateInputString('A', 100, 'X', 70), "x", StringComparison.InvariantCultureIgnoreCase },
            new object[] { GenerateInputString('A', 100, 'X', 70), "x", StringComparison.OrdinalIgnoreCase },
            new object[] { GenerateInputString('A', 1000, 'X', 500), "X", StringComparison.Ordinal },
            new object[] { GenerateInputString('\u3060', 1000, 'x', 500), "x", StringComparison.Ordinal },
            new object[] { GenerateInputString('\u3060', 100, '\u3059', 50), "\u3059", StringComparison.Ordinal }
        };

        [Benchmark]
        [MemberData(nameof(s_indexTestData))]
        public void SpanIndexOfSpanComparison(string input, string value, StringComparison comparisonType)
        {
            ReadOnlySpan<char> inputSpan = input.AsSpan();
            ReadOnlySpan<char> valueSpan = value.AsSpan();

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    inputSpan.IndexOf(valueSpan, comparisonType);
                }
        }
    }
}
