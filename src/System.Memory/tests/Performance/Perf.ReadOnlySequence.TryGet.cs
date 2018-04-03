// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Memory.Tests;
using System.MemoryTests;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Tests
{
    public class Rerf_ReadOnlySequence_TryGet
    {
        private const int InnerCount = 1000;
        volatile static int _volatileInt = 0;

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Byte_Array(int size, int start, int lenght)
        {
            var buffer = new ReadOnlySequence<byte>(new byte[size], start, lenght);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<byte> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Byte_Memory(int size, int start, int lenght)
        {
            var manager = new CustomMemoryForTest<byte>(new byte[size], start, lenght);
            var buffer = new ReadOnlySequence<byte>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<byte> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Byte_SingleSegment(int size, int start, int lenght)
        {
            var segment1 = new BufferSegment<byte>(new byte[size]);
            var buffer = new ReadOnlySequence<byte>(segment1, start, segment1, start + lenght);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<byte> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Byte_MultiSegment(int size, int start, int lenght)
        {
            var segment1 = new BufferSegment<byte>(new byte[size]);
            BufferSegment<byte> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new byte[size]);
            var buffer = new ReadOnlySequence<byte>(segment1, start, segment2, lenght);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var p = buffer.Start;
                        while (buffer.TryGet(ref p, out ReadOnlyMemory<byte> memory))
                            localInt ^= memory.Length;
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
                        for (int j = 0; j < Benchmark.InnerIterationCount; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<byte> memory))
                                localInt ^= memory.Length;
                        }
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
                        for (int j = 0; j < Benchmark.InnerIterationCount; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<byte> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Char_Array(int size, int start, int lenght)
        {
            var buffer = new ReadOnlySequence<char>(new char[size], start, lenght);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Char_Memory(int size, int start, int lenght)
        {
            var manager = new CustomMemoryForTest<char>(new char[size], start, lenght);
            var buffer = new ReadOnlySequence<char>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Char_SingleSegment(int size, int start, int lenght)
        {
            var segment1 = new BufferSegment<char>(new char[size]);
            var buffer = new ReadOnlySequence<char>(segment1, start, segment1, start + lenght);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void Char_MultiSegment(int size, int start, int lenght)
        {
            var segment1 = new BufferSegment<char>(new char[size]);
            BufferSegment<char> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new char[size]);
            var buffer = new ReadOnlySequence<char>(segment1, start, segment2, lenght);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var p = buffer.Start;
                        while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                            localInt ^= memory.Length;
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(1000, 0, 1000)]
        [InlineData(1000, 100, 800)]
        [InlineData(1000 * 1000, 0, 1000 * 1000)]
        [InlineData(1000 * 1000, 1000, 998 * 1000)]
        private static void String(int size, int start, int lenght)
        {
            ReadOnlyMemory<char> strMemory = new string('a', size).AsMemory();
            strMemory = strMemory.Slice(start, lenght);
            var buffer = new ReadOnlySequence<char>(strMemory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        for (int j = 0; j < size / 10; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                                localInt ^= memory.Length;
                        }
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
                        for (int j = 0; j < Benchmark.InnerIterationCount; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                                localInt ^= memory.Length;
                        }
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
                        for (int j = 0; j < Benchmark.InnerIterationCount; j++)
                        {
                            var p = buffer.Start;
                            while (buffer.TryGet(ref p, out ReadOnlyMemory<char> memory))
                                localInt ^= memory.Length;
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

    }
}
