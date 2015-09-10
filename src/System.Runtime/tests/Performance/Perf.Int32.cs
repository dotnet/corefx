// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public class Perf_Int32 : PerfTestBase
    {
        [Benchmark]
        public void ToString_()
        {
            Int32 i32 = 32;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    i32.ToString();
        }

        [Benchmark]
        [InlineData(100)]
        [InlineData(1000)]
        public void Parse_str(int length)
        {
            string builtString = CreateString(length);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    Int32.Parse(builtString);
        }
    }
}
