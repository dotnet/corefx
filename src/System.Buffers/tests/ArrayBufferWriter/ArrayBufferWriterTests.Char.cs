// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Buffers.Tests
{
    public static partial class ArrayBufferWriterTests_Char
    {
        [Fact]
        public static void ArrayBufferWriter_Ctor()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                Assert.True(output.FreeCapacity > 0);
                Assert.True(output.Capacity > 0);
                Assert.Equal(0, output.WrittenCount);
                Assert.True(ReadOnlyMemory<char>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
            }

            using (var output = new ArrayBufferWriter<char>(200))
            {
                Assert.True(output.FreeCapacity >= 200);
                Assert.True(output.Capacity >= 200);
                Assert.Equal(0, output.WrittenCount);
                Assert.True(ReadOnlyMemory<char>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
            }

            using (ArrayBufferWriter<char> output = default)
            {
                Assert.Equal(null, output);
            }
        }

        [Fact]
        public static void Invalid_Ctor()
        {
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter<char>(0));
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter<char>(-1));
            Assert.Throws<OutOfMemoryException>(() => new ArrayBufferWriter<char>(int.MaxValue));
        }

        [Fact]
        public static void DoubleDispose()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                output.Dispose();
            }
        }

        [Fact]
        public static void DisposeThenAccess()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                output.Dispose();
                Assert.Throws<ObjectDisposedException>(() => output.FreeCapacity);
                Assert.Throws<ObjectDisposedException>(() => output.Capacity);
                Assert.Throws<ObjectDisposedException>(() => output.WrittenCount);
                Assert.Throws<ObjectDisposedException>(() => output.WrittenMemory);
                Assert.Throws<ObjectDisposedException>(() => output.Clear());
                Assert.Throws<ObjectDisposedException>(() => output.Advance(1));
                Assert.Throws<ObjectDisposedException>(() => output.GetMemory());
            }
        }

        [Fact]
        public static void Clear()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                int previousAvailable = output.FreeCapacity;
                WriteData(output, 2);
                Assert.True(output.FreeCapacity < previousAvailable);
                Assert.True(output.WrittenCount > 0);
                Assert.False(ReadOnlyMemory<char>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
                output.Clear();
                Assert.Equal(0, output.WrittenCount);
                Assert.True(ReadOnlyMemory<char>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
            }
        }

        [Fact]
        public static void Advance()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                int capacity = output.Capacity;
                Assert.Equal(capacity, output.FreeCapacity);
                output.Advance(output.FreeCapacity);
                Assert.Equal(capacity, output.WrittenCount);
                Assert.Equal(0, output.FreeCapacity);
            }

            using (var output = new ArrayBufferWriter<char>())
            {
                output.Advance(output.Capacity);
                Assert.Equal(output.Capacity, output.WrittenCount);
                Assert.Equal(0, output.FreeCapacity);
                int previousCapacity = output.Capacity;
                Span<char> _ = output.GetSpan();
                Assert.True(output.Capacity > previousCapacity);
            }

            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 2);
                ReadOnlySpan<char> previous = output.WrittenMemory.Span;
                output.Advance(10);
                Assert.False(previous.SequenceEqual(output.WrittenMemory.Span));
            }
        }

        [Fact]
        public static void AdvanceZero()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 2);
                Assert.Equal(2, output.WrittenCount);
                ReadOnlySpan<char> previous = output.WrittenMemory.Span;
                output.Advance(0);
                Assert.Equal(2, output.WrittenCount);
                Assert.True(previous.SequenceEqual(output.WrittenMemory.Span));
            }
        }

        [Fact]
        public static void InvalidAdvance()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                Assert.Throws<ArgumentException>(() => output.Advance(-1));
                Assert.Throws<InvalidOperationException>(() => output.Advance(output.Capacity + 1));
            }

            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 100);
                Assert.Throws<InvalidOperationException>(() => output.Advance(output.FreeCapacity + 1));
            }
        }

        public static bool IsX64 { get; } = IntPtr.Size >= 8;

        [ConditionalFact(nameof(IsX64))]
        [OuterLoop]
        public static void InvalidAdvance_Large()
        {
            try
            {
                using (var output = new ArrayBufferWriter<char>(2_000_000_000))
                {
                    WriteData(output, 1_000);
                    Assert.Throws<InvalidOperationException>(() => output.Advance(int.MaxValue));
                    Assert.Throws<InvalidOperationException>(() => output.Advance(2_000_000_000 - 1_000 + 1));
                }
            }
            catch (OutOfMemoryException) { }
        }

        [Fact]
        public static void GetMemoryAndSpan()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 2);
                Span<char> span = output.GetSpan();
                Memory<char> memory = output.GetMemory();
                Span<char> memorySpan = memory.Span;
                Assert.True(span.Length > 0);
                Assert.True(memorySpan.Length > 0);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(default, span[i]);
                    Assert.Equal(default, memorySpan[i]);
                }
            }

            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 2);
                ReadOnlySpan<char> writtenSoFar = output.WrittenMemory.Span;
                int previousAvailable = output.FreeCapacity;
                Span<char> span = output.GetSpan(500);
                Assert.True(span.Length >= 500);
                Assert.True(output.FreeCapacity >= 500);
                Assert.True(output.FreeCapacity > previousAvailable);

                Assert.True(writtenSoFar.SequenceEqual(span.Slice(0, output.WrittenCount)));

                Memory<char> memory = output.GetMemory();
                Span<char> memorySpan = memory.Span;
                Assert.True(span.Length >= 500);
                Assert.True(memorySpan.Length >= 500);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(default, span[i]);
                    Assert.Equal(default, memorySpan[i]);
                }

                memory = output.GetMemory(500);
                memorySpan = memory.Span;
                Assert.True(memorySpan.Length >= 500);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < memorySpan.Length; i++)
                {
                    Assert.Equal(default, memorySpan[i]);
                }
            }
        }

        [Fact]
        public static void GetSpanShouldAtleastDoubleWhenGrowing()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 100);
                int previousAvailable = output.FreeCapacity;

                Span<char> span = output.GetSpan(previousAvailable);
                Assert.Equal(previousAvailable, output.FreeCapacity);

                span = output.GetSpan(previousAvailable + 1);
                Assert.True(output.FreeCapacity >= previousAvailable * 2);
            }
        }

        [Fact]
        public static void GetSpanOnlyGrowsAboveThreshold()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                int previousAvailable = output.FreeCapacity;

                for (int i = 0; i < 10; i++)
                {
                    Span<char> span = output.GetSpan();
                    Assert.Equal(previousAvailable, output.FreeCapacity);
                }
            }

            using (var output = new ArrayBufferWriter<char>(100))
            {
                int previousAvailable = output.FreeCapacity;

                Span<char> span = output.GetSpan();
                Assert.True(output.FreeCapacity > previousAvailable);

                previousAvailable = output.FreeCapacity;
                for (int i = 0; i < 10; i++)
                {
                    span = output.GetSpan();
                    Assert.Equal(previousAvailable, output.FreeCapacity);
                }
            }
        }

        [Fact]
        public static void InvalidGetMemoryAndSpan()
        {
            using (var output = new ArrayBufferWriter<char>())
            {
                WriteData(output, 2);
                Assert.Throws<ArgumentException>(() => output.GetSpan(-1));
                Assert.Throws<ArgumentException>(() => output.GetMemory(-1));
            }
        }

        [Fact]
        public static void MultipleCallsToGetSpan()
        {
            using (var output = new ArrayBufferWriter<char>(300))
            {
                int previousAvailable = output.FreeCapacity;
                Assert.True(previousAvailable >= 300);
                Assert.True(output.Capacity >= 300);
                Assert.Equal(previousAvailable, output.Capacity);
                Span<char> span = output.GetSpan();
                Assert.True(span.Length >= previousAvailable);
                Assert.True(span.Length >= 256);
                Span<char> newSpan = output.GetSpan();
                Assert.Equal(span.Length, newSpan.Length);

                unsafe
                {
                    void* pSpan = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
                    void* pNewSpan = Unsafe.AsPointer(ref MemoryMarshal.GetReference(newSpan));
                    Assert.Equal((IntPtr)pSpan, (IntPtr)pNewSpan);
                }

                Assert.Equal(span.Length, output.GetSpan().Length);
            }
        }

        private static void WriteData(IBufferWriter<char> bufferWriter, int numBytes)
        {
            Span<char> outputSpan = bufferWriter.GetSpan(numBytes);
            Debug.Assert(outputSpan.Length >= numBytes);
            var random = new Random(42);

            var data = new char[numBytes];

            for (int i = 0; i < numBytes; i++)
            {
                data[i] = (char)random.Next(0, char.MaxValue);
            }

            data.CopyTo(outputSpan);
            bufferWriter.Advance(numBytes);
        }
    }
}
