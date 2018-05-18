// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Memory.Tests;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Tests
{
    public class Rerf_ReadOnlySequence_GetPosition
    {
        private const int InnerCount = 10_000;
        volatile static int _volatileInt = 0;

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_Array(int bufSize, int bufOffset)
        {
            var buffer = new ReadOnlySequence<byte>(new byte[bufSize], bufOffset, bufSize - 2 * bufOffset);
            int offset = (int)buffer.Length / 10;
            SequencePosition end = buffer.GetPosition(0, buffer.End);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        SequencePosition pos = buffer.Start;
                        while (!pos.Equals(end))
                        {
                            pos = buffer.GetPosition(offset, pos);
                            localInt ^= pos.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_MultiSegment(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<byte>(new byte[bufSize / 10]);
            BufferSegment<byte> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new byte[bufSize / 10]);
            var buffer = new ReadOnlySequence<byte>(segment1, bufOffset, segment2, bufSize / 10 - bufOffset);
            int offset = (int)buffer.Length / 10;
            SequencePosition end = buffer.GetPosition(0, buffer.End);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        SequencePosition pos = buffer.Start;
                        while (!pos.Equals(end))
                        {
                            pos = buffer.GetPosition(offset, pos);
                            localInt ^= pos.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount * 10)]
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
                        SequencePosition pos = buffer.GetPosition(0);
                        localInt ^= pos.GetInteger();
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount * 10)]
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
                        SequencePosition pos = buffer.GetPosition(0);
                        localInt ^= pos.GetInteger();
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_MultiSegment(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<char>(new char[bufSize / 10]);
            BufferSegment<char> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new char[bufSize / 10]);
            var buffer = new ReadOnlySequence<char>(segment1, bufOffset, segment2, bufSize / 10 - bufOffset);
            int offset = (int)buffer.Length / 10;
            SequencePosition end = buffer.GetPosition(0, buffer.End);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        SequencePosition pos = buffer.Start;
                        while (!pos.Equals(end))
                        {
                            pos = buffer.GetPosition(offset, pos);
                            localInt ^= pos.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void String(int bufSize, int bufOffset)
        {
            ReadOnlyMemory<char> memory = new string('a', bufSize).AsMemory();
            memory = memory.Slice(bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<char>(memory);
            int offset = (int)buffer.Length / 10;
            SequencePosition end = buffer.GetPosition(0, buffer.End);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        SequencePosition pos = buffer.Start;
                        while (!pos.Equals(end))
                        {
                            pos = buffer.GetPosition(offset, pos);
                            localInt ^= pos.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount * 10)]
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
                        SequencePosition p = buffer.GetPosition(0);
                        localInt ^= p.GetInteger();
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount * 10)]
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
                        SequencePosition p = buffer.GetPosition(0);
                        localInt ^= p.GetInteger();
                    }
                }
                _volatileInt = localInt;
            }
        }

    }
}
