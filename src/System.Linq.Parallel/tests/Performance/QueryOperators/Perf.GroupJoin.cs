// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Xunit.Performance;

namespace System.Linq.Parallel.Tests
{
    public sealed class GroupJoinPerfTestsUnorderedLeftUnorderedRight : GroupJoinPerfTests
    {
    }

    public sealed class GroupJoinPerfTestsUnorderedLeftOrderedRight : GroupJoinPerfTests
    {
        protected override ParallelQuery<int> Right => base.Right.AsOrdered();
    }

    public sealed class GroupJoinPerfTestsOrderedLeftUnorderedRight : GroupJoinPerfTests
    {
        protected override ParallelQuery<int> Left => base.Left.AsOrdered();
    }

    public sealed class GroupJoinPerfTestsOrderedLeftOrderedRight : GroupJoinPerfTests
    {
        protected override ParallelQuery<int> Left => base.Left.AsOrdered();
        protected override ParallelQuery<int> Right => base.Right.AsOrdered();
    }

    public abstract class GroupJoinPerfTests
    {
        const int LeftCount = 100;
        const int RightsPerLeft = 20;
        protected virtual ParallelQuery<int> Left => UnorderedSources.Default(LeftCount);
        protected virtual ParallelQuery<int> Right => UnorderedSources.Default(LeftCount * RightsPerLeft);

        private ParallelQuery<KeyValuePair<int, int>> CreateQuery()
        {
            return Left.GroupJoin(Right, x => x, y => y / LeftCount, (x, y) => KeyValuePair.Create(x, y.Sum()));
        }

        private static volatile ParallelQuery<KeyValuePair<int, int>> _queryCreationResult;

        [Benchmark(InnerIterationCount = 1_000_000), MeasureGCAllocations]
        public void QueryCreation()
        {
            ParallelQuery<KeyValuePair<int, int>> values = CreateQuery();

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
