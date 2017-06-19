﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Threading.Tests
{
    public class Perf_Monitor
    {
        [Benchmark(InnerIterationCount = 250)]
        public static void EnterExit()
        {
            object sync = new object();

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Monitor.Enter(sync);
                        Monitor.Exit(sync);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = 100)]
        public static void TryEnterExit()
        {
            object sync = new object();

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        Monitor.TryEnter(sync, 0);
                        Monitor.TryEnter(sync, 0);
                        Monitor.Exit(sync);
                    }
                }
            }
        }
    }
}
