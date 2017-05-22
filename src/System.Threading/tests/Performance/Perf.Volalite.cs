// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Threading.Tests
{
    public class Perf_Volatile
    {
        [Benchmark]
        public void Read_double()
        {
            double location = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Volatile.Read(ref location);
                }
            }
        }

        [Benchmark]
        public void Write_double()
        {
            double location = 0;
            double newValue = 1;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Volatile.Write(ref location, newValue);
                }
            }
        }
    }
}
