// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Runtime.CompilerServices;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Text.Tests
{
    public static partial class Utf8FormatterTests
    {
        private const int InnerCount = 1000000;

        // There are really only two integer formatters: Int64/UInt64. Benchmarking the others won't provide any extra code coverage.

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(12837467L)] // standard format
        [InlineData(1283L)] // standard format short
        [InlineData(0L)]
        [InlineData(-9223372036854775808L)] // min value
        [InlineData(9223372036854775807L)] // max value
        [InlineData(-2147483648)] // int32 min value
        [InlineData(2147483647)] // int32 max value
        [InlineData(1000000000000000000)] // quintillion
        [InlineData(-1000000000000000000)] // negative quintillion
        private static void FormatterInt64(long value)
        {
            byte[] utf8ByteArray = new byte[40];
            Span<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Formatter.TryFormat(value, utf8ByteSpan, out int bytesWritten);
                        TestHelpers.DoNotIgnore(value, bytesWritten);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(12837467LU)] // standard format
        [InlineData(1283LU)] // standard format short
        [InlineData(0LU)] // min value
        [InlineData(18446744073709551615LU)] // max value
        [InlineData(2147483647)] // int32 max value
        [InlineData(1000000000000000000)] // quintillion
        private static void FormatterUInt64(ulong value)
        {
            byte[] utf8ByteArray = new byte[40];
            Span<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Formatter.TryFormat(value, utf8ByteSpan, out int bytesWritten);
                        TestHelpers.DoNotIgnore(value, bytesWritten);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void FormatterDateTimeOffsetNow()
        {
            DateTimeOffset value = new DateTimeOffset(year: 2017, month: 12, day: 30, hour: 3, minute: 45, second: 22, millisecond: 950, offset: new TimeSpan(hours: -8, minutes: 0, seconds: 0));

            byte[] utf8ByteArray = new byte[40];
            Span<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Utf8Formatter.TryFormat(value, utf8ByteSpan, out int bytesWritten);
                        TestHelpers.DoNotIgnore(value, bytesWritten);
                    }
                }
            }
        }
    }
}
