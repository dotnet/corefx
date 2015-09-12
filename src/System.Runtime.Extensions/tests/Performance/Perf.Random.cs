// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Xunit.Performance;

namespace System.Runtime.Extensions.Tests
{
    public class Perf_Random : PerfTestBase
    {
        [Benchmark]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new Random();
        }

        [Benchmark]
        public void Next_int()
        {
            Random rand = new Random();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    rand.Next(10000);
        }

        [Benchmark]
        public void Next_int_int()
        {
            Random rand = new Random();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    rand.Next(100, 10000);
        }
    }
}
