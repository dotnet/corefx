// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Pipelines.Tests
{
    public class PipelineReaderWriterFacts : IDisposable
    {
        public PipelineReaderWriterFacts()
        {
            _pool = new TestMemoryPool();
            _pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
        }

        public void Dispose()
        {
            _pipe.Writer.Complete();
            _pipe.Reader.Complete();
            _pool?.Dispose();
        }

        private readonly Pipe _pipe;

        private readonly TestMemoryPool _pool;

        [Fact]
        public async Task AdvanceEmptyBufferAfterWritingResetsAwaitable()
        {
            byte[] bytes = Encoding.ASCII.GetBytes("Hello World");

            await _pipe.Writer.WriteAsync(bytes);
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            Assert.Equal(11, buffer.Length);
            Assert.True(buffer.IsSingleSegment);
            var array = new byte[11];
            buffer.First.Span.CopyTo(array);
            Assert.Equal("Hello World", Encoding.ASCII.GetString(array));

            _pipe.Reader.AdvanceTo(buffer.End);

            // Now write 0 and advance 0
            await _pipe.Writer.WriteAsync(new byte[] { });
            result = await _pipe.Reader.ReadAsync();
            _pipe.Reader.AdvanceTo(result.Buffer.End);

            ValueTask<ReadResult> awaitable = _pipe.Reader.ReadAsync();
            Assert.False(awaitable.IsCompleted);
        }

        [Fact]
        public async Task AdvanceResetsCommitHeadIndex()
        {
            _pipe.Writer.GetMemory(1);
            _pipe.Writer.Advance(100);
            await _pipe.Writer.FlushAsync();

            // Advance to the end
            ReadResult readResult = await _pipe.Reader.ReadAsync();
            _pipe.Reader.AdvanceTo(readResult.Buffer.End);

            // Try reading, it should block
            ValueTask<ReadResult> awaitable = _pipe.Reader.ReadAsync();
            Assert.False(awaitable.IsCompleted);

            // Unblock without writing anything
            _pipe.Writer.GetMemory();
            await _pipe.Writer.FlushAsync();

            Assert.True(awaitable.IsCompleted);

            // Advance to the end should reset awaitable
            readResult = await awaitable;
            _pipe.Reader.AdvanceTo(readResult.Buffer.End);

            // Try reading, it should block
            awaitable = _pipe.Reader.ReadAsync();
            Assert.False(awaitable.IsCompleted);
        }

        [Fact]
        public async Task AdvanceShouldResetStateIfReadCanceled()
        {
            _pipe.Reader.CancelPendingRead();

            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;
            _pipe.Reader.AdvanceTo(buffer.End);

            Assert.False(result.IsCompleted);
            Assert.True(result.IsCanceled);
            Assert.True(buffer.IsEmpty);

            ValueTask<ReadResult> awaitable = _pipe.Reader.ReadAsync();
            Assert.False(awaitable.IsCompleted);
        }

        [Fact]
        public async Task AdvanceToInvalidCursorThrows()
        {
            await _pipe.Writer.WriteAsync(new byte[100]);

            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            _pipe.Reader.AdvanceTo(buffer.End);

            _pipe.Reader.CancelPendingRead();
            result = await _pipe.Reader.ReadAsync();
            Assert.Throws<InvalidOperationException>(() => _pipe.Reader.AdvanceTo(buffer.End));
            _pipe.Reader.AdvanceTo(result.Buffer.End);
        }

        [Fact]
        public async Task AdvanceWithGetPositionCrossingIntoWriteHeadWorks()
        {
            // Create two blocks
            Memory<byte> memory = _pipe.Writer.GetMemory(1);
            _pipe.Writer.Advance(memory.Length);
            memory = _pipe.Writer.GetMemory(1);
            _pipe.Writer.Advance(memory.Length);
            await _pipe.Writer.FlushAsync();

            // Read single block
            ReadResult readResult = await _pipe.Reader.ReadAsync();

            // Allocate more memory
            memory = _pipe.Writer.GetMemory(1);

            // Create position that would cross into write head
            ReadOnlySequence<byte> buffer = readResult.Buffer;
            SequencePosition position = buffer.GetPosition(buffer.Length);

            // Return everything
            _pipe.Reader.AdvanceTo(position);

            // Advance writer
            _pipe.Writer.Advance(memory.Length);
        }

        [Fact]
        public async Task CompleteReaderAfterFlushWithoutAdvancingDoesNotThrow()
        {
            await _pipe.Writer.FlushAsync();
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            _pipe.Reader.Complete();
        }

        [Fact]
        public async Task ResetAfterCompleteReaderAndWriterWithoutAdvancingClearsEverything()
        {
            _pipe.Writer.WriteEmpty(4094);
            _pipe.Writer.WriteEmpty(4094);
            await _pipe.Writer.FlushAsync();
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            SequenceMarshal.TryGetReadOnlySequenceSegment(
                buffer,
                out ReadOnlySequenceSegment<byte> start,
                out int startIndex,
                out ReadOnlySequenceSegment<byte> end,
                out int endIndex);

            var startSegment = (BufferSegment)start;
            var endSegment = (BufferSegment)end;
            Assert.NotNull(startSegment.OwnedMemory);
            Assert.NotNull(endSegment.OwnedMemory);

            _pipe.Reader.Complete();

            // Nothing cleaned up
            Assert.NotNull(startSegment.OwnedMemory);
            Assert.NotNull(endSegment.OwnedMemory);

            _pipe.Writer.Complete();

            // Should be cleaned up now
            Assert.Null(startSegment.OwnedMemory);
            Assert.Null(endSegment.OwnedMemory);

            _pipe.Reset();
        }

        [Fact]
        public async Task AdvanceAfterCompleteThrows()
        {
            await _pipe.Writer.WriteAsync(new byte[1]);
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            _pipe.Reader.Complete();

            var exception = Assert.Throws<InvalidOperationException>(() => _pipe.Reader.AdvanceTo(buffer.End));
            Assert.Equal("Reading is not allowed after reader was completed.", exception.Message);
        }

        [Fact]
        public async Task EmptyBufferStartCrossingSegmentBoundaryIsTreatedLikeAndEnd()
        {
            Memory<byte> memory = _pipe.Writer.GetMemory();
            // Append one full segment to a pipe
            _pipe.Writer.Write(memory.Span);
            await _pipe.Writer.FlushAsync();

            // Consume entire segment
            ReadResult result = await _pipe.Reader.ReadAsync();
            _pipe.Reader.AdvanceTo(result.Buffer.End);

            // Append empty segment
            _pipe.Writer.GetMemory(1);
            await _pipe.Writer.FlushAsync();

            result = await _pipe.Reader.ReadAsync();

            Assert.True(result.Buffer.IsEmpty);
            Assert.Equal(result.Buffer.Start, result.Buffer.End);

            _pipe.Writer.GetMemory();
            _pipe.Reader.AdvanceTo(result.Buffer.Start);
            ValueTask<ReadResult> awaitable = _pipe.Reader.ReadAsync();
            Assert.False(awaitable.IsCompleted);
        }

        [Fact]
        public void FlushAsync_ReturnsCompletedTaskWhenMaxSizeIfZero()
        {
            PipeWriter writableBuffer = _pipe.Writer.WriteEmpty(1);
            ValueTask<FlushResult> flushTask = writableBuffer.FlushAsync();
            Assert.True(flushTask.IsCompleted);

            writableBuffer = _pipe.Writer.WriteEmpty(1);
            flushTask = writableBuffer.FlushAsync();
            Assert.True(flushTask.IsCompleted);
        }

        [Fact]
        public async Task FlushAsync_ThrowsIfWriterReaderWithException()
        {
            void ThrowTestException()
            {
                try
                {
                    throw new InvalidOperationException("Reader exception");
                }
                catch (Exception e)
                {
                    _pipe.Reader.Complete(e);
                }
            }

            ThrowTestException();

            InvalidOperationException invalidOperationException =
                await Assert.ThrowsAsync<InvalidOperationException>(async () => await _pipe.Writer.FlushAsync());

            Assert.Equal("Reader exception", invalidOperationException.Message);
            Assert.Contains("ThrowTestException", invalidOperationException.StackTrace);

            invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _pipe.Writer.FlushAsync());
            Assert.Equal("Reader exception", invalidOperationException.Message);
            Assert.Contains("ThrowTestException", invalidOperationException.StackTrace);
        }

        [Fact]
        public async Task HelloWorldAcrossTwoBlocks()
        {
            //     block 1       ->    block2
            // [padding..hello]  ->  [  world   ]
            PipeWriter writeBuffer = _pipe.Writer;
            var blockSize = _pipe.Writer.GetMemory().Length;

            byte[] paddingBytes = Enumerable.Repeat((byte)'a', blockSize - 5).ToArray();
            byte[] bytes = Encoding.ASCII.GetBytes("Hello World");

            writeBuffer.Write(paddingBytes);
            writeBuffer.Write(bytes);
            await writeBuffer.FlushAsync();

            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;
            Assert.False(buffer.IsSingleSegment);
            ReadOnlySequence<byte> helloBuffer = buffer.Slice(blockSize - 5);
            Assert.False(helloBuffer.IsSingleSegment);
            var memory = new List<ReadOnlyMemory<byte>>();
            foreach (ReadOnlyMemory<byte> m in helloBuffer)
            {
                memory.Add(m);
            }

            List<ReadOnlyMemory<byte>> spans = memory;
            _pipe.Reader.AdvanceTo(buffer.Start, buffer.Start);

            Assert.Equal(2, memory.Count);
            var helloBytes = new byte[spans[0].Length];
            spans[0].Span.CopyTo(helloBytes);
            var worldBytes = new byte[spans[1].Length];
            spans[1].Span.CopyTo(worldBytes);
            Assert.Equal("Hello", Encoding.ASCII.GetString(helloBytes));
            Assert.Equal(" World", Encoding.ASCII.GetString(worldBytes));
        }

        [Fact]
        public async Task ReadAsync_ThrowsIfWriterCompletedWithException()
        {
            void ThrowTestException()
            {
                try
                {
                    throw new InvalidOperationException("Writer exception");
                }
                catch (Exception e)
                {
                    _pipe.Writer.Complete(e);
                }
            }

            ThrowTestException();

            InvalidOperationException invalidOperationException =
                await Assert.ThrowsAsync<InvalidOperationException>(async () => await _pipe.Reader.ReadAsync());

            Assert.Equal("Writer exception", invalidOperationException.Message);
            Assert.Contains("ThrowTestException", invalidOperationException.StackTrace);

            invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _pipe.Reader.ReadAsync());
            Assert.Equal("Writer exception", invalidOperationException.Message);
            Assert.Contains("ThrowTestException", invalidOperationException.StackTrace);
        }

        [Fact]
        public async Task ReaderShouldNotGetUnflushedBytes()
        {
            // Write 10 and flush
            PipeWriter buffer = _pipe.Writer;
            buffer.Write(new byte[] { 0, 0, 0, 10 });
            await buffer.FlushAsync();

            // Write 9
            buffer = _pipe.Writer;
            buffer.Write(new byte[] { 0, 0, 0, 9 });

            // Write 8
            buffer.Write(new byte[] { 0, 0, 0, 8 });

            // Make sure we don't see it yet
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> reader = result.Buffer;

            Assert.Equal(4, reader.Length);
            Assert.Equal(new byte[] { 0, 0, 0, 10 }, reader.ToArray());

            // Don't move
            _pipe.Reader.AdvanceTo(reader.Start);

            // Now flush
            await buffer.FlushAsync();

            reader = (await _pipe.Reader.ReadAsync()).Buffer;

            Assert.Equal(12, reader.Length);
            Assert.Equal(new byte[] { 0, 0, 0, 10 }, reader.Slice(0, 4).ToArray());
            Assert.Equal(new byte[] { 0, 0, 0, 9 }, reader.Slice(4, 4).ToArray());
            Assert.Equal(new byte[] { 0, 0, 0, 8 }, reader.Slice(8, 4).ToArray());

            _pipe.Reader.AdvanceTo(reader.Start, reader.Start);
        }

        [Fact]
        public async Task ReaderShouldNotGetUnflushedBytesWhenOverflowingSegments()
        {
            // Fill the block with stuff leaving 5 bytes at the end
            Memory<byte> buffer = _pipe.Writer.GetMemory();

            int len = buffer.Length;
            // Fill the buffer with garbage
            //     block 1       ->    block2
            // [padding..hello]  ->  [  world   ]
            byte[] paddingBytes = Enumerable.Repeat((byte)'a', len - 5).ToArray();
            _pipe.Writer.Write(paddingBytes);
            await _pipe.Writer.FlushAsync();

            // Write 10 and flush
            _pipe.Writer.Write(new byte[] { 0, 0, 0, 10 });

            // Write 9
            _pipe.Writer.Write(new byte[] { 0, 0, 0, 9 });

            // Write 8
            _pipe.Writer.Write(new byte[] { 0, 0, 0, 8 });

            // Make sure we don't see it yet
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> reader = result.Buffer;

            Assert.Equal(len - 5, reader.Length);

            // Don't move
            _pipe.Reader.AdvanceTo(reader.End);

            // Now flush
            await _pipe.Writer.FlushAsync();

            reader = (await _pipe.Reader.ReadAsync()).Buffer;

            Assert.Equal(12, reader.Length);
            Assert.Equal(new byte[] { 0, 0, 0, 10 }, reader.Slice(0, 4).ToArray());
            Assert.Equal(new byte[] { 0, 0, 0, 9 }, reader.Slice(4, 4).ToArray());
            Assert.Equal(new byte[] { 0, 0, 0, 8 }, reader.Slice(8, 4).ToArray());

            _pipe.Reader.AdvanceTo(reader.Start, reader.Start);
        }

        [Fact]
        public async Task ReaderShouldNotGetUnflushedBytesWithAppend()
        {
            // Write 10 and flush
            PipeWriter buffer = _pipe.Writer;
            buffer.Write(new byte[] { 0, 0, 0, 10 });
            await buffer.FlushAsync();

            // Write Hello to another pipeline and get the buffer
            byte[] bytes = Encoding.ASCII.GetBytes("Hello");

            var c2 = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline));
            await c2.Writer.WriteAsync(bytes);
            ReadResult result = await c2.Reader.ReadAsync();
            ReadOnlySequence<byte> c2Buffer = result.Buffer;

            Assert.Equal(bytes.Length, c2Buffer.Length);

            // Write 9 to the buffer
            buffer = _pipe.Writer;
            buffer.Write(new byte[] { 0, 0, 0, 9 });

            // Append the data from the other pipeline
            foreach (ReadOnlyMemory<byte> memory in c2Buffer)
            {
                buffer.Write(memory.Span);
            }

            // Mark it as consumed
            c2.Reader.AdvanceTo(c2Buffer.End);

            // Now read and make sure we only see the comitted data
            result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> reader = result.Buffer;

            Assert.Equal(4, reader.Length);
            Assert.Equal(new byte[] { 0, 0, 0, 10 }, reader.Slice(0, 4).ToArray());

            // Consume nothing
            _pipe.Reader.AdvanceTo(reader.Start);

            // Flush the second set of writes
            await buffer.FlushAsync();

            reader = (await _pipe.Reader.ReadAsync()).Buffer;

            // int, int, "Hello"
            Assert.Equal(13, reader.Length);
            Assert.Equal(new byte[] { 0, 0, 0, 10 }, reader.Slice(0, 4).ToArray());
            Assert.Equal(new byte[] { 0, 0, 0, 9 }, reader.Slice(4, 4).ToArray());
            Assert.Equal("Hello", Encoding.ASCII.GetString(reader.Slice(8).ToArray()));

            _pipe.Reader.AdvanceTo(reader.Start, reader.Start);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadAsyncOnCompletedCapturesTheExecutionContext(bool useSynchronizationContext)
        {
            var pipe = new Pipe(new PipeOptions(useSynchronizationContext: useSynchronizationContext));

            SynchronizationContext previous = SynchronizationContext.Current;
            var sc = new CustomSynchronizationContext();

            if (useSynchronizationContext)
            {
                SynchronizationContext.SetSynchronizationContext(sc);
            }

            try
            {
                AsyncLocal<int> val = new AsyncLocal<int>();
                var tcs = new TaskCompletionSource<int>();
                val.Value = 10;

                pipe.Reader.ReadAsync().GetAwaiter().OnCompleted(() =>
                {
                    tcs.TrySetResult(val.Value);
                });

                val.Value = 20;

                pipe.Writer.WriteEmpty(100);
                // Don't run any code on our fake sync context
                await pipe.Writer.FlushAsync().ConfigureAwait(false);

                if (useSynchronizationContext)
                {
                    Assert.Equal(1, sc.Callbacks.Count);
                    sc.Callbacks[0].Item1(sc.Callbacks[0].Item2);
                }

                int value = await tcs.Task.ConfigureAwait(false);
                Assert.Equal(10, value);
            }
            finally
            {
                if (useSynchronizationContext)
                {
                    SynchronizationContext.SetSynchronizationContext(previous);
                }

                pipe.Reader.Complete();
                pipe.Writer.Complete();
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task FlushAsyncOnCompletedCapturesTheExecutionContextAndSyncContext(bool useSynchronizationContext)
        {
            var pipe = new Pipe(new PipeOptions(useSynchronizationContext: useSynchronizationContext, pauseWriterThreshold: 20, resumeWriterThreshold: 10));

            SynchronizationContext previous = SynchronizationContext.Current;
            var sc = new CustomSynchronizationContext();

            if (useSynchronizationContext)
            {
                SynchronizationContext.SetSynchronizationContext(sc);
            }

            try
            {
                AsyncLocal<int> val = new AsyncLocal<int>();
                var tcs = new TaskCompletionSource<int>();
                val.Value = 10;

                pipe.Writer.WriteEmpty(20);
                pipe.Writer.FlushAsync().GetAwaiter().OnCompleted(() =>
                {
                    tcs.TrySetResult(val.Value);
                });

                val.Value = 20;

                // Don't run any code on our fake sync context
                ReadResult result = await pipe.Reader.ReadAsync().ConfigureAwait(false);
                pipe.Reader.AdvanceTo(result.Buffer.End);

                if (useSynchronizationContext)
                {
                    Assert.Equal(1, sc.Callbacks.Count);
                    sc.Callbacks[0].Item1(sc.Callbacks[0].Item2);
                }

                int value = await tcs.Task.ConfigureAwait(false);
                Assert.Equal(10, value);
            }
            finally
            {
                if (useSynchronizationContext)
                {
                    SynchronizationContext.SetSynchronizationContext(previous);
                }

                pipe.Reader.Complete();
                pipe.Writer.Complete();
            }
        }

        [Fact]
        public async Task ReadingCanBeCanceled()
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(() => { _pipe.Writer.Complete(new OperationCanceledException(cts.Token)); });

            Task ignore = Task.Run(
                async () =>
                {
                    await Task.Delay(1000);
                    cts.Cancel();
                });

            await Assert.ThrowsAsync<OperationCanceledException>(
                async () =>
                {
                    ReadResult result = await _pipe.Reader.ReadAsync();
                    ReadOnlySequence<byte> buffer = result.Buffer;
                });
        }

        [Fact]
        public async Task SyncReadThenAsyncRead()
        {
            PipeWriter buffer = _pipe.Writer;
            buffer.Write(Encoding.ASCII.GetBytes("Hello World"));
            await buffer.FlushAsync();

            bool gotData = _pipe.Reader.TryRead(out ReadResult result);
            Assert.True(gotData);

            Assert.Equal("Hello World", Encoding.ASCII.GetString(result.Buffer.ToArray()));

            _pipe.Reader.AdvanceTo(result.Buffer.GetPosition(6));

            result = await _pipe.Reader.ReadAsync();

            Assert.Equal("World", Encoding.ASCII.GetString(result.Buffer.ToArray()));

            _pipe.Reader.AdvanceTo(result.Buffer.End);
        }

        [Fact]
        public void ThrowsOnAllocAfterCompleteWriter()
        {
            _pipe.Writer.Complete();

            Assert.Throws<InvalidOperationException>(() => _pipe.Writer.GetMemory());
        }

        [Fact]
        public void ThrowsOnReadAfterCompleteReader()
        {
            _pipe.Reader.Complete();

            Assert.Throws<InvalidOperationException>(() => _pipe.Reader.ReadAsync());
        }

        [Fact]
        public void TryReadAfterCancelPendingReadReturnsTrue()
        {
            _pipe.Reader.CancelPendingRead();

            bool gotData = _pipe.Reader.TryRead(out ReadResult result);

            Assert.True(result.IsCanceled);

            _pipe.Reader.AdvanceTo(result.Buffer.End);
        }

        [Fact]
        public void TryReadAfterCloseWriterWithExceptionThrows()
        {
            _pipe.Writer.Complete(new Exception("wow"));

            var ex = Assert.Throws<Exception>(() => _pipe.Reader.TryRead(out ReadResult result));
            Assert.Equal("wow", ex.Message);
        }

        [Fact]
        public void TryReadAfterReaderCompleteThrows()
        {
            _pipe.Reader.Complete();

            Assert.Throws<InvalidOperationException>(() => _pipe.Reader.TryRead(out ReadResult result));
        }

        [Fact]
        public void TryReadAfterWriterCompleteReturnsTrue()
        {
            _pipe.Writer.Complete();

            bool gotData = _pipe.Reader.TryRead(out ReadResult result);

            Assert.True(result.IsCompleted);

            _pipe.Reader.AdvanceTo(result.Buffer.End);
        }

        [Fact]
        public void WhenTryReadReturnsFalseDontNeedToCallAdvance()
        {
            bool gotData = _pipe.Reader.TryRead(out ReadResult result);
            Assert.False(gotData);
            _pipe.Reader.AdvanceTo(default);
        }

        [Fact]
        public async Task WritingDataMakesDataReadableViaPipeline()
        {
            byte[] bytes = Encoding.ASCII.GetBytes("Hello World");

            await _pipe.Writer.WriteAsync(bytes);
            ReadResult result = await _pipe.Reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;

            Assert.Equal(11, buffer.Length);
            Assert.True(buffer.IsSingleSegment);
            var array = new byte[11];
            buffer.First.Span.CopyTo(array);
            Assert.Equal("Hello World", Encoding.ASCII.GetString(array));

            _pipe.Reader.AdvanceTo(buffer.Start, buffer.Start);
        }

        [Fact]
        public async Task DoubleAsyncReadThrows()
        {
            ValueTask<ReadResult> readTask1 = _pipe.Reader.ReadAsync();
            ValueTask<ReadResult> readTask2 = _pipe.Reader.ReadAsync();

            var task1 = Assert.ThrowsAsync<InvalidOperationException>(async () => await readTask1);
            var task2 = Assert.ThrowsAsync<InvalidOperationException>(async () => await readTask2);

            var exception1 = await task1;
            var exception2 = await task2;

            Assert.Equal("Concurrent reads or writes are not supported.", exception1.Message);
            Assert.Equal("Concurrent reads or writes are not supported.", exception2.Message);
        }

        [Fact]
        public void GetResultBeforeCompletedThrows()
        {
            ValueTask<ReadResult> awaiter = _pipe.Reader.ReadAsync();

            Assert.Throws<InvalidOperationException>(() => awaiter.GetAwaiter().GetResult());
        }

        [Fact]
        public async Task CompleteAfterAdvanceCommits()
        {
            _pipe.Writer.WriteEmpty(4);

            _pipe.Writer.Complete();

            var result = await _pipe.Reader.ReadAsync();
            Assert.Equal(4, result.Buffer.Length);
            _pipe.Reader.AdvanceTo(result.Buffer.End);
        }

        [Fact]
        public async Task AdvanceWithoutReadThrows()
        {
            await _pipe.Writer.WriteAsync(new byte[3]);
            ReadResult readResult = await _pipe.Reader.ReadAsync();
            _pipe.Reader.AdvanceTo(readResult.Buffer.Start);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _pipe.Reader.AdvanceTo(readResult.Buffer.End));
            Assert.Equal("No reading operation to complete.", exception.Message);
        }

        [Fact]
        public async Task TryReadAfterReadAsyncThrows()
        {
            await _pipe.Writer.WriteAsync(new byte[3]);
            ReadResult readResult = await _pipe.Reader.ReadAsync();

            Assert.Throws<InvalidOperationException>(() => _pipe.Reader.TryRead(out _));
            _pipe.Reader.AdvanceTo(readResult.Buffer.Start);
        }

        [Fact]
        public void GetMemoryZeroReturnsNonEmpty()
        {
            Assert.True(_pipe.Writer.GetMemory(0).Length > 0);
        }

        [Fact]
        public async Task ReadAsyncWithDataReadyReturnsTaskWithValue()
        {
            _pipe.Writer.WriteEmpty(10);
            await _pipe.Writer.FlushAsync();
            var task = _pipe.Reader.ReadAsync();
            Assert.True(IsTaskWithResult(task));
        }

        [Fact]
        public void CancelledReadAsyncReturnsTaskWithValue()
        {
            _pipe.Reader.CancelPendingRead();
            var task = _pipe.Reader.ReadAsync();
            Assert.True(IsTaskWithResult(task));
        }

        [Fact]
        public void FlushAsyncWithoutBackpressureReturnsTaskWithValue()
        {
            _pipe.Writer.WriteEmpty(10);
            var task = _pipe.Writer.FlushAsync();
            Assert.True(IsTaskWithResult(task));
        }

        [Fact]
        public void CancelledFlushAsyncReturnsTaskWithValue()
        {
            _pipe.Writer.CancelPendingFlush();
            var task = _pipe.Writer.FlushAsync();
            Assert.True(IsTaskWithResult(task));
        }

        private bool IsTaskWithResult<T>(ValueTask<T> task)
        {
            return task == new ValueTask<T>(task.Result);
        }

        private sealed class CustomSynchronizationContext : SynchronizationContext
        {
            public List<Tuple<SendOrPostCallback, object>> Callbacks = new List<Tuple<SendOrPostCallback, object>>();

            public override void Post(SendOrPostCallback d, object state)
            {
                Callbacks.Add(Tuple.Create(d, state));
            }
        }
    }
}
