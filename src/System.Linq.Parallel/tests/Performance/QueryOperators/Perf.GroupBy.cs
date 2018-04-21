// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xunit.Performance;

namespace System.Linq.Parallel.Tests
{
    public sealed class GroupByPerfTestsUnordered : GroupByPerfTests
    {
    }

    public sealed class GroupByPerfTestsOrdered : GroupByPerfTests
    {
        protected override ParallelQuery<int> CreateQueryBase(int count) => base.CreateQueryBase(count).AsOrdered();
    }

    public abstract class GroupByPerfTests
    {
        const int TotalElementCount = 50_000;
        const int CrossProductInnerIterationCount = 100;

        protected virtual ParallelQuery<int> CreateQueryBase(int count) => UnorderedSources.Default(count);

        private ParallelQuery<KeyValuePair<int, int>> CreateQuery(int groupCount, int elementsPerGroup)
        {
            return CreateQueryBase(groupCount * elementsPerGroup).GroupBy(x => x % groupCount,
                (k, v) => KeyValuePair.Create(k, v.Sum()));
        }

        [Benchmark(InnerIterationCount = 1_000_000), MeasureGCAllocations]
        public void QueryCreation()
        {
            QueryCreation(10);
        }

        [Benchmark(InnerIterationCount = CrossProductInnerIterationCount), MeasureGCAllocations]
        public void CrossProduct__10()
        {
            CrossProduct(10);
        }

        [Benchmark(InnerIterationCount = CrossProductInnerIterationCount), MeasureGCAllocations]
        public void CrossProduct__25()
        {
            CrossProduct(25);
        }

        [Benchmark(InnerIterationCount = CrossProductInnerIterationCount), MeasureGCAllocations]
        public void CrossProduct__50()
        {
            CrossProduct(50);
        }

        [Benchmark(InnerIterationCount = CrossProductInnerIterationCount), MeasureGCAllocations]
        public void CrossProduct_100()
        {
            CrossProduct(100);
        }

        [Benchmark(InnerIterationCount = CrossProductInnerIterationCount), MeasureGCAllocations]
        public void CrossProduct_500()
        {
            CrossProduct(500);
        }

        public void QueryCreation(int elementsPerGroup)
        {
            Debug.Assert(TotalElementCount % elementsPerGroup == 0);
            QueryCreation(TotalElementCount / elementsPerGroup, elementsPerGroup);
        }

        private static volatile ParallelQuery<KeyValuePair<int, int>> _queryCreationResult;

        public void QueryCreation(int groupCount, int elementsPerGroup)
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                long iters = Benchmark.InnerIterationCount;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < iters; i++)
                    {
                        _queryCreationResult = CreateQuery(groupCount, elementsPerGroup);
                    }
                }
            }
        }

        public void CrossProduct(int elementsPerGroup)
        {
            Debug.Assert(TotalElementCount % elementsPerGroup == 0);
            CrossProduct(TotalElementCount / elementsPerGroup, elementsPerGroup);
        }

        private static volatile int _crossProductResult;

        public void CrossProduct(int groupCount, int elementsPerGroup)
        {
            ParallelQuery<KeyValuePair<int, int>> values = CreateQuery(groupCount, elementsPerGroup);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                long iters = Benchmark.InnerIterationCount;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < iters; i++)
                    {
                        _crossProductResult = 0;
                        foreach (KeyValuePair<int, int> pair in values)
                        {
                            _crossProductResult += pair.Key * pair.Value;
                        }
                    }
                }
            }
        }
    }
}
