// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Diagnostics.ProcessTests
{
    public class Perf_ProcessStartInfo : ProcessTestBase
    {
        [Benchmark]
        public void SetArguments()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Setup
                ProcessStartInfo info = new ProcessStartInfo();

                // Actual perf testing
                using (iteration.StartMeasurement())
                    info.Arguments = "args";
            }
        }

        [Benchmark]
        public void SetFileName()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                // Setup
                ProcessStartInfo info = new ProcessStartInfo();
                string currentFileName;
                using (var proc = Process.GetCurrentProcess())
                    currentFileName = proc.StartInfo.FileName;

                // Actual perf testing
                using (iteration.StartMeasurement())
                    info.FileName = currentFileName;
            }
        }

        [Benchmark]
        public void SetRedirectStandardOutput()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                using (iteration.StartMeasurement())
                    info.RedirectStandardOutput = true;
            }
        }

        [Benchmark]
        public void SetUseShellExecute()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                ProcessStartInfo info = new ProcessStartInfo();
                using (iteration.StartMeasurement())
                    info.UseShellExecute = true;
            }
        }
    }
}
