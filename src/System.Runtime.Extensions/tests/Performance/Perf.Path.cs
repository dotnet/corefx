// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Microsoft.Xunit.Performance;

namespace System.Runtime.Extensions.Tests
{
    public class Perf_Path : PerfTestBase
    {
        [Benchmark]
        public void Combine()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Setup
                string testPath1 = GetTestFilePath();
                string testPath2 = Guid.NewGuid().ToString();

                // Actual perf testing
                using (iteration.StartMeasurement())
                    Path.Combine(testPath1, testPath2);
            }
        }

        [Benchmark]
        public void GetFileName()
        {
            string testPath = GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    Path.GetFileName(testPath);
        }

        [Benchmark]
        public void GetDirectoryName()
        {
            string testPath = GetTestFilePath();
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    Path.GetDirectoryName(testPath);
        }
    }
}
