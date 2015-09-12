// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;

namespace System.IO.FileSystem.Tests
{
    public class Perf_File : PerfTestBase
    {
        [Benchmark]
        public void Exists()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Setup
                string testFile = GetTestFilePath();
                File.Create(testFile);

                // Actual perf testing
                using (iteration.StartMeasurement())
                    File.Exists(testFile);

                // Teardown
                File.Delete(testFile);
            }
        }

        [Benchmark]
        public void Delete()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Setup
                string testFile = GetTestFilePath();
                File.Create(testFile);

                // Actual perf testing
                using (iteration.StartMeasurement())
                    File.Delete(testFile);
            }
        }
    }
}
