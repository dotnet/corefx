// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public static class Perf_DateTime
    {
        [Benchmark]
        public static void GetNow()
        {
            DateTime dateTime;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        dateTime = DateTime.Now; dateTime = DateTime.Now; dateTime = DateTime.Now;
                        dateTime = DateTime.Now; dateTime = DateTime.Now; dateTime = DateTime.Now;
                        dateTime = DateTime.Now; dateTime = DateTime.Now; dateTime = DateTime.Now;
                    }
                }
            }
        }

        [Benchmark]
        public static void GetUtcNow()
        {
            DateTime dateTime;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        dateTime = DateTime.UtcNow; dateTime = DateTime.UtcNow; dateTime = DateTime.UtcNow;
                        dateTime = DateTime.UtcNow; dateTime = DateTime.UtcNow; dateTime = DateTime.UtcNow;
                        dateTime = DateTime.UtcNow; dateTime = DateTime.UtcNow; dateTime = DateTime.UtcNow;
                    }
                }
            }
        }

        [Benchmark]
        public static void ToString_String()
        {
            DateTime dateTime = DateTime.Now;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        dateTime.ToString("g"); dateTime.ToString("g"); dateTime.ToString("g");
                        dateTime.ToString("g"); dateTime.ToString("g"); dateTime.ToString("g");
                        dateTime.ToString("g"); dateTime.ToString("g"); dateTime.ToString("g");
                    }
                }
            }
        }

        [Benchmark]
        public static void Operation_Subtract()
        {
            TimeSpan result;
            var dateTime1 = new DateTime(1996, 6, 3, 22, 15, 0);
            var dateTime2 = new DateTime(1996, 12, 6, 13, 2, 0);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        result = dateTime1 - dateTime2; result = dateTime1 - dateTime2; result = dateTime1 - dateTime2;
                        result = dateTime1 - dateTime2; result = dateTime1 - dateTime2; result = dateTime1 - dateTime2;
                        result = dateTime1 - dateTime2; result = dateTime1 - dateTime2; result = dateTime1 - dateTime2;
                    }
                }
            }
        }
    }
}
