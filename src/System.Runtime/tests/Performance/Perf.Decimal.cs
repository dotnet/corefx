﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Tests
{
    public class Perf_Decimal
    {
        private volatile string _string;

        [Benchmark(InnerIterationCount = 4_000_000)]
        public void DefaultToString()
        {
            decimal number = new decimal(1.23456789E+5);

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        _string = number.ToString();
                    }
                }
            }
        }
    }
}
