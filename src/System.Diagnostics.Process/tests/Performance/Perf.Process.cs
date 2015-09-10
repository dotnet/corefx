// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using Microsoft.Xunit.Performance;
using System.IO;

namespace System.Diagnostics.ProcessTests
{
    public class Perf_Process : ProcessTestBase
    {
        [Benchmark]
        public void Kill()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    proc.Kill();
        }

        [Benchmark]
        public void GetProcessesByName()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    Process.GetProcessesByName("1");
        }

        [Benchmark]
        public void GetId()
        {
            int id;
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    id = proc.Id;
        }

        [Benchmark]
        public void Create()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                Process proc;
                using (iteration.StartMeasurement())
                    proc = new Process();
                proc.Dispose();
            }
        }

        [Benchmark]
        public void Start()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    proc.Start();
        }

        [Benchmark]
        public void GetHasExited()
        {
            bool result;
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    result = proc.HasExited;
        }

        [Benchmark]
        public void GetExitCode()
        {
            int result;
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    result = proc.ExitCode;
        }

        [Benchmark]
        public void GetStartInfo()
        {
            ProcessStartInfo result;
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    result = proc.StartInfo;
        }

        [Benchmark]
        public void WaitForExit()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcess())
                using (iteration.StartMeasurement())
                    proc.WaitForExit();
        }

        [Benchmark]
        public void WaitForExit_int()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcess())
                using (iteration.StartMeasurement())
                    proc.WaitForExit(30);
        }

        [Benchmark]
        public void GetStandardOutput()
        {
            StreamReader result;
            foreach (var iteration in Benchmark.Iterations)
                using (Process proc = CreateProcessInfinite())
                using (iteration.StartMeasurement())
                    result = proc.StandardOutput;
        }
    }
}
