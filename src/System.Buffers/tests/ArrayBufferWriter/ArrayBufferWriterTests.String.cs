// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace System.Buffers.Tests
{
    public static partial class ArrayBufferWriterTests_String
    {
        [Fact]
        public static void ArrayBufferWriter_Ctor()
        {
            using (var output = new ArrayBufferWriter<string>())
            {
                Assert.True(output.FreeCapacity > 0);
                Assert.True(output.Capacity > 0);
                Assert.Equal(0, output.WrittenCount);
                Assert.True(ReadOnlyMemory<string>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
            }

            using (var output = new ArrayBufferWriter<string>(200))
            {
                Assert.True(output.FreeCapacity >= 200);
                Assert.True(output.Capacity >= 200);
                Assert.Equal(0, output.WrittenCount);
                Assert.True(ReadOnlyMemory<string>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
            }

            using (ArrayBufferWriter<string> output = default)
            {
                Assert.Equal(null, output);
            }
        }

        [Fact]
        public static void Invalid_Ctor()
        {
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter<string>(0));
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter<string>(-1));
            Assert.Throws<OutOfMemoryException>(() => new ArrayBufferWriter<string>(int.MaxValue));
        }

        [Fact]
        public static void DoubleDispose()
        {
            using (var output = new ArrayBufferWriter<string>())
            {
                output.Dispose();
            }
        }

        [Fact]
        public static void DisposeThenAccess()
        {
            using (var output = new ArrayBufferWriter<string>())
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
            using (var output = new ArrayBufferWriter<string>())
            {
                int previousAvailable = output.FreeCapacity;
                WriteData(output, 2);
                Assert.True(output.FreeCapacity < previousAvailable);
                Assert.True(output.WrittenCount > 0);
                Assert.False(ReadOnlyMemory<string>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
                output.Clear();
                Assert.Equal(0, output.WrittenCount);
                Assert.True(ReadOnlyMemory<string>.Empty.Span.SequenceEqual(output.WrittenMemory.Span));
            }
        }

        [Fact]
        public static void Advance()
        {
            using (var output = new ArrayBufferWriter<string>())
            {
                int capacity = output.Capacity;
                Assert.Equal(capacity, output.FreeCapacity);
                output.Advance(output.FreeCapacity);
                Assert.Equal(capacity, output.WrittenCount);
                Assert.Equal(0, output.FreeCapacity);
            }

            using (var output = new ArrayBufferWriter<string>())
            {
                output.Advance(output.Capacity);
                Assert.Equal(output.Capacity, output.WrittenCount);
                Assert.Equal(0, output.FreeCapacity);
                int previousCapacity = output.Capacity;
                Span<string> _ = output.GetSpan();
                Assert.True(output.Capacity > previousCapacity);
            }

            using (var output = new ArrayBufferWriter<string>())
            {
                WriteData(output, 2);
                ReadOnlySpan<string> previous = output.WrittenMemory.Span;
                output.Advance(10);
                Assert.False(previous.SequenceEqual(output.WrittenMemory.Span));
            }
        }

        [Fact]
        public static void AdvanceZero()
        {
            using (var output = new ArrayBufferWriter<string>())
            {
                WriteData(output, 2);
                Assert.Equal(2, output.WrittenCount);
                ReadOnlySpan<string> previous = output.WrittenMemory.Span;
                output.Advance(0);
                Assert.Equal(2, output.WrittenCount);
                Assert.True(previous.SequenceEqual(output.WrittenMemory.Span));
            }
        }

        [Fact]
        public static void InvalidAdvance()
        {
            using (var output = new ArrayBufferWriter<string>())
            {
                Assert.Throws<ArgumentException>(() => output.Advance(-1));
                Assert.Throws<InvalidOperationException>(() => output.Advance(output.Capacity + 1));
            }

            using (var output = new ArrayBufferWriter<string>())
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
                using (var output = new ArrayBufferWriter<string>(2_000_000_000))
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
            using (var output = new ArrayBufferWriter<string>())
            {
                WriteData(output, 2);
                Span<string> span = output.GetSpan();
                Memory<string> memory = output.GetMemory();
                Span<string> memorySpan = memory.Span;
                Assert.True(span.Length > 0);
                Assert.True(memorySpan.Length > 0);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(default, span[i]);
                    Assert.Equal(default, memorySpan[i]);
                }
            }

            using (var output = new ArrayBufferWriter<string>())
            {
                WriteData(output, 2);
                ReadOnlySpan<string> writtenSoFar = output.WrittenMemory.Span;
                int previousAvailable = output.FreeCapacity;
                Span<string> span = output.GetSpan(500);
                Assert.True(span.Length >= 500);
                Assert.True(output.FreeCapacity >= 500);
                Assert.True(output.FreeCapacity > previousAvailable);

                Assert.True(writtenSoFar.SequenceEqual(span.Slice(0, output.WrittenCount)));

                Memory<string> memory = output.GetMemory();
                Span<string> memorySpan = memory.Span;
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
            using (var output = new ArrayBufferWriter<string>())
            {
                WriteData(output, 100);
                int previousAvailable = output.FreeCapacity;

                Span<string> span = output.GetSpan(previousAvailable);
                Assert.Equal(previousAvailable, output.FreeCapacity);

                span = output.GetSpan(previousAvailable + 1);
                Assert.True(output.FreeCapacity >= previousAvailable * 2);
            }
        }

        [Fact]
        public static void GetSpanOnlyGrowsAboveThreshold()
        {
            using (var output = new ArrayBufferWriter<string>())
            {
                int previousAvailable = output.FreeCapacity;

                for (int i = 0; i < 10; i++)
                {
                    Span<string> span = output.GetSpan();
                    Assert.Equal(previousAvailable, output.FreeCapacity);
                }
            }

            using (var output = new ArrayBufferWriter<string>(100))
            {
                int previousAvailable = output.FreeCapacity;

                Span<string> span = output.GetSpan();
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
            using (var output = new ArrayBufferWriter<string>())
            {
                WriteData(output, 2);
                Assert.Throws<ArgumentException>(() => output.GetSpan(-1));
                Assert.Throws<ArgumentException>(() => output.GetMemory(-1));
            }
        }

        [Fact]
        public static void MultipleCallsToGetSpan()
        {
            using (var output = new ArrayBufferWriter<string>(300))
            {
                int previousAvailable = output.FreeCapacity;
                Assert.True(previousAvailable >= 300);
                Assert.True(output.Capacity >= 300);
                Assert.Equal(previousAvailable, output.Capacity);
                Span<string> span = output.GetSpan();
                Assert.True(span.Length >= previousAvailable);
                Assert.True(span.Length >= 256);
                Span<string> newSpan = output.GetSpan();
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

        private static void WriteData(IBufferWriter<string> bufferWriter, int numBytes)
        {
            Span<string> outputSpan = bufferWriter.GetSpan(numBytes);
            Debug.Assert(outputSpan.Length >= numBytes);
            var random = new Random(42);

            var data = new string[numBytes];

            for (int i = 0; i < numBytes; i++)
            {
                int length = random.Next(5, 10);
                data[i] = GetRandomString(length, 32, 127);
            }

            data.CopyTo(outputSpan);
            bufferWriter.Advance(numBytes);
        }

        private static string GetRandomString(int length, int minCodePoint, int maxCodePoint)
        {
            Random r = new Random(42);
            StringBuilder sb = new StringBuilder(length);
            while (length-- != 0)
            {
                sb.Append((char)r.Next(minCodePoint, maxCodePoint));
            }
            return sb.ToString();
        }
    }
}
