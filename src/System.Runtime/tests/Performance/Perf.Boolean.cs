// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public static class Perf_Boolean
    {
        [Benchmark]
        public static void Parse_String()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        bool.Parse("True"); bool.Parse("True"); bool.Parse("True");
                        bool.Parse("True"); bool.Parse("True"); bool.Parse("True");
                        bool.Parse("True"); bool.Parse("True"); bool.Parse("True");
                    }
                }
            }
        }

        [Benchmark]
        public static void ToString_Performance()
        {
            bool b = true;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        b.ToString(); b.ToString(); b.ToString();
                        b.ToString(); b.ToString(); b.ToString();
                        b.ToString(); b.ToString(); b.ToString();
                    }
                }
            }
        }
    }
}
