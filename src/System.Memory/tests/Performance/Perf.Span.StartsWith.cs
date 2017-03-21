// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_StartsWith
    {
        [Benchmark]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        [InlineData(5, 1)]
        [InlineData(6, 1)]
        [InlineData(7, 1)]
        [InlineData(8, 1)]
        [InlineData(16, 1)]
        [InlineData(16, 10)]
        [InlineData(32, 1)]
        [InlineData(32, 10)]
        [InlineData(64, 1)]
        [InlineData(64, 10)]
        [InlineData(64, 50)]
        [InlineData(100, 1)]
        [InlineData(100, 10)]
        [InlineData(100, 50)]
        [InlineData(100, 100)]
        [InlineData(1000, 1)]
        [InlineData(1000, 10)]
        [InlineData(1000, 50)]
        [InlineData(1000, 100)]
        [InlineData(10000, 1)]
        [InlineData(10000, 10)]
        [InlineData(10000, 50)]
        [InlineData(10000, 100)]
        [InlineData(100000, 1)]
        [InlineData(100000, 10)]
        [InlineData(100000, 50)]
        [InlineData(100000, 100)]
        public void String(int size, int valSize)
        {
            var a = new int[size];
            for (int i = 0; i < size; i++)
            {
                int num = 65 + i % 26;
                /*char c = Convert.ToChar(num);
                char[] chars = {c};*/
                a[i] = num;
            }

            var b = new int[valSize];
            for (int i = 0; i < valSize; i++)
            {
                int num = 65 + i % 26;
                /*char c = Convert.ToChar(num);
                char[] chars = { c };*/
                b[i] = num;
            }
            var span = new Span<int>(a);
            var value = new Span<int>(b);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        bool result = span.StartsWith(value);
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(4, 1)]
        [InlineData(5, 1)]
        [InlineData(6, 1)]
        [InlineData(7, 1)]
        [InlineData(8, 1)]
        [InlineData(16, 1)]
        [InlineData(16, 10)]
        [InlineData(32, 1)]
        [InlineData(32, 10)]
        [InlineData(64, 1)]
        [InlineData(64, 10)]
        [InlineData(64, 50)]
        [InlineData(100, 1)]
        [InlineData(100, 10)]
        [InlineData(100, 50)]
        [InlineData(100, 100)]
        [InlineData(1000, 1)]
        [InlineData(1000, 10)]
        [InlineData(1000, 50)]
        [InlineData(1000, 100)]
        [InlineData(10000, 1)]
        [InlineData(10000, 10)]
        [InlineData(10000, 50)]
        [InlineData(10000, 100)]
        [InlineData(100000, 1)]
        [InlineData(100000, 10)]
        [InlineData(100000, 50)]
        [InlineData(100000, 100)]
        public void Byte(int size, int valSize)
        {
            var a = new byte[size];
            for (int i = 0; i < size; i++)
            {
                int num = 65 + i % 26;
                a[i] = (byte)num;
            }

            var b = new byte[valSize];
            for (int i = 0; i < valSize; i++)
            {
                int num = 65 + i % 26;
                b[i] = (byte)num;
            }
            var span = new Span<byte>(a);
            var value = new Span<byte>(b);
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        bool result = span.StartsWith(value);
                    }
                }
            }
        }
    }
}
