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
    public static partial class ArrayBufferWriterTests
    {
        [Fact]
        public static void ArrayBufferWriter_Ctor()
        {
            using (var output = new ArrayBufferWriter())
            {
                Assert.True(output.BytesAvailable > 0);
                Assert.True(output.Capacity > 0);
                Assert.Equal(0, output.BytesWritten);
                Assert.Equal(0, output.TotalBytesWritten);
                Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
            }

            using (var output = new ArrayBufferWriter(200))
            {
                Assert.True(output.BytesAvailable >= 200);
                Assert.True(output.Capacity >= 200);
                Assert.Equal(0, output.BytesWritten);
                Assert.Equal(0, output.TotalBytesWritten);
                Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
            }

            using (ArrayBufferWriter output = default)
            {
                Assert.Equal(null, output);
            }
        }

        [Fact]
        public static void Invalid_Ctor()
        {
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter(0));
            Assert.Throws<ArgumentException>(() => new ArrayBufferWriter(-1));
            Assert.Throws<OutOfMemoryException>(() => new ArrayBufferWriter(int.MaxValue));
        }

        [Fact]
        public static void DoubleDispose()
        {
            using (var output = new ArrayBufferWriter())
            {
                output.Dispose();
            }
        }

        [Fact]
        public static void DisposeThenAccess()
        {
            using (var output = new ArrayBufferWriter())
            {
                output.Dispose();
                Assert.Throws<ObjectDisposedException>(() => output.BytesAvailable);
                Assert.Throws<ObjectDisposedException>(() => output.Capacity);
                Assert.Throws<ObjectDisposedException>(() => output.BytesWritten);
                Assert.Throws<ObjectDisposedException>(() => output.TotalBytesWritten);
                Assert.Throws<ObjectDisposedException>(() => output.OutputAsMemory);
                Assert.Throws<ObjectDisposedException>(() => output.CopyToAndReset(default));
                Assert.Throws<ObjectDisposedException>(() => output.Reset());
                Assert.Throws<ObjectDisposedException>(() => ((IBufferWriter<byte>)output).Advance(1));
                Assert.Throws<ObjectDisposedException>(() => ((IBufferWriter<byte>)output).GetMemory());
            }
        }

        [Fact]
        public static void Reset()
        {
            using (var output = new ArrayBufferWriter())
            {
                int previousAvailable = output.BytesAvailable;
                WriteData(output, 2);
                Assert.True(output.BytesAvailable < previousAvailable);
                Assert.Equal(0, output.TotalBytesWritten);
                Assert.True(output.BytesWritten > 0);
                Assert.False(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
                output.Reset();
                Assert.Equal(0, output.BytesWritten);
                Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));
            }
        }

        [Fact]
        public static void Advance()
        {
            using (var output = new ArrayBufferWriter())
            {
                int capacity = output.Capacity;
                Assert.Equal(capacity, output.BytesAvailable);
                ((IBufferWriter<byte>)output).Advance(output.BytesAvailable);
                Assert.Equal(capacity, output.BytesWritten);
                Assert.Equal(0, output.BytesAvailable);
            }

            using (var output = new ArrayBufferWriter())
            {
                IBufferWriter<byte> bufferWriter = output;
                bufferWriter.Advance(output.Capacity);
                Assert.Equal(0, output.TotalBytesWritten);
                Assert.Equal(output.Capacity, output.BytesWritten);
                Assert.Equal(0, output.BytesAvailable);
                int previousCapacity = output.Capacity;
                Span<byte> _ = bufferWriter.GetSpan();
                Assert.True(output.Capacity > previousCapacity);
            }

            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 2);
                ReadOnlySpan<byte> previous = output.OutputAsSpan;
                ((IBufferWriter<byte>)output).Advance(10);
                Assert.False(previous.SequenceEqual(output.OutputAsSpan));
            }
        }

        [Fact]
        public static void AdvanceZero()
        {
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 2);
                Assert.Equal(2, output.BytesWritten);
                ReadOnlySpan<byte> previous = output.OutputAsSpan;
                ((IBufferWriter<byte>)output).Advance(0);
                Assert.Equal(2, output.BytesWritten);
                Assert.True(previous.SequenceEqual(output.OutputAsSpan));
            }
        }

        [Fact]
        public static void InvalidAdvance()
        {
            using (var output = new ArrayBufferWriter())
            {
                IBufferWriter<byte> bufferWriter = output;
                Assert.Throws<ArgumentException>(() => bufferWriter.Advance(-1));
                Assert.Throws<InvalidOperationException>(() => bufferWriter.Advance(output.Capacity + 1));
            }

            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 100);
                IBufferWriter<byte> bufferWriter = output;
                Assert.Throws<InvalidOperationException>(() => bufferWriter.Advance(output.BytesAvailable + 1));
            }
        }

        [Fact]
        [OuterLoop]
        public static void InvalidAdvance_Large()
        {
            using (var output = new ArrayBufferWriter(2_000_000_000))
            {
                WriteData(output, 1_000);
                IBufferWriter<byte> bufferWriter = output;
                Assert.Throws<InvalidOperationException>(() => bufferWriter.Advance(int.MaxValue));
                Assert.Throws<InvalidOperationException>(() => bufferWriter.Advance(2_000_000_000 - 1_000 + 1));
            }
        }

        [Fact]
        public static void GetMemoryAndSpan()
        {
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 2);
                IBufferWriter<byte> bufferWriter = output;
                Span<byte> span = bufferWriter.GetSpan();
                Memory<byte> memory = bufferWriter.GetMemory();
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

            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 2);
                ReadOnlySpan<byte> writtenSoFar = output.OutputAsSpan;
                int previousAvailable = output.BytesAvailable;
                IBufferWriter<byte> bufferWriter = output;
                Span<byte> span = bufferWriter.GetSpan(500);
                Assert.True(span.Length >= 500);
                Assert.True(output.BytesAvailable >= 500);
                Assert.True(output.BytesAvailable > previousAvailable);

                Assert.True(writtenSoFar.SequenceEqual(span.Slice(0, output.BytesWritten)));

                Memory<byte> memory = bufferWriter.GetMemory();
                Span<byte> memorySpan = memory.Span;
                Assert.True(span.Length >= 500);
                Assert.True(memorySpan.Length >= 500);
                Assert.Equal(span.Length, memorySpan.Length);
                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(0, span[i]);
                    Assert.Equal(0, memorySpan[i]);
                }

                memory = bufferWriter.GetMemory(500);
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
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 100);
                int previousAvailable = output.BytesAvailable;
                IBufferWriter<byte> bufferWriter = output;

                Span<byte> span = bufferWriter.GetSpan(previousAvailable);
                Assert.Equal(previousAvailable, output.BytesAvailable);

                span = bufferWriter.GetSpan(previousAvailable + 1);
                Assert.True(output.BytesAvailable >= previousAvailable * 2);
            }
        }

        [Fact]
        public static void GetSpanOnlyGrowsAboveThreshold()
        {
            using (var output = new ArrayBufferWriter())
            {
                int previousAvailable = output.BytesAvailable;
                IBufferWriter<byte> bufferWriter = output;

                for (int i = 0; i < 10; i++)
                {
                    Span<byte> span = bufferWriter.GetSpan();
                    Assert.Equal(previousAvailable, output.BytesAvailable);
                }
            }

            using (var output = new ArrayBufferWriter(100))
            {
                int previousAvailable = output.BytesAvailable;
                IBufferWriter<byte> bufferWriter = output;

                Span<byte> span = bufferWriter.GetSpan();
                Assert.True(output.BytesAvailable > previousAvailable);

                previousAvailable = output.BytesAvailable;
                for (int i = 0; i < 10; i++)
                {
                    span = bufferWriter.GetSpan();
                    Assert.Equal(previousAvailable, output.BytesAvailable);
                }
            }
        }

        [Fact]
        public static void InvalidGetMemoryAndSpan()
        {
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 2);
                IBufferWriter<byte> bufferWriter = output;
                Assert.Throws<ArgumentException>(() => bufferWriter.GetSpan(-1));
                Assert.Throws<ArgumentException>(() => bufferWriter.GetMemory(-1));
            }
        }

        [Fact]
        public static void MultipleCallsToGetSpan()
        {
            using (var output = new ArrayBufferWriter(300))
            {
                int previousAvailable = output.BytesAvailable;
                Assert.True(previousAvailable >= 300);
                Assert.True(output.Capacity >= 300);
                Assert.Equal(previousAvailable, output.Capacity);
                IBufferWriter<byte> bufferWriter = output;
                Span<byte> span = bufferWriter.GetSpan();
                Assert.True(span.Length >= previousAvailable);
                Assert.True(span.Length >= 256);
                Span<byte> newSpan = bufferWriter.GetSpan();
                Assert.Equal(span.Length, newSpan.Length);

                unsafe
                {
                    void* pSpan = Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
                    void* pNewSpan = Unsafe.AsPointer(ref MemoryMarshal.GetReference(newSpan));
                    Assert.Equal((IntPtr)pSpan, (IntPtr)pNewSpan);
                }

                Assert.Equal(span.Length, bufferWriter.GetSpan().Length);
            }
        }

        [Fact]
        public static void WriteAndCopyToStream()
        {
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 100);
                using (var memStream = new MemoryStream(100))
                {
                    Assert.Equal(100, output.BytesWritten);
                    Assert.Equal(0, output.TotalBytesWritten);

                    ReadOnlySpan<byte> outputSpan = output.OutputAsSpan.ToArray();

                    ReadOnlySpan<byte> transientSpan = output.OutputAsSpan;

                    Assert.True(transientSpan[0] != 0);

                    output.CopyToAndReset(memStream);

                    Assert.True(transientSpan[0] == 0);

                    Assert.Equal(100, output.TotalBytesWritten);
                    Assert.Equal(0, output.BytesWritten);
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
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 100);
                using (var memStream = new MemoryStream(100))
                {
                    Assert.Equal(100, output.BytesWritten);
                    Assert.Equal(0, output.TotalBytesWritten);

                    ReadOnlyMemory<byte> outputMemory = output.OutputAsMemory.ToArray();

                    ReadOnlyMemory<byte> transient = output.OutputAsMemory;

                    Assert.True(transient.Span[0] != 0);

                    await output.CopyToAndResetAsync(memStream);

                    Assert.True(transient.Span[0] == 0);

                    Assert.Equal(100, output.TotalBytesWritten);
                    Assert.Equal(0, output.BytesWritten);
                    byte[] streamOutput = memStream.ToArray();

                    Assert.True(ReadOnlyMemory<byte>.Empty.Span.SequenceEqual(output.OutputAsMemory.Span));
                    Assert.True(ReadOnlySpan<byte>.Empty.SequenceEqual(output.OutputAsSpan));

                    Assert.Equal(outputMemory.Length, streamOutput.Length);
                    Assert.True(outputMemory.Span.SequenceEqual(streamOutput));
                }
            }
        }

        [Fact]
        public static void InvalidStream()
        {
            using (var output = new ArrayBufferWriter())
            {
                WriteData(output, 100);
                Assert.Throws<ArgumentNullException>(() => output.CopyToAndReset(null));
                Assert.ThrowsAsync<ArgumentNullException>(() => output.CopyToAndResetAsync(null));

                Stream stream = null;
                Assert.Throws<ArgumentNullException>(() => output.CopyToAndReset(stream));
                Assert.ThrowsAsync<ArgumentNullException>(() => output.CopyToAndResetAsync(stream));
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
