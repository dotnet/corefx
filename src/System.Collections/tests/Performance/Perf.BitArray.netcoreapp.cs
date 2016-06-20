// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Collections.Tests
{
    public static partial class Perf_BitArray
    {
        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void LeftShift(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        BitArray copy = (BitArray)source.Clone();
                        BitArray result = copy.LeftShift(size / 2);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void RightShift(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        BitArray copy = (BitArray)source.Clone();
                        BitArray result = copy.RightShift(size / 2);
                    }
        }
    }
}
