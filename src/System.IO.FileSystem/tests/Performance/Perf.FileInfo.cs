// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;

namespace System.IO.FileSystem.Tests
{
    public class Perf_FileInfo : PerfTestBase
    {
        [Benchmark]
        public void ctor_str()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new FileInfo(GetTestFilePath());
        }
    }
}
