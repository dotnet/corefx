// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public class Perf_IntPtr
    {
        [Benchmark]
        public void GetZero()
        {
            IntPtr ptr;
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    ptr = IntPtr.Zero;
        }

        [Benchmark]
        public void ctor_int32()
        {
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    new IntPtr(0);
        }

        [Benchmark]
        public void op_Equality_IntPtr_IntPtr()
        {
            bool res;
            IntPtr ptr1 = new IntPtr(0);
            IntPtr ptr2 = new IntPtr(0);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    res = ptr1 == ptr2;
        }
    }
}
