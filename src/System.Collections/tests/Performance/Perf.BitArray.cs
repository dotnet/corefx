// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Collections.Tests
{
    public static partial class Perf_BitArray
    {
        private readonly static Random _random = new Random(837322);

        private static int[] CreateIntArray(int size)
        {
            if (size == 0)
            {
                return new int[] { /* Deliberately Empty */ };
            }

            byte[] original = CreateByteArray(size);
            int[] data = new int[(original.Length - 3) / 4 + 1];
            for (int i = 0; i < data.Length; i++)
            {
                int orig = i * 4;
                data[i] = (original[orig + 0] << 0) | (original[orig + 1] << 8) | (original[orig + 2] << 16) | (original[orig + 3] << 24);
            }

            return data;
        }

        private static byte[] CreateByteArray(int size)
        {
            if (size == 0)
            {
                return new byte[] { /* Deliberately Empty */ };
            }

            byte[] data = new byte[size / 8];
            _random.NextBytes(data);
            return data;
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void ctor_intarray(int size)
        {
            int[] source = CreateIntArray(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        new BitArray(source);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void ctor_bytearray(int size)
        {
            byte[] source = CreateByteArray(size);
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        new BitArray(source);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void CopyTo_intarray(int size)
        {
            BitArray source = new BitArray(CreateIntArray(size));
            int[] destination = new int[size];
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        ((ICollection)source).CopyTo(destination, 0);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void CopyTo_bytearray(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            byte[] destination = new byte[size];
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        ((ICollection)source).CopyTo(destination, 0);
                    }
        }

        [Benchmark]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void Get(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        bool a = source[_random.Next(size)];
                    }
        }

        [Benchmark]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void Set(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        source[_random.Next(size)] = (_random.Next(1) == 0);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void Not(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        source = source.Not();
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void And(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            BitArray other = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        BitArray result = source.And(other);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void Or(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            BitArray other = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        BitArray result = source.Or(other);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void Length(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        BitArray copy = (BitArray)source.Clone();
                        copy.Length = (source.Length / 2);
                    }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(16384)]
        public static void Xor(int size)
        {
            BitArray source = new BitArray(CreateByteArray(size));
            BitArray other = new BitArray(CreateByteArray(size));
            foreach (var iteration in Benchmark.Iterations)
                using (iteration.StartMeasurement())
                    for (int i = 0; i <= 20000; i++)
                    {
                        BitArray result = source.Xor(other);
                    }
        }
    }
}
