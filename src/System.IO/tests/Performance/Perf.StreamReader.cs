// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;

namespace System.IO.Tests
{
    public class Perf_StreamReader : PerfTestBase
    {
        [Benchmark]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new StreamReader();
        }
    }
}
