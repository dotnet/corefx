// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Tests
{
    public class Perf_HashCode
    {
        [Benchmark]
        public void Add_()
        { 
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                {
                    var hc = new HashCode();
                    for (int i = 0; i < 10000; i++)
                    {
                        hc.Add(i); hc.Add(i); hc.Add(i);
                        hc.Add(i); hc.Add(i); hc.Add(i);
                        hc.Add(i); hc.Add(i); hc.Add(i);
                    }
                    hc.ToHashCode();
                }
        }
    }
}
