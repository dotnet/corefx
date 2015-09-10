﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public class Perf_DateTime
    {
        [Benchmark]
        public void GetNow()
        {
            DateTime dt;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    dt = DateTime.Now;
        }

        [Benchmark]
        public void ToString_str()
        {
            DateTime dt = DateTime.Now;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    dt.ToString("g");
        }

        [Benchmark]
        public void op_Subtraction()
        {
            TimeSpan result;
            DateTime date1 = new DateTime(1996, 6, 3, 22, 15, 0);
            DateTime date2 = new DateTime(1996, 12, 6, 13, 2, 0);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    result = date1 - date2;
        }
    }
}
