// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public static class Perf_UInt32
    {
        [Benchmark]
        public static void ToString_Performance()
        {
            var ui32 = new uint();
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        ui32.ToString(); ui32.ToString(); ui32.ToString();
                        ui32.ToString(); ui32.ToString(); ui32.ToString();
                        ui32.ToString(); ui32.ToString(); ui32.ToString();
                    }
                }
            }
        }
    }
}
