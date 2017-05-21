// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Threading.Tests
{
    public class Perf_Interlocked
    {
        [Benchmark]
        public static void IncrementDecrement_int()
        {
            int integer32 = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.Increment(ref integer32);
                    Interlocked.Decrement(ref integer32);
                }
            }
        }

        [Benchmark]
        public static void IncrementDecrement_long()
        {
            long integer64 = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.Increment(ref integer64);
                    Interlocked.Decrement(ref integer64);
                }
            }
        }

        [Benchmark]
        public static void Add_int()
        {
            int integer32 = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.Add(ref integer32, 2);
                    Interlocked.Add(ref integer32, -2);
                }
            }
        }

        [Benchmark]
        public static void Add_long()
        {
            long integer64 = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.Add(ref integer64, 2);
                    Interlocked.Add(ref integer64, -2);
                }
            }
        }

        [Benchmark]
        public static void Read()
        {
            long location = 0;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.Read(ref location);
                }
            }
        }

        [Benchmark]
        public static void Exchange()
        {
            long location = 0;
            long newValue = 1;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.Exchange(ref location, newValue);
                }
            }
        }

        [Benchmark]
        public static void CompareExchange_int()
        {
            int location = 0;
            int newValue = 1;
            int comparand = 0;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.CompareExchange(ref location, newValue, comparand);
                }
            }
        }

        [Benchmark]
        public static void CompareExchange_long()
        {
            long location = 0;
            long newValue = 1;
            long comparand = 0;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    Interlocked.CompareExchange(ref location, newValue, comparand);
                }
            }
        }
    }
}
