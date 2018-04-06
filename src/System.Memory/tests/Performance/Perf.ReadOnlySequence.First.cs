// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Memory.Tests;
using System.MemoryTests;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Tests
{
    public class Perf_ReadOnlySequence_First
    {
        private const int InnerCount = 100_000;
        volatile static int _volatileInt = 0;

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_Array(int size, int offset)
        {
            var buffer = new ReadOnlySequence<byte>(new byte[size], offset, size - 2 * offset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<byte> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_Memory(int size, int offset)
        {
            var manager = new CustomMemoryForTest<byte>(new byte[size], offset, size - 2 * offset);
            var buffer = new ReadOnlySequence<byte>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<byte> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_SingleSegment(int size, int offset)
        {
            var segment1 = new BufferSegment<byte>(new byte[size]);
            var buffer = new ReadOnlySequence<byte>(segment1, offset, segment1, size - offset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<byte> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_MultiSegment(int size, int offset)
        {
            var segment1 = new BufferSegment<byte>(new byte[size / 10]);
            BufferSegment<byte> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new byte[size / 10]);
            var buffer = new ReadOnlySequence<byte>(segment1, offset, segment2, size / 10 - offset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<byte> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Byte_Empty()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<byte> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Byte_Default()
        {
            ReadOnlySequence<byte> buffer = default;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<byte> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_Array(int size, int offset)
        {
            var buffer = new ReadOnlySequence<char>(new char[size], offset, size - 2 * offset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_Memory(int size, int offset)
        {
            var manager = new CustomMemoryForTest<char>(new char[size], offset, size - 2 * offset);
            var buffer = new ReadOnlySequence<char>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_SingleSegment(int size, int offset)
        {
            var segment1 = new BufferSegment<char>(new char[size]);
            var buffer = new ReadOnlySequence<char>(segment1, offset, segment1, size - offset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_MultiSegment(int size, int offset)
        {
            var segment1 = new BufferSegment<char>(new char[size / 10]);
            BufferSegment<char> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new char[size / 10]);

            var buffer = new ReadOnlySequence<char>(segment1, offset, segment2, size / 10 - offset); 

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void String(int size, int offset)
        {
            ReadOnlyMemory<char> memory = new string('a', size).AsMemory();
            memory = memory.Slice(offset, size - 2 * offset);
            var buffer = new ReadOnlySequence<char>(memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Char_Empty()
        {
            ReadOnlySequence<char> buffer = ReadOnlySequence<char>.Empty;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Char_Default()
        {
            ReadOnlySequence<char> buffer = default;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        ReadOnlyMemory<char> first = buffer.First;
                        localInt ^= first.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

    }
}
