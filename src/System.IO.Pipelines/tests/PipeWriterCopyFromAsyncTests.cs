using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Pipelines.Tests
{
    public class PipeWriterCopyFromAsyncTests
    {
        [Fact]
        public async Task CopyFromAsyncWorks()
        {
            var helloBytes = Encoding.UTF8.GetBytes("Hello World");

            var pipe = new Pipe();
            var stream = new MemoryStream(helloBytes);
            await pipe.Writer.CopyFromAsync(stream);

            ReadResult result = await pipe.Reader.ReadAsync();

            Assert.Equal(helloBytes, result.Buffer.ToArray());

            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
            pipe.Writer.Complete();
        }

        [Fact]
        public async Task CopyFromAsyncCalledMultipleTimesWorks()
        {
            var hello = "Hello World";
            var helloBytes = Encoding.UTF8.GetBytes(hello);
            var expected = Encoding.UTF8.GetBytes(hello + hello + hello);

            var pipe = new Pipe();
            await pipe.Writer.CopyFromAsync(new MemoryStream(helloBytes));
            await pipe.Writer.CopyFromAsync(new MemoryStream(helloBytes));
            await pipe.Writer.CopyFromAsync(new MemoryStream(helloBytes));
            pipe.Writer.Complete();

            ReadResult result = await pipe.Reader.ReadAsync();

            Assert.Equal(expected, result.Buffer.ToArray());

            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task StreamCopyToAsyncWorks()
        {
            var helloBytes = Encoding.UTF8.GetBytes("Hello World");

            var pipe = new Pipe();
            var stream = new MemoryStream(helloBytes);
            await stream.CopyToAsync(pipe.Writer);

            ReadResult result = await pipe.Reader.ReadAsync();

            Assert.Equal(helloBytes, result.Buffer.ToArray());

            pipe.Reader.AdvanceTo(result.Buffer.End);
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task CancellingViaCancelPendingFlushThrows()
        {
            var helloBytes = Encoding.UTF8.GetBytes("Hello World");

            var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: helloBytes.Length - 1, resumeWriterThreshold: 0));
            var stream = new MemoryStream(helloBytes);
            Task task = pipe.Writer.CopyFromAsync(stream);

            Assert.False(task.IsCompleted);

            pipe.Writer.CancelPendingFlush();

            await Assert.ThrowsAsync<OperationCanceledException>(() => task);

            pipe.Writer.Complete();
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task CancellingViaCancellationTokenThrows()
        {
            var helloBytes = Encoding.UTF8.GetBytes("Hello World");

            var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: helloBytes.Length - 1, resumeWriterThreshold: 0));
            var stream = new MemoryStream(helloBytes);
            var cts = new CancellationTokenSource();
            Task task = pipe.Writer.CopyFromAsync(stream, cts.Token);

            Assert.False(task.IsCompleted);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => task);

            pipe.Writer.Complete();
            pipe.Reader.Complete();
        }

        [Fact]
        public async Task CancellingStreamViaCancellationTokenThrows()
        {
            var pipe = new Pipe();
            var stream = new CancelledReadsStream();
            var cts = new CancellationTokenSource();
            Task task = pipe.Writer.CopyFromAsync(stream, cts.Token);

            Assert.False(task.IsCompleted);

            cts.Cancel();

            stream.WaitForReadTask.TrySetResult(null);

            await Assert.ThrowsAsync<OperationCanceledException>(() => task);


            pipe.Writer.Complete();
            pipe.Reader.Complete();
        }

        private class CancelledReadsStream : ReadOnlyStream
        {
            public TaskCompletionSource<object> WaitForReadTask = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
            
            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                await WaitForReadTask.Task;

                cancellationToken.ThrowIfCancellationRequested();

                return 0;
            }

#if !netstandard
            public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                await WaitForReadTask.Task;

                cancellationToken.ThrowIfCancellationRequested();

                return 0;
            }
#endif
        }

        private abstract class ReadOnlyStream : Stream
        {
            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
