// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Exclude until we determine the dependency should work on System.IO.Pipelines
#if false

using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class PipeTests
    {
        [Fact]
        public static async void NullObjectInputFails()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await JsonSerializer.ReadAsync<string>((PipeReader)null));
        }

        [Fact]
        public async static Task VerifyValueFail()
        {
            Pipe pipe = new Pipe();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await JsonSerializer.WriteAsync("", null, pipe.Writer));
        }

        [Fact]
        public async static Task VerifyTypeFail()
        {
            Pipe pipe = new Pipe();
            await Assert.ThrowsAsync<ArgumentException>(async () => await JsonSerializer.WriteAsync(1, typeof(string), pipe.Writer));
        }

        [Fact]
        public static async Task NullObjectValue()
        {
            Pipe pipe = new Pipe();
            await JsonSerializer.WriteAsync((object)null, pipe.Writer);
            pipe.Writer.Complete();

            bool success = pipe.Reader.TryRead(out ReadResult readResult);
            Assert.True(success);

            string value = Encoding.UTF8.GetString(readResult.Buffer.ToArray());
            Assert.Equal("null", value);
        }

        [Fact]
        public static async Task PipeRoundTripAsync()
        {
            LargeDataTestClass objOriginal = new LargeDataTestClass();
            objOriginal.Initialize();
            objOriginal.Verify();

            Pipe pipe = new Pipe(new PipeOptions(readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline));
            Task toTask = JsonSerializer.WriteAsync(objOriginal, pipe.Writer);
            ValueTask<LargeDataTestClass> fromTask = JsonSerializer.ReadAsync<LargeDataTestClass>(pipe.Reader);

            await toTask;
            pipe.Writer.Complete();

            LargeDataTestClass objCopy = await fromTask;
            pipe.Reader.Complete();

            objCopy.Verify();
        }

        [Fact]
        public static async Task PipePrimitivesAsync()
        {
            Pipe pipe = new Pipe(new PipeOptions(readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline));

            ValueTask<bool> task = JsonSerializer.ReadAsync<bool>(pipe.Reader);

            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(@"t"));
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(@"r"));
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(@"u"));
            await pipe.Writer.WriteAsync(Encoding.UTF8.GetBytes(@"e"));
            pipe.Writer.Complete();

            bool b = await task;

            Assert.Equal(true, b);
        }
    }
}

#endif
