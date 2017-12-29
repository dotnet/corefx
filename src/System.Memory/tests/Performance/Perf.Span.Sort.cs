// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_Sort
    {
        [Benchmark()]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void ArraySort_Int_Random(int size)
        {
            BenchmarkAndAssertArray(size);
        }
        
        [Benchmark()]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public void SpanSort_Int_Random(int size)
        {
            BenchmarkAndAssertSpan(size, i => i);
        }

        private static void BenchmarkAndAssertArray(int size)
        {
            BenchmarkAndAssertArray(size, i => i);
        }

        //private static void BenchmarkAndAssertArray(int size, string value, int expectedIndex)
        //{
        //    BenchmarkAndAssertArray(size, i => i.ToString(NumberFormat), value, expectedIndex);
        //}

        const int Seed = 213718398;
        private static void BenchmarkAndAssertArray<T>(int size, Func<int, T> toValue)
            where T : IComparable<T>
        {
            var random = new Random(Seed);
            var array = new T[size];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = toValue(random.Next());
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Array.Sort(array);
                }
            }
        }

        private static void BenchmarkAndAssertSpan(int size)
        {
            BenchmarkAndAssertSpan(size, i => i);
        }

        //private static void BenchmarkAndAssertSpan(int size, string value, int expectedIndex)
        //{
        //    BenchmarkAndAssertSpan(size, i => i.ToString(NumberFormat), value, expectedIndex);
        //}

        private static void BenchmarkAndAssertSpan<T>(int size, Func<int, T> toValue)
            where T : IComparable<T>
        {
            var random = new Random(Seed);
            Span<T> span = new T[size];
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = toValue(random.Next());
            }

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        span.Sort();
                    }
                }
            }
        }
    }
}
