// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Microsoft.Xunit.Performance;

namespace System.Tests
{
    public class Perf_Random
    {
        private volatile Random random;
        private volatile int i;
        private double d;

        [Benchmark(InnerIterationCount = 30000)]
        public void ctor()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        random = new Random(); random = new Random(); random = new Random();
                        random = new Random(); random = new Random(); random = new Random();
                        random = new Random(); random = new Random(); random = new Random();
                    }
        }

        [Benchmark(InnerIterationCount = 5000000)]
        public void Next_int()
        {
            Random rand = new Random(123456);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        i = rand.Next(10000); i = rand.Next(10000); i = rand.Next(10000);
                        i = rand.Next(10000); i = rand.Next(10000); i = rand.Next(10000);
                        i = rand.Next(10000); i = rand.Next(10000); i = rand.Next(10000);
                    }
        }

        [Benchmark(InnerIterationCount = 5000000)]
        public void Next_int_int()
        {
            Random rand = new Random(123456);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        i = rand.Next(100, 10000); i = rand.Next(100, 10000); i = rand.Next(100, 10000);
                        i = rand.Next(100, 10000); i = rand.Next(100, 10000); i = rand.Next(100, 10000);
                        i = rand.Next(100, 10000); i = rand.Next(100, 10000); i = rand.Next(100, 10000);
                    }
        }

        [Benchmark(InnerIterationCount = 5000)]
        public void NextBytes()
        {
            Random rand = new Random(123456);
            byte[] b1 = new byte[1000];
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        rand.NextBytes(b1); rand.NextBytes(b1); rand.NextBytes(b1);
                        rand.NextBytes(b1); rand.NextBytes(b1); rand.NextBytes(b1);
                        rand.NextBytes(b1); rand.NextBytes(b1); rand.NextBytes(b1);
                    }
        }

        [Benchmark(InnerIterationCount = 5000000)]
        public void NextDouble()
        {
            Random rand = new Random(123456);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        d = rand.NextDouble(); d = rand.NextDouble(); d = rand.NextDouble();
                        d = rand.NextDouble(); d = rand.NextDouble(); d = rand.NextDouble();
                        d = rand.NextDouble(); d = rand.NextDouble(); d = rand.NextDouble();
                    }
        }
    }
}
