// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Pipelines.Tests
{
    public class PipeReaderStreamTests
    {
        public delegate Task<int> ReadAsyncDelegate(Stream stream, byte[] data);

        [Theory]
        [MemberData(nameof(ReadCalls))]
        public async Task ReadingFromPipeReaderStreamReadsFromUnderlyingPipeReader(ReadAsyncDelegate readAsync)
        {
            byte[] helloBytes = Encoding.ASCII.GetBytes("Hello World");
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(helloBytes);
            pipe.Writer.Complete();

            var stream = new PipeReaderStream(pipe.Reader);

            var buffer = new byte[1024];
            int read = await readAsync(stream, buffer);

            Assert.Equal(helloBytes, buffer.AsSpan(0, read).ToArray());
            pipe.Reader.Complete();
        }

        [Theory]
        [MemberData(nameof(ReadCalls))]
        public async Task AsStreamReturnsPipeReaderStream(ReadAsyncDelegate readAsync)
        {
            byte[] helloBytes = Encoding.ASCII.GetBytes("Hello World");
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(helloBytes);
            pipe.Writer.Complete();

            Stream stream = pipe.Reader.AsStream();

            var buffer = new byte[1024];
            int read = await readAsync(stream, buffer);

            Assert.Equal(helloBytes, buffer.AsSpan(0, read).ToArray());
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task ReadingWithSmallerBufferWorks()
        {
            byte[] helloBytes = Encoding.ASCII.GetBytes("Hello World");
            var pipe = new Pipe();
            await pipe.Writer.WriteAsync(helloBytes);
            pipe.Writer.Complete();

            Stream stream = pipe.Reader.AsStream();

            var buffer = new byte[5];
            int read = await stream.ReadAsync(buffer);

            Assert.Equal(5, read);
            Assert.Equal(helloBytes.AsSpan(0, 5).ToArray(), buffer);

            buffer = new byte[3];
            read = await stream.ReadAsync(buffer);

            Assert.Equal(3, read);
            Assert.Equal(helloBytes.AsSpan(5, 3).ToArray(), buffer);

            // Verify that the buffer is partially consumed and we can read the rest from the PipeReader directly
            ReadResult result = await pipe.Reader.ReadAsync();
            Assert.Equal(helloBytes.AsSpan(8).ToArray(), result.Buffer.ToArray());
            pipe.Reader.AdvanceTo(result.Buffer.End);

            pipe.Reader.Complete();
        }

        [Fact]
        public async Task EndOfPipeReaderReturnsZeroBytesFromReadAsync()
        {
            var pipe = new Pipe();
            Memory<byte> memory = pipe.Writer.GetMemory();
            pipe.Writer.Advance(5);
            pipe.Writer.Complete();

            Stream stream = pipe.Reader.AsStream();

            var buffer = new byte[5];
            var read = await stream.ReadAsync(buffer);

            Assert.Equal(5, read);

            read = await stream.ReadAsync(buffer);

            // Read again to make sure it always returns 0
            Assert.Equal(0, read);

            pipe.Reader.Complete();
        }

        [Fact]
        public async Task BuggyPipeReaderImplementationThrows()
        {
            var pipeReader = new BuggyPipeReader();
            
            Stream stream = pipeReader.AsStream();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await stream.ReadAsync(new byte[5]));
        }

        [Fact]
        public async Task WritingToPipeReaderStreamThrowsNotSupported()
        {
            var pipe = new Pipe();

            Stream stream = pipe.Reader.AsStream();
            Assert.False(stream.CanWrite);
            Assert.False(stream.CanSeek);
            Assert.True(stream.CanRead);
            Assert.Throws<NotSupportedException>(() => { long length = stream.Length; });
            Assert.Throws<NotSupportedException>(() => { long position = stream.Position; });
            Assert.Throws<NotSupportedException>(() => stream.Seek(0, SeekOrigin.Begin));
            Assert.Throws<NotSupportedException>(() => stream.Write(new byte[10], 0, 10));
            await Assert.ThrowsAsync<NotSupportedException>(() => stream.WriteAsync(new byte[10], 0, 10));
            await Assert.ThrowsAsync<NotSupportedException>(() => stream.WriteAsync(new byte[10]).AsTask());

            pipe.Reader.Complete();
            pipe.Writer.Complete();
        }

        [Fact]
        public async Task CancellingPendingReadThrowsOperationCancelledException()
        {
            var pipe = new Pipe();

            Stream stream = pipe.Reader.AsStream();
            ValueTask<int> task = stream.ReadAsync(new byte[1024]);
            Assert.False(task.IsCompleted);

            pipe.Reader.CancelPendingRead();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
            pipe.Writer.Complete();
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task CanReadAfterCancellingPendingRead()
        {
            var pipe = new Pipe();

            Stream stream = pipe.Reader.AsStream();
            ValueTask<int> task = stream.ReadAsync(new byte[1024]);
            Assert.False(task.IsCompleted);

            pipe.Reader.CancelPendingRead();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
            pipe.Writer.Complete();

            ReadResult result = await pipe.Reader.ReadAsync();
            Assert.True(result.IsCompleted);

            pipe.Reader.Complete();
        }

        [Fact]
        public async Task CancellationTokenFlowsToUnderlyingPipeReader()
        {
            var pipe = new Pipe();

            Stream stream = pipe.Reader.AsStream();
            var cts = new CancellationTokenSource();
            ValueTask<int> task = stream.ReadAsync(new byte[1024], cts.Token);
            Assert.False(task.IsCompleted);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
            pipe.Writer.Complete();
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task DefaultPipeReaderImplementationReturnsPipeReaderStream()
        {
            var pipeReader = new TestPipeReader();
            Stream stream = pipeReader.AsStream();

            await stream.ReadAsync(new byte[10]);

            Assert.True(pipeReader.ReadCalled);
            Assert.True(pipeReader.AdvanceToCalled);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task MultipleReadsToReadAllDataFromPipeReaderAsStream(bool syncCompletion, bool baseAsStream)
        {
            var pipe = new Pipe();
            PipeReader reader = baseAsStream ? new TestAsStreamPipeReader(pipe.Reader) : pipe.Reader;
            Stream stream = reader.AsStream();

            const int NumReads = 100;
            const int BytesPerRead = 10;
            var actualBytes = new byte[NumReads * BytesPerRead];
            new Random().NextBytes(actualBytes);

            var buffer = new byte[BytesPerRead];
            ValueTask<int> read;

            if (syncCompletion)
            {
                await pipe.Writer.WriteAsync(actualBytes);
            }

            // Read all the data from the pipe
            for (int i = 0; i < NumReads; i++)
            {
                read = stream.ReadAsync(buffer);
                if (!syncCompletion)
                {
                    await pipe.Writer.WriteAsync(actualBytes.AsSpan(i * BytesPerRead, BytesPerRead).ToArray());
                }
                Assert.Equal(BytesPerRead, await read);
            }

            // Have a pending read and cancel it
            read = stream.ReadAsync(buffer);
            pipe.CancelPendingRead();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await read);

            // Complete the pipe and read EOF
            if (syncCompletion)
            {
                pipe.Writer.Complete();
                Assert.Equal(0, await stream.ReadAsync(buffer));
            }
            else
            {
                read = stream.ReadAsync(buffer);
                pipe.Writer.Complete();
                Assert.Equal(0, await read);
            }

            // Reading again returns 0 again
            Assert.Equal(0, await stream.ReadAsync(buffer));

            // Try to read after it's been completed.
            reader.Complete();
            Assert.Throws<InvalidOperationException>(() => { stream.ReadAsync(buffer); });
        }

        [Fact]
        public void AsStreamReturnsSameInstance()
        {
            var pipeReader = new TestPipeReader();
            Stream stream = pipeReader.AsStream();

            Assert.Same(stream, pipeReader.AsStream());
        }

        public class BuggyPipeReader : PipeReader
        {
            public override void AdvanceTo(SequencePosition consumed)
            {
                
            }

            public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
            {
                
            }

            public override void CancelPendingRead()
            {
                throw new NotImplementedException();
            }

            public override void Complete(Exception exception = null)
            {
                throw new NotImplementedException();
            }

            public override void OnWriterCompleted(Action<Exception, object> callback, object state)
            {
                throw new NotImplementedException();
            }

            public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
            {
                // Returns a ReadResult with no buffer and with IsCompleted and IsCancelled false
                return default;
            }

            public override bool TryRead(out ReadResult result)
            {
                throw new NotImplementedException();
            }
        }

        public class TestPipeReader : PipeReader
        {
            public bool ReadCalled { get; set; }
            public bool AdvanceToCalled { get; set; }

            public override void AdvanceTo(SequencePosition consumed)
            {
                AdvanceToCalled = true;
            }

            public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
            {
                throw new NotImplementedException();
            }

            public override void CancelPendingRead()
            {
                throw new NotImplementedException();
            }

            public override void Complete(Exception exception = null)
            {
                throw new NotImplementedException();
            }

            public override void OnWriterCompleted(Action<Exception, object> callback, object state)
            {
                throw new NotImplementedException();
            }

            public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
            {
                ReadCalled = true;
                return new ValueTask<ReadResult>(new ReadResult(default, isCanceled: false, isCompleted: true));
            }

            public override bool TryRead(out ReadResult result)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Delegates to the wrapped reader for all operations other than AsStream.</summary>
        internal sealed class TestAsStreamPipeReader : PipeReader
        {
            private PipeReader _reader;

            public TestAsStreamPipeReader(PipeReader reader) => _reader = reader;

            public override void AdvanceTo(SequencePosition consumed) => _reader.AdvanceTo(consumed);
            public override void AdvanceTo(SequencePosition consumed, SequencePosition examined) => _reader.AdvanceTo(consumed, examined);
            public override void CancelPendingRead() => _reader.CancelPendingRead();
            public override void Complete(Exception exception = null) => _reader.Complete(exception);
            public override Task CopyToAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken)) => _reader.CopyToAsync(destination, cancellationToken);
            public override void OnWriterCompleted(Action<Exception, object> callback, object state) => _reader.OnWriterCompleted(callback, state);
            public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default(CancellationToken)) => _reader.ReadAsync(cancellationToken);
            public override bool TryRead(out ReadResult result) => _reader.TryRead(out result);
        }

        public static IEnumerable<object[]> ReadCalls
        {
            get
            {
                ReadAsyncDelegate readArrayAsync = (stream, data) =>
                {
                    return stream.ReadAsync(data, 0, data.Length);
                };

                ReadAsyncDelegate readMemoryAsync = async (stream, data) =>
                {
                    return await stream.ReadAsync(data);
                };

                ReadAsyncDelegate readMemoryAsyncWithThreadHop = async (stream, data) =>
                {
                    await Task.Yield();

                    return await stream.ReadAsync(data);
                };

                ReadAsyncDelegate readArraySync = (stream, data) =>
                {
                    return Task.FromResult(stream.Read(data, 0, data.Length));
                };

                ReadAsyncDelegate readSpanSync = (stream, data) =>
                {
                    return Task.FromResult(stream.Read(data));
                };

                yield return new object[] { readArrayAsync };
                yield return new object[] { readMemoryAsync };
                yield return new object[] { readMemoryAsyncWithThreadHop };
                yield return new object[] { readArraySync };
                yield return new object[] { readSpanSync };
            }
        }
    }
}
