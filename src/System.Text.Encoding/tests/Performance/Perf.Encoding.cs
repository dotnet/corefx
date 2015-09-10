// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Xunit;
using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public class Perf_Encoding : PerfTestBase
    {
        [Benchmark]
        [InlineData(40)]
        [InlineData(160)]
        public void GetBytes_str(int size)
        {
            Encoding enc = Encoding.UTF8;
            string toEncode = CreateString(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    enc.GetBytes(toEncode);
        }
    }
}
