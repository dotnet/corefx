// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Text.Tests
{
    public static partial class Utf8FormatterTests
    {
        private const int InnerCount = 100000;

        // There are really only two integer formatters: Int64/UInt64. Benchmarking the others won't provide any extra code coverage.

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(214748364L)]
        [InlineData(2L)]
        [InlineData(21474836L)]
        [InlineData(21474L)]
        [InlineData(214L)]
        [InlineData(2147L)]
        [InlineData(214748L)]
        [InlineData(21L)]
        [InlineData(2147483L)]
        [InlineData(922337203685477580L)]
        [InlineData(92233720368547758L)]
        [InlineData(9223372036854775L)]
        [InlineData(922337203685477L)]
        [InlineData(92233720368547L)]
        [InlineData(9223372036854L)]
        [InlineData(922337203685L)]
        [InlineData(92233720368L)]
        [InlineData(-214748364L)]
        [InlineData(-2L)]
        [InlineData(-21474836L)]
        [InlineData(-21474L)]
        [InlineData(-214L)]
        [InlineData(-2147L)]
        [InlineData(-214748L)]
        [InlineData(-21L)]
        [InlineData(-2147483L)]
        [InlineData(-922337203685477580L)]
        [InlineData(-92233720368547758L)]
        [InlineData(-9223372036854775L)]
        [InlineData(-922337203685477L)]
        [InlineData(-92233720368547L)]
        [InlineData(-9223372036854L)]
        [InlineData(-922337203685L)]
        [InlineData(-92233720368L)]
        [InlineData(0L)]
        [InlineData(-9223372036854775808L)] // min value
        [InlineData(9223372036854775807L)] // max value
        [InlineData(-2147483648L)] // int32 min value
        [InlineData(2147483647L)] // int32 max value
        [InlineData(-4294967295000000000L)] // -(uint.MaxValue * Billion)
        [InlineData(4294967295000000000L)] // uint.MaxValue * Billion
        [InlineData(-4294967295000000001L)] // -(uint.MaxValue * Billion + 1)
        [InlineData(4294967295000000001L)] // uint.MaxValue * Billion + 1
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
        [InlineData(-12837467)] // standard format
        [InlineData(12837467)] // standard format
        [InlineData(-1283)] // standard format short
        [InlineData(1283)] // standard format short
        [InlineData(0)]
        [InlineData(-2147483648)] // int32 min value
        [InlineData(2147483647)] // int32 max value
        private static void FormatterInt32(int value)
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
        [InlineData(214748364LU)]
        [InlineData(2LU)]
        [InlineData(21474836LU)]
        [InlineData(21474LU)]
        [InlineData(214LU)]
        [InlineData(2147LU)]
        [InlineData(214748LU)]
        [InlineData(21LU)]
        [InlineData(2147483LU)]
        [InlineData(922337203685477580LU)]
        [InlineData(92233720368547758LU)]
        [InlineData(9223372036854775LU)]
        [InlineData(922337203685477LU)]
        [InlineData(92233720368547LU)]
        [InlineData(9223372036854LU)]
        [InlineData(922337203685LU)]
        [InlineData(92233720368LU)]
        [InlineData(0LU)] // min value
        [InlineData(18446744073709551615LU)] // max value
        [InlineData(2147483647LU)] // int32 max value
        [InlineData(9223372036854775807LU)] // int64 max value
        [InlineData(1000000000000000000LU)] // quintillion
        [InlineData(4294967295000000000LU)] // uint.MaxValue * Billion
        [InlineData(4294967295000000001LU)] // uint.MaxValue * Billion + 1
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

        [Benchmark(InnerIterationCount = 4_000_000)]
        private static void FormatterDecimal()
        {
            decimal value = new decimal(1.23456789E+5);

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

        // Reenable commented out test cases when https://github.com/xunit/xunit/issues/1822 is fixed.
        [Benchmark]
        [InlineData(double.NegativeInfinity, 10_000_000)]   // Negative Infinity
        [InlineData(double.MinValue, 100_000)]              // Min Negative Normal
        [InlineData(-3.14159265358979324, 1_000_000)]       // Negative pi
        [InlineData(-2.71828182845904524, 1_000_000)]       // Negative e
        [InlineData(-1.0, 1_000_000)]                       // Negative One
        // [InlineData(-2.2250738585072014E-308, 100_000)]     // Max Negative Normal
        [InlineData(-2.2250738585072009E-308, 100_000)]     // Min Negative Subnormal
        [InlineData(-double.Epsilon, 100_000)]              // Max Negative Subnormal (Negative Epsilon)
        [InlineData(-0.0, 10_000_000)]                      // Negative Zero
        [InlineData(double.NaN, 10_000_000)]                // NaN
        [InlineData(0.0, 10_000_000)]                       // Positive Zero
        [InlineData(double.Epsilon, 100_000)]               // Min Positive Subnormal (Positive Epsilon)
        [InlineData(2.2250738585072009E-308, 100_000)]      // Max Positive Subnormal
        // [InlineData(2.2250738585072014E-308, 100_000)]      // Min Positive Normal
        [InlineData(1.0, 1_000_000)]                        // Positive One
        [InlineData(2.71828182845904524, 1_000_000)]        // Positive e
        [InlineData(3.14159265358979324, 1_000_000)]        // Positive pi
        [InlineData(double.MaxValue, 100_000)]              // Max Positive Normal
        [InlineData(double.PositiveInfinity, 10_000_000)]   // Positive Infinity
        private static void FormatterDouble(double value, int innerIterations)
        {
            byte[] utf8ByteArray = new byte[40];
            Span<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterations; i++)
                    {
                        Utf8Formatter.TryFormat(value, utf8ByteSpan, out int bytesWritten);
                        TestHelpers.DoNotIgnore(value, bytesWritten);
                    }
                }
            }
        }

        // Reenable commented out test cases when https://github.com/xunit/xunit/issues/1822 is fixed.
        [Benchmark]
        [InlineData(float.NegativeInfinity, 10_000_000)]    // Negative Infinity
        [InlineData(float.MinValue, 100_000)]               // Min Negative Normal
        [InlineData(-3.14159265f, 1_000_000)]               // Negative pi
        [InlineData(-2.71828183f, 1_000_000)]               // Negative e
        [InlineData(-1.0f, 1_000_000)]                      // Negative One
        // [InlineData(-1.17549435E-38f, 100_000)]             // Max Negative Normal
        [InlineData(-1.17549421E-38f, 100_000)]             // Min Negative Subnormal
        [InlineData(-float.Epsilon, 100_000)]               // Max Negative Subnormal (Negative Epsilon)
        [InlineData(-0.0f, 10_000_000)]                     // Negative Zero
        [InlineData(float.NaN, 10_000_000)]                 // NaN
        [InlineData(0.0f, 10_000_000)]                      // Positive Zero
        [InlineData(float.Epsilon, 100_000)]                // Min Positive Subnormal (Positive Epsilon)
        [InlineData(1.17549421E-38f, 100_000)]              // Max Positive Subnormal
        // [InlineData(1.17549435E-38f, 100_000)]              // Min Positive Normal
        [InlineData(1.0f, 1_000_000)]                       // Positive One
        [InlineData(2.71828183f, 1_000_000)]                // Positive e
        [InlineData(3.14159265f, 1_000_000)]                // Positive pi
        [InlineData(float.MaxValue, 100_000)]               // Max Positive Normal
        [InlineData(float.PositiveInfinity, 10_000_000)]    // Positive Infinity
        private static void FormatterSingle(float value, int innerIterations)
        {
            byte[] utf8ByteArray = new byte[40];
            Span<byte> utf8ByteSpan = utf8ByteArray;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < innerIterations; i++)
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
