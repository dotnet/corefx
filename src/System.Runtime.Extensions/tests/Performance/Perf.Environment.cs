// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;
using System.Runtime.InteropServices;

namespace System.Runtime.Extensions.Tests
{
    public class Perf_Environment : PerfTestBase
    {
        [Benchmark]
        public void GetEnvironmentVariable()
        {
            string env = CreateString(15);
            try
            {
                // setup the environment variable so we can read it
                Environment.SetEnvironmentVariable(env, "value");

                // read the valid environment variable for the test
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                        Environment.GetEnvironmentVariable(env);
            }
            finally
            {
                // clear the variable that we set
                Environment.SetEnvironmentVariable(env, null);
            }
        }

        [Benchmark]
        public void GetNewLine()
        {
            string line;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    line = Environment.NewLine;
        }

        [Benchmark]
        public void ExpandEnvironmentVariables()
        {
            string env = CreateString(15);
            try
            {
                // setup the environment variable so we can read it
                Environment.SetEnvironmentVariable(env, "value");

                // read the valid environment variable
                foreach (var iteration in Benchmark.Iterations)
                    using (iteration.StartMeasurement())
                        Environment.ExpandEnvironmentVariables("%" + env + "%");
            }
            finally
            {
                // clear the variable that we set
                Environment.SetEnvironmentVariable(env, null);
            }
        }
    }
}
