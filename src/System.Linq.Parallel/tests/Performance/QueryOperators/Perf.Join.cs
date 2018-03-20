// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Xunit.Performance;

namespace System.Linq.Parallel.Tests
{
    public sealed class JoinPerfTestsUnorderedLeftUnorderedRight : JoinPerfTests
    {
    }

    public sealed class JoinPerfTestsUnorderedLeftOrderedRight : JoinPerfTests
    {
        protected override ParallelQuery<int> Right => base.Right.AsOrdered();
    }

    public sealed class JoinPerfTestsOrderedLeftUnorderedRight : JoinPerfTests
    {
        protected override ParallelQuery<int> Left => base.Left.AsOrdered();
    }

    public sealed class JoinPerfTestsOrderedLeftOrderedRight : JoinPerfTests
    {
        protected override ParallelQuery<int> Left => base.Left.AsOrdered();
        protected override ParallelQuery<int> Right => base.Right.AsOrdered();
    }

    public abstract class JoinPerfTests
    {
        const int LeftCount = 75;
        const int RightsPerLeft = 20;
        protected virtual ParallelQuery<int> Left => UnorderedSources.Default(LeftCount);
        protected virtual ParallelQuery<int> Right => UnorderedSources.Default(LeftCount * RightsPerLeft);

        private ParallelQuery<KeyValuePair<int, int>> CreateQuery()
        {
            return Left.Join(Right, x => x, y => y / RightsPerLeft, (x, y) => KeyValuePair.Create(x, y));
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
