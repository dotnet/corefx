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
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void ArraySort_Int_Random(int size)
        {
            BenchmarkAndAssertArrayInt(size);
        }
        
        [Benchmark()]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void SpanSort_Int_Random(int size)
        {
            BenchmarkAndAssertSpan(size, i => i);
        }

        private static void BenchmarkAndAssertArrayInt(int size)
        {
            BenchmarkAndAssertArray(size, i => i);
        }

        //private static void BenchmarkAndAssertArrayString(int size)
        //{
        //    BenchmarkAndAssertArray(size, i => i.ToString(NumberFormat));
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

        private static void BenchmarkAndAssertSpanInt(int size)
        {
            BenchmarkAndAssertSpan(size, i => i);
        }

        //private static void BenchmarkAndAssertSpanString(int size)
        //{
        //    BenchmarkAndAssertSpan(size, i => i.ToString(NumberFormat));
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
                    span.Sort();
                }
            }
        }
    }
}
