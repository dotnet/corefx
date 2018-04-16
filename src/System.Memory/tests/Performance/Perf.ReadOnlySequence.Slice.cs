// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Memory.Tests;
using System.MemoryTests;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Tests
{
    public abstract class Perf_ReadOnlySequence_Slice_Byte: Perf_ReadOnlySequence_Slice<byte>
    {
        public Perf_ReadOnlySequence_Slice_Byte(ReadOnlySequenceFactory<byte> factory): base(factory) { }

        public class Array: Perf_ReadOnlySequence_Slice_Byte
        {
            public Array(): base(ReadOnlySequenceFactory<byte>.ArrayFactory) { }
        }

        public class Memory : Perf_ReadOnlySequence_Slice_Byte
        {
            public Memory() : base(ReadOnlySequenceFactory<byte>.MemoryFactory) { }
        }

        public class SingleSegment : Perf_ReadOnlySequence_Slice_Byte
        {
            public SingleSegment() : base(ReadOnlySequenceFactory<byte>.SingleSegmentFactory) { }
        }

        public class TenSegments : Perf_ReadOnlySequence_Slice_Byte
        {
            public TenSegments() : base(new ReadOnlySequenceFactory<byte>.SegmentsTestSequenceFactory(10)) { }
        }
    }

    public abstract class Perf_ReadOnlySequence_Slice_Char: Perf_ReadOnlySequence_Slice<char>
    {
        public Perf_ReadOnlySequence_Slice_Char(ReadOnlySequenceFactory<char> factory): base(factory) { }

        public class Array: Perf_ReadOnlySequence_Slice_Char
        {
            public Array(): base(ReadOnlySequenceFactory<char>.ArrayFactory) { }
        }

        public class Memory : Perf_ReadOnlySequence_Slice_Char
        {
            public Memory() : base(ReadOnlySequenceFactory<char>.MemoryFactory) { }
        }

        public class String : Perf_ReadOnlySequence_Slice_Char
        {
            public String() : base(ReadOnlySequenceFactoryChar.StringFactory) { }
        }

        public class Segments___1 : Perf_ReadOnlySequence_Slice_Char
        {
            public Segments___1() : base(ReadOnlySequenceFactory<char>.SingleSegmentFactory) { }
        }

        public class Segments__10 : Perf_ReadOnlySequence_Slice_Byte
        {
            public Segments__10() : base(new ReadOnlySequenceFactory<byte>.SegmentsTestSequenceFactory(10)) { }
        }

        public class Segments_100 : Perf_ReadOnlySequence_Slice_Byte
        {
            public Segments_100() : base(new ReadOnlySequenceFactory<byte>.SegmentsTestSequenceFactory(100)) { }
        }

    }

    public abstract class Perf_ReadOnlySequence_Slice<T>
    {

        private const int InnerCount = 10_000;
        volatile static int _volatileInt = 0;

        internal ReadOnlySequenceFactory<T> Factory { get; }

        public Perf_ReadOnlySequence_Slice(ReadOnlySequenceFactory<T> factory)
        {
            Factory = factory;
        }

        
        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 10)]
        public void Long(int bufSize, int posCount)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(bufSize);

            List<long> positions = CookPositionsLong(buffer, posCount);
            //Console.WriteLine($"Long count {positions.Count}");
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach (long startOffset in positions)
                        {
                            ReadOnlySequence<T> sliced = buffer.Slice(startOffset);
                            localInt ^= sliced.Start.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }


        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 10)]
        public void LongLong(int bufSize, int posCount)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(bufSize);

            List<(long, long)> positions = CookPositionsLongLong(buffer, posCount);
            //Console.WriteLine($"LongLong count {positions.Count}");
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach ((long start, long length) in positions)
                        {
                            ReadOnlySequence<T> sliced = buffer.Slice(start, length);
                            localInt ^= sliced.Start.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 10)]
        public void LongPos(int bufSize, int posCount)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(bufSize);

            List<(long, SequencePosition)> positions = CookPositionsLongPos(buffer, posCount);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach ((long start, SequencePosition end) in positions)
                        {
                            ReadOnlySequence<T> sliced = buffer.Slice(start, end);
                            localInt ^= sliced.Start.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 10)]
        public void Pos(int bufSize, int posCount)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(bufSize);

            List<SequencePosition> positions = CookPositionsPos(buffer, posCount);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach (SequencePosition start in positions)
                        {
                            ReadOnlySequence<T> sliced = buffer.Slice(start);
                            localInt ^= sliced.Start.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 10)]
        public void PosLong(int bufSize, int posCount)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(bufSize);

            List<(SequencePosition, long)> positions = CookPositionsPosLong(buffer, posCount);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach ((SequencePosition start, long length) in positions)
                        {
                            ReadOnlySequence<T> sliced = buffer.Slice(start, length);
                            localInt ^= sliced.Start.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 10)]
        public void PosPos(int bufSize, int posCount)
        {
            ReadOnlySequence<T> buffer = Factory.CreateOfSize(bufSize);

            List<(SequencePosition, SequencePosition)> positions = CookPositionsPosPos(buffer, posCount);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                int localInt = 0;
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        foreach ((SequencePosition start, SequencePosition end) in positions)
                        {
                            ReadOnlySequence<T> sliced = buffer.Slice(start, end);
                            localInt ^= sliced.Start.GetInteger();
                        }
                    }
                }
                _volatileInt = localInt;
            }
        }

        #region  CookPositions

        internal static List<long> CookPositionsLong(in ReadOnlySequence<T> buffer, int count)
        {
            var result = new List<long>(count);

            long length = buffer.Length;
            long offset = length / count;

            long start = 0;
            while (length >= offset)
            {
                start += offset;
                result.Add(start);
                length -= offset;
            }

            return result;
        }

        internal static List<SequencePosition> CookPositionsPos(in ReadOnlySequence<T> buffer, int count)
        {
            var result = new List<SequencePosition>(count);

            List<long> positions = CookPositionsLong(buffer, count);
            foreach (long startOffset in positions)
            {
                SequencePosition start = buffer.GetPosition(startOffset);
                result.Add(start);
            }

            return result;
        }

        internal static List<(long, long)> CookPositionsLongLong(in ReadOnlySequence<T> buffer, int count)
        {
            var result = new List<(long, long)>(count);

            long length = buffer.Length;
            long offset = length / (2 * count);

            long start = 0, end = length;
            while (length >= 2 * offset)
            {
                start += offset;
                length -= 2 * offset;
                result.Add((start, length));
            }

            return result;
        }

        internal static List<(long, SequencePosition)> CookPositionsLongPos(in ReadOnlySequence<T> buffer, int count)
        {
            var result = new List<(long, SequencePosition)>(count);

            List<(long, long)> positions = CookPositionsLongLong(buffer, count);
            foreach ((long startOffset, long sliceLength) in positions)
            {
                SequencePosition start = buffer.GetPosition(startOffset);
                SequencePosition end = buffer.GetPosition(sliceLength, start);
                result.Add((startOffset, end));
            }

            return result;
        }

        internal static List<(SequencePosition, long)> CookPositionsPosLong(in ReadOnlySequence<T> buffer, int count)
        {
            var result = new List<(SequencePosition, long)>(count);

            List<(long, long)> positions = CookPositionsLongLong(buffer, count);
            foreach ((long startOffset, long sliceLength) in positions)
            {
                SequencePosition start = buffer.GetPosition(startOffset);
                result.Add((start, sliceLength));
            }

            return result;
        }

        internal static List<(SequencePosition, SequencePosition)> CookPositionsPosPos(in ReadOnlySequence<T> buffer, int count)
        {
            var result = new List<(SequencePosition, SequencePosition)>(count);

            List<(long, long)> positions = CookPositionsLongLong(buffer, count);
            foreach ((long startOffset, long sliceLength) in positions)
            {
                SequencePosition start = buffer.GetPosition(startOffset);
                SequencePosition end = buffer.GetPosition(sliceLength, start);
                result.Add((start, end));
            }

            return result;
        }

        #endregion
    }
}
