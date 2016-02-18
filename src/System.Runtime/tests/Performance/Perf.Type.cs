// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;

namespace System.Runtime.Tests
{
    public static class Perf_Type
    {
        [Benchmark]
        public static void GetTypeFromHandle()
        {
            RuntimeTypeHandle typeHandle = typeof(int).TypeHandle;
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        Type.GetTypeFromHandle(typeHandle); Type.GetTypeFromHandle(typeHandle); Type.GetTypeFromHandle(typeHandle);
                        Type.GetTypeFromHandle(typeHandle); Type.GetTypeFromHandle(typeHandle); Type.GetTypeFromHandle(typeHandle);
                        Type.GetTypeFromHandle(typeHandle); Type.GetTypeFromHandle(typeHandle); Type.GetTypeFromHandle(typeHandle);
                    }
                }
            }
        }

        [Benchmark]
        public static void Operator_Equality()
        {
            bool result;
            Type type1 = typeof(int);
            Type tyupe2 = typeof(string);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        result = type1 == tyupe2; result = type1 == tyupe2; result = type1 == tyupe2;
                        result = type1 == tyupe2; result = type1 == tyupe2; result = type1 == tyupe2;
                        result = type1 == tyupe2; result = type1 == tyupe2; result = type1 == tyupe2;
                    }
                }
            }
        }
    }
}
