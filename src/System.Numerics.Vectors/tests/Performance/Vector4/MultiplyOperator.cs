﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Numerics.Tests
{
    public static partial class Perf_Vector4
    {
        [Benchmark(InnerIterationCount = VectorTests.DefaultInnerIterationsCount)]
        public static void MultiplyOperatorBenchmark()
        {
            var expectedResult = Vector4.Zero;

            foreach (var iteration in Benchmark.Iterations)
            {
                Vector4 actualResult;

                using (iteration.StartMeasurement())
                {
                    actualResult = MultiplyOperatorTest();
                }

                VectorTests.AssertEqual(expectedResult, actualResult);
            }
        }

        public static Vector4 MultiplyOperatorTest()
        {
            var result = VectorTests.Vector4Value;

            for (var iteration = 0; iteration < Benchmark.InnerIterationCount; iteration++)
            {
                result *= VectorTests.Vector4Delta;
            }

            return result;
        }
    }
}
