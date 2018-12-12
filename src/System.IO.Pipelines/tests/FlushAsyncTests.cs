// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Pipelines.Tests
{
    public class FlushAsyncTests : PipeTest
    {
        [Fact]
        public void FlushAsync_ReturnsCompletedTaskWhenMaxSizeIfZero()
        {
            PipeWriter writableBuffer = Pipe.Writer.WriteEmpty(1);
            ValueTask<FlushResult> flushTask = writableBuffer.FlushAsync();
            Assert.True(flushTask.IsCompleted);

            writableBuffer = Pipe.Writer.WriteEmpty(1);
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
                    Pipe.Reader.Complete(e);
                }
            }

            ThrowTestException();

            InvalidOperationException invalidOperationException =
                await Assert.ThrowsAsync<InvalidOperationException>(async () => await Pipe.Writer.FlushAsync());

            Assert.Equal("Reader exception", invalidOperationException.Message);
            Assert.Contains("ThrowTestException", invalidOperationException.StackTrace);

            invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await Pipe.Writer.FlushAsync());
            Assert.Equal("Reader exception", invalidOperationException.Message);
            Assert.Contains("ThrowTestException", invalidOperationException.StackTrace);

            Assert.Single(Regex.Matches(invalidOperationException.StackTrace, "Pipe.GetFlushResult"));
        }

        [Fact]
        public async Task CallingFlushAsyncMultipleTimesAllowsFirstToComplete()
        {
            Pipe.Writer.WriteEmpty(65);
            var flushTask = Pipe.Writer.FlushAsync().AsTask();

            Pipe.Writer.WriteEmpty(1);

            _ = Pipe.Writer.FlushAsync();
            _ = Pipe.Writer.FlushAsync();
            _ = Pipe.Writer.FlushAsync();

            var result = await Pipe.Reader.ReadAsync();
            Pipe.Reader.AdvanceTo(result.Buffer.End);

            Assert.True(flushTask.IsCompleted);
        }
    }
}
