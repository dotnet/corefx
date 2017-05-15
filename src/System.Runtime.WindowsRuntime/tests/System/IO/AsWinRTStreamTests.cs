// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Storage.Streams;
using Xunit;

namespace System.IO
{
    public class AsWinRTStreamTests
    {
        [Fact]
        public static void AsInputStream_FromReadOnlyStream()
        {
            Stream managedStream = TestStreamProvider.CreateReadOnlyStream();
            using (IInputStream ins = managedStream.AsInputStream())
            {
                Assert.NotNull(ins);

                // Adapting a read-only managed Stream to IOutputStream must throw a NotSupportedException
                Assert.Throws<NotSupportedException>(() => { IOutputStream outs = managedStream.AsOutputStream(); });
            }
        }

        [Fact]
        public static void AsOutputStream_FromWriteOnlyStream()
        {
            Stream managedStream = TestStreamProvider.CreateWriteOnlyStream();
            using (IOutputStream outs = managedStream.AsOutputStream())
            {
                Assert.NotNull(outs);

                // Adapting a write-only managed Stream to IInputStream must throw a NotSupportedException
                Assert.Throws<NotSupportedException>(() => { IInputStream ins = managedStream.AsInputStream(); });
            }
        }

        [Fact]
        public static void AsInputStream_WrapsToSameInstance()
        {
            Stream managedStream = TestStreamProvider.CreateReadOnlyStream();
            using (IInputStream ins = managedStream.AsInputStream())
            {
                Assert.NotNull(ins);
                Assert.Same(ins, managedStream.AsInputStream());
            }
        }

        [Fact]
        public static void AsOutputStream_WrapsToSameInstance()
        {
            Stream managedStream = TestStreamProvider.CreateWriteOnlyStream();
            using (IOutputStream outs = managedStream.AsOutputStream())
            {
                Assert.NotNull(outs);
                Assert.Same(outs, managedStream.AsOutputStream());
            }
        }

        [Fact]
        public static void AsInputStream_RoundtripUnwrap()
        {
            // NetFx Stream -> IInputStream -> NetFx Stream -> roundtrip reference equality is preserved
            Stream managedStream = TestStreamProvider.CreateReadOnlyStream();
            using (IInputStream ins = managedStream.AsInputStream())
            {
                Assert.Same(managedStream, ins.AsStreamForRead());
            }
        }

        [Fact]
        public static void AsOutputStream_RoundtripUnwrap()
        {
            // NetFx Stream -> IOutputStream -> NetFx Stream -> roundtrip reference equality is preserved
            Stream managedStream = TestStreamProvider.CreateWriteOnlyStream();
            using (IOutputStream outs = managedStream.AsOutputStream())
            {
                Assert.Same(managedStream, outs.AsStreamForWrite());
            }
        }

        [Fact]
        public static void AsInputStream_Equal()
        {
            Stream stream = TestStreamProvider.CreateReadOnlyStream();

            using (IInputStream insOne = stream.AsInputStream())
            using (IInputStream insTwo = stream.AsInputStream())
            {
                Assert.Equal(insOne, insTwo);
            }
        }

        [Fact]
        public static void AsInputStream_NotEqual()
        {
            Stream streamOne = TestStreamProvider.CreateReadOnlyStream();
            Stream streamTwo = TestStreamProvider.CreateReadOnlyStream();

            Assert.NotEqual(streamOne, streamTwo);

            using (IInputStream insOne = streamOne.AsInputStream())
            using (IInputStream insTwo = streamTwo.AsInputStream())
            {
                Assert.NotEqual(insOne, insTwo);
            }
        }

        [Fact]
        public static void AsOutputStream_Equal()
        {
            Stream stream = TestStreamProvider.CreateWriteOnlyStream();

            using (IOutputStream outsOne = stream.AsOutputStream())
            using (IOutputStream outsTwo = stream.AsOutputStream())
            {
                Assert.Equal(outsOne, outsOne);
            }
        }

        [Fact]
        public static void AsOutputStream_NotEqual()
        {
            Stream streamOne = TestStreamProvider.CreateWriteOnlyStream();
            Stream streamTwo = TestStreamProvider.CreateWriteOnlyStream();

            Assert.NotEqual(streamOne, streamTwo);

            using (IOutputStream outsOne = streamOne.AsOutputStream())
            using (IOutputStream outsTwo = streamTwo.AsOutputStream())
            {
                Assert.NotEqual(outsOne, outsTwo);
            }
        }

        [Fact]
        public static void TestRead_MemoryStream_None()
        {
            DoTestRead(() => { return TestStreamProvider.CreateMemoryStreamAsInputStream(); }, InputStreamOptions.None, false, true);
        }

        [Fact]
        public static void TestRead_MemoryStream_Partial()
        {
            DoTestRead(() => { return TestStreamProvider.CreateMemoryStreamAsInputStream(); }, InputStreamOptions.Partial, false, true);
        }

        [Fact]
        public static void TestWrite_MemoryStream()
        {
            DoTestWrite(() => { return TestStreamProvider.CreateMemoryStream(); }, false);
        }

        private static volatile bool s_progressCallbackInvoked = false;
        private static volatile bool s_completedCallbackInvoked = false;
        private static volatile IBuffer s_buffer;
        private static volatile uint s_resultBytesWritten = 0;

        private static void DoTestRead(Func<IInputStream> createStreamFunc, InputStreamOptions inputStreamOptions, bool mustInvokeProgressHandler, bool completesSynchronously)
        {

            IInputStream stream = createStreamFunc();
            s_buffer = WindowsRuntimeBuffer.Create(TestStreamProvider.ModelStreamLength);

            IAsyncOperationWithProgress<IBuffer, uint> readOp = stream.ReadAsync(s_buffer, (uint)TestStreamProvider.ModelStreamLength, inputStreamOptions);

            if (completesSynchronously)
            {
                // New readOp for a stream where we know that reading is sycnhronous must have Status = Completed
                Assert.Equal(AsyncStatus.Completed, readOp.Status);
            }
            else
            {
                // Note the race. By the tie we get here, the status of the op may be started or already completed.
                AsyncStatus readOpStatus = readOp.Status;
                Assert.True(readOpStatus == AsyncStatus.Completed || readOpStatus == AsyncStatus.Started, "New readOp must have Status = Started or Completed (race)");
            }

            s_progressCallbackInvoked = false;
            s_completedCallbackInvoked = false;

            uint readOpId = readOp.Id;
            EventWaitHandle waitHandle = new ManualResetEvent(false);

            readOp.Progress = (asyncReadOp, bytesCompleted) =>
            {

                s_progressCallbackInvoked = true;

                // asyncReadOp.Id in a progress callback must match the ID of the asyncReadOp to which the callback was assigned
                Assert.Equal(readOpId, asyncReadOp.Id);

                // asyncReadOp.Status must be 'Started' for a asyncReadOp in progress
                Assert.Equal(AsyncStatus.Started, asyncReadOp.Status);

                Assert.True(0 <= bytesCompleted && bytesCompleted <= (uint)TestStreamProvider.ModelStreamLength,
                    "bytesCompleted must be in range [0, maxBytesToRead] asyncReadOp in progress");
            };

            readOp.Completed = (asyncReadOp, passedStatus) =>
            {
                try
                {
                    s_completedCallbackInvoked = true;

                    // asyncReadOp.Id in a completion callback must match the ID of the asyncReadOp to which the callback was assigned
                    Assert.Equal(readOpId, asyncReadOp.Id);

                    // asyncReadOp.Status must match passedStatus for a completed asyncReadOp
                    Assert.Equal(passedStatus, asyncReadOp.Status);

                    // asyncReadOp.Status must be 'Completed' for a completed asyncReadOp
                    Assert.Equal(AsyncStatus.Completed, asyncReadOp.Status);

                    IBuffer resultBuffer = asyncReadOp.GetResults();

                    // asyncReadOp.GetResults() must not return null for a completed asyncReadOp
                    Assert.NotNull(resultBuffer);

                    Assert.True(0 < resultBuffer.Capacity,
                        "resultBuffer.Capacity when a completed asyncReadOp was expected to read more than zero bytes");

                    Assert.True(0 < resultBuffer.Length,
                        "resultBuffer.Length when a completed asyncReadOp was expected to read more than zero bytes");

                    Assert.True(resultBuffer.Length <= resultBuffer.Capacity,
                                           "resultBuffer.Length <= resultBuffer.Capacity is required for a completed asyncReadOp");

                    if (inputStreamOptions == InputStreamOptions.None)
                    {
                        // resultBuffer.Length must be equal to requested number of bytes when an asyncReadOp with
                        // InputStreamOptions.None completes successfully
                        Assert.Equal(resultBuffer.Length, (uint)TestStreamProvider.ModelStreamLength);
                    }

                    if (inputStreamOptions == InputStreamOptions.Partial)
                    {
                        Assert.True(resultBuffer.Length <= TestStreamProvider.ModelStreamLength,
                            "resultBuffer.Length must be smaller-or-equal to requested number of bytes when an asyncReadOp with"
                            + " InputStreamOptions.Partial completes successfully");
                    }
                    s_buffer = resultBuffer;
                }
                finally
                {
                    waitHandle.Set();
                }
            };

            // Now, let's block until the read op is complete.
            // We speculate that it will complete within 3500 msec, although under high load it may not be.
            // If the test fails we should use a better way to determine if callback is really not invoked, or if it's just too slow.
            waitHandle.WaitOne(500);
            waitHandle.WaitOne(1000);
            waitHandle.WaitOne(2000);

            if (mustInvokeProgressHandler)
            {
                Assert.True(s_progressCallbackInvoked,
                    "Progress callback specified to ReadAsync callback must be invoked when reading from this kind of stream");
            }

            Assert.True(s_completedCallbackInvoked,
                "Completion callback specified to ReadAsync callback must be invoked");

            // readOp.Status must be 'Completed' for a completed async readOp
            Assert.Equal(AsyncStatus.Completed, readOp.Status);

            Assert.True(0 < s_buffer.Capacity,
                "buffer.Capacity when a completed async readOp was expected to read more than zero bytes");

            Assert.True(0 < s_buffer.Length,
                "buffer.Length when a completed async readOp was expected to read more than zero bytes");

            Assert.True(s_buffer.Length <= s_buffer.Capacity,
                "buffer.Length <= buffer.Capacity is required for a completed async readOp");

            if (inputStreamOptions == InputStreamOptions.None)
            {
                // buffer.Length must be equal to requested number of bytes when an async readOp with
                //  InputStreamOptions.None completes successfully
                Assert.Equal((uint)TestStreamProvider.ModelStreamLength, s_buffer.Length);
            }

            if (inputStreamOptions == InputStreamOptions.Partial)
            {
                Assert.True(s_buffer.Length <= TestStreamProvider.ModelStreamLength,
                    "buffer.Length must be smaller-or-equal to requested number of bytes when an async readOp with"
                    + " InputStreamOptions.Partial completes successfully");
            }

            byte[] results = new byte[s_buffer.Length];
            s_buffer.CopyTo(0, results, 0, (int)s_buffer.Length);

            Assert.True(TestStreamProvider.CheckContent(results, 0, (int)s_buffer.Length),
                "Result data returned from AsyncRead must be the same as expected from the test data source");
        }

        private static void DoTestWrite(Func<Stream> createStreamFunc, bool mustInvokeProgressHandler)
        {
            Stream backingStream = createStreamFunc();
            using (IOutputStream stream = backingStream.AsOutputStream())
            {
                // Create test data
                Random rnd = new Random(20100720); //  Must be a different seed than used for TestStreamProvider.ModelStreamContents
                byte[] modelWriteData = new byte[0xA000];
                rnd.NextBytes(modelWriteData);

                // Start test

                IBuffer buffer = modelWriteData.AsBuffer();

                // ibuffer.Length for IBuffer created by Array.ToBuffer(void) must equal to array.Length
                Assert.Equal((uint)modelWriteData.Length, buffer.Length);

                // ibuffer.Capacity for IBuffer created by Array.ToBuffer(void) must equal to array.Length
                Assert.Equal((uint)modelWriteData.Length, buffer.Capacity);

                IAsyncOperationWithProgress<uint, uint> writeOp = stream.WriteAsync(buffer);

                // Note the race. By the tie we get here, the status of the op may be started or already completed.
                AsyncStatus writeOpStatus = writeOp.Status;
                Assert.True(writeOpStatus == AsyncStatus.Completed || writeOpStatus == AsyncStatus.Started, "New writeOp must have Status = Started or Completed (race)");

                uint writeOpId = writeOp.Id;
                s_progressCallbackInvoked = false;
                s_completedCallbackInvoked = false;

                EventWaitHandle waitHandle = new ManualResetEvent(false);

                writeOp.Progress = (asyncWriteOp, bytesCompleted) =>
                {
                    s_progressCallbackInvoked = true;

                    // asyncWriteOp.Id in a progress callback must match the ID of the asyncWriteOp to which the callback was assigned
                    Assert.Equal(writeOpId, asyncWriteOp.Id);

                    // asyncWriteOp.Status must be 'Started' for a asyncWriteOp in progress
                    Assert.Equal(AsyncStatus.Started, asyncWriteOp.Status);

                    Assert.True(0 <= bytesCompleted && bytesCompleted <= (uint)TestStreamProvider.ModelStreamLength,
                        "bytesCompleted must be in range [0, maxBytesToWrite] asyncWriteOp in progress");
                };

                writeOp.Completed = (asyncWriteOp, passedStatus) =>
                {
                    try
                    {
                        s_completedCallbackInvoked = true;

                        // asyncWriteOp.Id in a completion callback must match the ID of the asyncWriteOp to which the callback was assigned
                        Assert.Equal(writeOpId, asyncWriteOp.Id);

                        // asyncWriteOp.Status must match passedStatus for a completed asyncWriteOp
                        Assert.Equal(passedStatus, asyncWriteOp.Status);

                        // asyncWriteOp.Status must be 'Completed' for a completed asyncWriteOp
                        Assert.Equal(AsyncStatus.Completed, asyncWriteOp.Status);

                        uint bytesWritten = asyncWriteOp.GetResults();

                        // asyncWriteOp.GetResults() must return that all required bytes were written for a completed asyncWriteOp
                        Assert.Equal((uint)modelWriteData.Length, bytesWritten);

                        s_resultBytesWritten = bytesWritten;
                    }
                    finally
                    {
                        waitHandle.Set();
                    }
                };

                // Now, let's block until the write op is complete.
                // We speculate that it will complete within 3500 msec, although under high load it may not be.
                // If the test fails we should use a better way to determine if callback is really not invoked, or if it's just too slow.
                waitHandle.WaitOne(500);
                waitHandle.WaitOne(1000);
                waitHandle.WaitOne(2000);

                if (mustInvokeProgressHandler)
                {
                    Assert.True(s_progressCallbackInvoked,
                        "Progress callback specified to WriteAsync callback must be invoked when reading from this kind of stream");
                }

                Assert.True(s_completedCallbackInvoked, "Completion callback specified to WriteAsync callback must be invoked");

                // writeOp.Status must be 'Completed' for a completed async writeOp
                Assert.Equal(AsyncStatus.Completed, writeOp.Status);

                // writeOp.GetResults() must return that all required bytes were written for a completed async writeOp
                Assert.Equal((uint)modelWriteData.Length, s_resultBytesWritten);

                // Check contents

                backingStream.Seek(0, SeekOrigin.Begin);
                byte[] verifyBuff = new byte[modelWriteData.Length + 1024];

                int r = backingStream.Read(verifyBuff, 0, verifyBuff.Length);

                for (int i = 0; i < modelWriteData.Length; i++)
                {
                    Assert.Equal(modelWriteData[i], verifyBuff[i]);
                }
            }
        }
    }
}

