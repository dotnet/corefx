// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System
{
    public class Perf_Math
    {
        [Benchmark]
        public void Floor_Decimal()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 3000000; i++)
                    {
                        Math.Floor(45.2m);
                    }
                }
            }
        }

        [Benchmark]
        public void Round_DoubleIntMidpointRounding()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 9000000; i++)
                    {
                        Math.Round(45.225595d, 2, MidpointRounding.ToEven);
                    }
                }
            }
        }

        [Benchmark]
        public void Round_DecimalIntMidpointRounding()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 5000000; i++)
                    {
                        Math.Round(45.225595m, 2, MidpointRounding.ToEven);
                    }
                }
            }
        }

        [Benchmark]
        public void Truncate_Decimal()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 5000000; i++)
                    {
                        Math.Truncate(45.225595m);
                    }
                }
            }
        }

        [Benchmark]
        public void IEEERemainder_DoubleDouble()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 5000000; i++)
                    {
                        Math.IEEERemainder(6, 4);
                    }
                }
            }
        }

        [Benchmark]
        public void Log_DoubleDouble()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 6000000; i++)
                    {
                        Math.Log(10, 5);
                    }
                }
            }
        }
    }
}
