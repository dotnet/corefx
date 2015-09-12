// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;
using System.Text;

namespace System.Runtime.Tests
{
    public class Perf_GC : PerfTestBase
    {
        [Benchmark]
        public void SuppressFinalize()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // obj has no managed resources to free, so suppressing the finalizer for it is fine.
                object obj = new object();

                using (iteration.StartMeasurement())
                    GC.SuppressFinalize(obj);
            }
        }
    }
}
