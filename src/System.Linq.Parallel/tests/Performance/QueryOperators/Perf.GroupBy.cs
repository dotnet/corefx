// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Xunit.Performance;

namespace System.Linq.Parallel.Tests
{
    public sealed class GroupByPerfTestsUnordered : GroupByPerfTests
    {
    }

    public sealed class GroupByPerfTestsOrdered : GroupByPerfTests
    {
        protected override ParallelQuery<int> QueryBase => base.QueryBase.AsOrdered();
    }

    public abstract class GroupByPerfTests
    {
        const int GroupCount = 75;
        const int ElementsPerGroup = 20;
        protected virtual ParallelQuery<int> QueryBase => UnorderedSources.Default(GroupCount * ElementsPerGroup);

        private ParallelQuery<KeyValuePair<int, int>> CreateQuery()
        {
            return QueryBase.GroupBy(x => x / ElementsPerGroup, (k, v) => KeyValuePair.Create(k, v.Sum()));
        }

        private static volatile ParallelQuery<KeyValuePair<int, int>> _queryCreationResult;

        [Benchmark(InnerIterationCount = 1_000_000), MeasureGCAllocations]
        public void QueryCreation()
        {
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                long iters = Benchmark.InnerIterationCount;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < iters; i++)
                    {
                        _queryCreationResult = CreateQuery();
                    }
                }
            }
        }

        private static volatile int _crossProductResult;

        [Benchmark(InnerIterationCount = 1_000), MeasureGCAllocations]
        public void CrossProduct()
        {
            ParallelQuery<KeyValuePair<int, int>> values = CreateQuery();

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
