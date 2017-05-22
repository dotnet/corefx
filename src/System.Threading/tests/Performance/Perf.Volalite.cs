// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Threading.Tests
{
    public class Perf_Volatile
    {
        [Benchmark(InnerIterationCount = 500)]
        public void ReadWrite_double()
        {
            double location = 0;
            double newValue = 1;

            Benchmark.Iterate(() =>
            {
                Volatile.Read(ref location);
                Volatile.Write(ref location, newValue);
            });
        }
    }
}
