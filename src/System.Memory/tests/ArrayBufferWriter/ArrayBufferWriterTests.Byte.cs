// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace System.Buffers.Tests
{
    public static partial class ArrayBufferWriterTests_Byte
    {
        [Fact]
        public static void ArrayBufferWriter_Ctor()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                Assert.True(output.AvailableSpace > 0);
                Assert.True(output.Capacity > 0);
                Assert.Equal(0, output.CurrentIndex);
                Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
            }

            using (var output = new ArrayBufferWriter<byte>(200))
            {
                Assert.True(output.AvailableSpace >= 200);
                Assert.True(output.Capacity >= 200);
                Assert.Equal(0, output.CurrentIndex);
                Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
            }

            using (ArrayBufferWriter<byte> output = default)
            {
                Assert.Equal(null, output);
            }
        }

        [Fact]
        public static void Invalid_Ctor()
        {
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter<byte>(0));
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter<byte>(-1));
            Assert.Throws<OutOfMemoryException>(() => new ArrayBufferWriter<byte>(int.MaxValue));
        }

        [Fact]
        public static void DoubleDispose()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                output.Dispose();
            }
        }

        [Fact]
        public static void DisposeThenAccess()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                output.Dispose();
                Assert.Throws<ObjectDisposedException>(() => output.AvailableSpace);
                Assert.Throws<ObjectDisposedException>(() => output.Capacity);
                Assert.Throws<ObjectDisposedException>(() => output.CurrentIndex);
                Assert.Throws<ObjectDisposedException>(() => output.OutputAsMemory);
                Assert.Throws<ObjectDisposedException>(() => output.Clear());
                Assert.Throws<ObjectDisposedException>(() => output.Advance(1));
                Assert.Throws<ObjectDisposedException>(() => output.GetMemory());
            }
        }

        [Fact]
        public static void Clear()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                int previousAvailable = output.AvailableSpace;
                WriteData(output, 2);
                Assert.True(output.AvailableSpace < previousAvailable);
                Assert.True(output.CurrentIndex > 0);
                Assert.False(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
                output.Clear();
                Assert.Equal(0, output.CurrentIndex);
                Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
            }
        }

        [Fact]
        public static void Advance()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                int capacity = output.Capacity;
                Assert.Equal(capacity, output.AvailableSpace);
                output.Advance(output.AvailableSpace);
                Assert.Equal(capacity, output.CurrentIndex);
                Assert.Equal(0, output.AvailableSpace);
            }

            using (var output = new ArrayBufferWriter<byte>())
            {
                output.Advance(output.Capacity);
                Assert.Equal(output.Capacity, output.CurrentIndex);
                Assert.Equal(0, output.AvailableSpace);
                int previousCapacity = output.Capacity;
                Span<byte> _ = output.GetSpan();
                Assert.True(output.Capacity > previousCapacity);
            }

            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 2);
                ReadOnlySpan<byte> previous = output.OutputAsSpan;
                output.Advance(10);
                Assert.False(previous.SequenceEqual(output.OutputAsSpan));
            }
        }

        [Fact]
        public static void AdvanceZero()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 2);
                Assert.Equal(2, output.CurrentIndex);
                ReadOnlySpan<byte> previous = output.OutputAsSpan;
                output.Advance(0);
                Assert.Equal(2, output.CurrentIndex);
                Assert.True(previous.SequenceEqual(output.OutputAsSpan));
            }
        }

        [Fact]
        public static void InvalidAdvance()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                Assert.Throws<ArgumentException>(() => output.Advance(-1));
                Assert.Throws<InvalidOperationException>(() => output.Advance(output.Capacity + 1));
            }

            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 100);
                Assert.Throws<InvalidOperationException>(() => output.Advance(output.AvailableSpace + 1));
            }
        }

        [Fact]
        [OuterLoop]
        public static void InvalidAdvance_Large()
        {
            using (var output = new ArrayBufferWriter<byte>(2_000_000_000))
            {
                WriteData(output, 1_000);
                Assert.Throws<InvalidOperationException>(() => output.Advance(int.MaxValue));
                Assert.Throws<InvalidOperationException>(() => output.Advance(2_000_000_000 - 1_000 + 1));
            }
        }

        [Fact]
        public static void GetMemoryAndSpan()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 2);
                Span<byte> span = output.GetSpan();
                Memory<byte> memory = output.GetMemory();
                Span<byte> memorySpan = memory.Span;
                Assert.True(span.Length > 0);
                Assert.True(memorySpan.Length > 0);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(0, span[i]);
                    Assert.Equal(0, memorySpan[i]);
                }
            }

            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 2);
                ReadOnlySpan<byte> writtenSoFar = output.OutputAsSpan;
                int previousAvailable = output.AvailableSpace;
                Span<byte> span = output.GetSpan(500);
                Assert.True(span.Length >= 500);
                Assert.True(output.AvailableSpace >= 500);
                Assert.True(output.AvailableSpace > previousAvailable);

                Assert.True(writtenSoFar.SequenceEqual(span.Slice(0, output.CurrentIndex)));

                Memory<byte> memory = output.GetMemory();
                Span<byte> memorySpan = memory.Span;
                Assert.True(span.Length >= 500);
                Assert.True(memorySpan.Length >= 500);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(0, span[i]);
                    Assert.Equal(0, memorySpan[i]);
                }

                memory = output.GetMemory(500);
                memorySpan = memory.Span;
                Assert.True(memorySpan.Length >= 500);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < memorySpan.Length; i++)
                {
                    Assert.Equal(0, memorySpan[i]);
                }
            }
        }

        [Fact]
        public static void GetSpanShouldAtleastDoubleWhenGrowing()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 100);
                int previousAvailable = output.AvailableSpace;

                Span<byte> span = output.GetSpan(previousAvailable);
                Assert.Equal(previousAvailable, output.AvailableSpace);

                span = output.GetSpan(previousAvailable + 1);
                Assert.True(output.AvailableSpace >= previousAvailable * 2);
            }
        }

        [Fact]
        public static void GetSpanOnlyGrowsAboveThreshold()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                int previousAvailable = output.AvailableSpace;

                for (int i = 0; i < 10; i++)
                {
                    Span<byte> span = output.GetSpan();
                    Assert.Equal(previousAvailable, output.AvailableSpace);
                }
            }

            using (var output = new ArrayBufferWriter<byte>(100))
            {
                int previousAvailable = output.AvailableSpace;

                Span<byte> span = output.GetSpan();
                Assert.True(output.AvailableSpace > previousAvailable);

                previousAvailable = output.AvailableSpace;
                for (int i = 0; i < 10; i++)
                {
                    span = output.GetSpan();
                    Assert.Equal(previousAvailable, output.AvailableSpace);
                }
            }
        }

        [Fact]
        public static void InvalidGetMemoryAndSpan()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 2);
                Assert.Throws<ArgumentException>(() => output.GetSpan(-1));
                Assert.Throws<ArgumentException>(() => output.GetMemory(-1));
            }
        }

        [Fact]
        public static void MultipleCallsToGetSpan()
        {
            using (var output = new ArrayBufferWriter<byte>(300))
            {
                int previousAvailable = output.AvailableSpace;
                Assert.True(previousAvailable >= 300);
                Assert.True(output.Capacity >= 300);
                Assert.Equal(previousAvailable, output.Capacity);
                Span<byte> span = output.GetSpan();
                Assert.True(span.Length >= previousAvailable);
                Assert.True(span.Length >= 256);
                Span<byte> newSpan = output.GetSpan();
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

        [Fact]
        public static void WriteAndCopyToStream()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 100);
                using (var memStream = new MemoryStream(100))
                {
                    Assert.Equal(100, output.CurrentIndex);

                    ReadOnlySpan<byte> outputSpan = output.OutputAsSpan.ToArray();

                    ReadOnlySpan<byte> transientSpan = output.OutputAsSpan;

                    Assert.True(transientSpan[0] != 0);

                    memStream.Write(transientSpan);
                    output.Clear();

                    Assert.True(transientSpan[0] == 0);

                    Assert.Equal(0, output.CurrentIndex);
                    byte[] streamOutput = memStream.ToArray();

                    Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                    Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));

                    Assert.Equal(outputSpan.Length, streamOutput.Length);
                    Assert.True(outputSpan.SequenceEqual(streamOutput));
                }
            }
        }


        [Fact]
        public static async Task WriteAndCopyToStreamAsync()
        {
            using (var output = new ArrayBufferWriter<byte>())
            {
                WriteData(output, 100);
                using (var memStream = new MemoryStream(100))
                {
                    Assert.Equal(100, output.CurrentIndex);

                    ReadOnlyMemory<byte> outputMemory = output.OutputAsMemory.ToArray();

                    ReadOnlyMemory<byte> transient = output.OutputAsMemory;

                    Assert.True(transient.Span[0] != 0);

                    await memStream.WriteAsync(transient);
                    output.Clear();

                    Assert.True(transient.Span[0] == 0);

                    Assert.Equal(0, output.CurrentIndex);
                    byte[] streamOutput = memStream.ToArray();

                    Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                    Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));

                    Assert.Equal(outputMemory.Length, streamOutput.Length);
                    Assert.True(outputMemory.Span.SequenceEqual(streamOutput));
                }
            }
        }

        private static void WriteData(IBufferWriter<byte> bufferWriter, int numBytes)
        {
            Span<byte> outputSpan = bufferWriter.GetSpan(numBytes);
            Debug.Assert(outputSpan.Length >= numBytes);
            var random = new Random(42);

            var data = new byte[numBytes];
            random.NextBytes(data);
            data.CopyTo(outputSpan);
            bufferWriter.Advance(numBytes);
        }
    }
}
