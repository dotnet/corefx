// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.Xunit.Performance;
using Xunit.NetCore.Extensions;

namespace System.Drawing.Tests
{
    public class Perf_FontFamily_Name
    {
        [Benchmark(InnerIterationCount = 10000)]
        [ConditionalBenchmark(typeof(Helpers), nameof(Helpers.GetGdiplusIsAvailable))]
        public void FontFamily_Name()
        {
            using (FontFamily family = FontFamily.GenericSerif)
            {
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                        {
                            var str = family.Name;
                        }
                    }
                }
            }
        }
    }
}
