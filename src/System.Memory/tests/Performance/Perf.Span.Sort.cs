// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_Sort
    {
        private const int InnerCountForNoSorting = 1000000;
        //private const string NumberFormat = "D9";

        [Benchmark(InnerIterationCount = InnerCountForNoSorting)]
        public void ArraySort_Int_Length_0()
        {
            int[] array = new int[0];
            BenchmarkRepeatableArray(array);
        }
        [Benchmark(InnerIterationCount = InnerCountForNoSorting)]
        public void ArraySort_Int_Length_1()
        {
            int[] array = new int[1];
            BenchmarkRepeatableArray(array);
        }
        [Benchmark()]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(10000)]
        [InlineData(1000000)]
        public void ArraySort_Int_Random(int length)
        {
            BenchmarkAndAssertArrayInt(length);
        }

        [Benchmark(InnerIterationCount = InnerCountForNoSorting)]
        public void SpanSort_Int_Length_0()
        {
            Span<int> span = new int[0];
            BenchmarkRepeatableSpan(span);
        }
        [Benchmark(InnerIterationCount = InnerCountForNoSorting)]
        public void SpanSort_Int_Length_1()
        {
            Span<int> span = new int[1];
            BenchmarkRepeatableSpan(span);
        }
        [Benchmark()]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(10000)]
        [InlineData(1000000)]
        public void SpanSort_Int_Random(int length)
        {
            BenchmarkAndAssertSpanInt(length);
        }

        private static void BenchmarkAndAssertArrayInt(int length)
        {
            BenchmarkAndAssertArray(length, i => i);
        }

        //private static void BenchmarkAndAssertArrayString(int length)
        //{
        //    BenchmarkAndAssertArray(length, i => i.ToString(NumberFormat));
        //}

        const int Seed = 213718398;
        private static void BenchmarkAndAssertArray<T>(int length, Func<int, T> toValue)
            where T : IComparable<T>
        {
            var random = CreateRandomArray(length, toValue);
            var work = new T[length];

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                Array.Copy(random, work, random.Length);

                using (iteration.StartMeasurement())
                {
                    Array.Sort(work);
                }
            }
        }

        private static void BenchmarkAndAssertSpanInt(int length)
        {
            BenchmarkAndAssertSpan(length, i => i);
        }

        //private static void BenchmarkAndAssertSpanString(int length)
        //{
        //    BenchmarkAndAssertSpan(length, i => i.ToString(NumberFormat));
        //}

        private static void BenchmarkAndAssertSpan<T>(int length, Func<int, T> toValue)
            where T : IComparable<T>
        {
            var random = CreateRandomArray(length, toValue);
            var work = new T[length];
            Span<T> spanWork = work;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                Array.Copy(random, work, random.Length);

                using (iteration.StartMeasurement())
                {
                    spanWork.Sort();
                }
            }
        }

        private static void BenchmarkRepeatableSpan(Span<int> span)
        {
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

        private static void BenchmarkRepeatableArray(int[] array)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Array.Sort(array);
                    }
                }
            }
        }

        private static T[] CreateRandomArray<T>(int length, Func<int, T> toValue)
            where T : IComparable<T>
        {
            var random = new Random(Seed);
            var array = new T[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = toValue(random.Next());
            }
            return array;
        }
    }
}
