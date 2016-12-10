// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_Fill
    {
        [Benchmark]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(100000)]
        public void Int(int size)
        {
            var a = new int[size];
            var span = new Span<int>(a);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Fill(42);
                    }
                }
            }
        }
    }
}
