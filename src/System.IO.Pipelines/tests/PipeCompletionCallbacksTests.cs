// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace System.IO.Pipelines.Tests
{
    public class PipeCompletionCallbacksTests : IDisposable
    {
        public PipeCompletionCallbacksTests()
        {
            _pool = new TestMemoryPool();
        }

        public void Dispose()
        {
            _pool.Dispose();
        }

        private readonly TestMemoryPool _pool;

        private class TestScheduler : PipeScheduler
        {
            public int CallCount { get; set; }

            public Exception LastException { get; set; }

            public override void Schedule(Action<object> action, object state)
            {
                CallCount++;
                try
                {
                    action(state);
                }
                catch (Exception e)
                {
                    LastException = e;
                }
            }
        }

        [Fact]
        public void CompletingReaderFromWriterCallbackWorks()
        {
            var callbackRan = false;
            var pipe = new Pipe(new PipeOptions(_pool, pauseWriterThreshold: 5, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            pipe.Writer.RegisterReaderCompleted((exception, state) => { pipe.Writer.Complete(); }, null);

            pipe.Reader.RegisterWriterCompleted((exception, state) => { callbackRan = true; }, null);

            pipe.Reader.Complete();
            Assert.True(callbackRan);
        }

        [Fact]
        public void CompletingWriterFromReaderCallbackWorks()
        {
            var callbackRan = false;
            var pipe = new Pipe(new PipeOptions(_pool, pauseWriterThreshold: 5, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            pipe.Reader.RegisterWriterCompleted((exception, state) => { pipe.Reader.Complete(); }, null);

            pipe.Writer.RegisterReaderCompleted((exception, state) => { callbackRan = true; }, null);

            pipe.Writer.Complete();
            Assert.True(callbackRan);
        }

        [Fact]
        public void RegisterReaderCompletedContinuesOnException()
        {
            var callbackState1 = new object();
            var callbackState2 = new object();
            var exception1 = new Exception();
            var exception2 = new Exception();

            var counter = 0;

            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState1, state);
                    Assert.Equal(0, counter);
                    counter++;
                    throw exception1;
                }, callbackState1);

            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState2, state);
                    Assert.Equal(1, counter);
                    counter++;
                    throw exception2;
                }, callbackState2);

            var aggregateException = Assert.Throws<AggregateException>(() => pipe.Reader.Complete());
            Assert.Equal(exception1, aggregateException.InnerExceptions[0]);
            Assert.Equal(exception2, aggregateException.InnerExceptions[1]);
            Assert.Equal(2, counter);
        }

        [Fact]
        public void RegisterReaderCompletedExceptionSurfacesToWriterScheduler()
        {
            var exception = new Exception();
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, writerScheduler: scheduler, readerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.RegisterReaderCompleted((e, state) => throw exception, null);
            pipe.Reader.Complete();

            Assert.Equal(1, scheduler.CallCount);
            var aggregateException = Assert.IsType<AggregateException>(scheduler.LastException);
            Assert.Equal(aggregateException.InnerExceptions[0], exception);
        }

        [Fact]
        public void RegisterReaderCompletedExecutesOnSchedulerIfCompleted()
        {
            var callbackRan = false;
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, writerScheduler: scheduler, readerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.Complete();

            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Null(exception);
                    callbackRan = true;
                }, null);

            Assert.True(callbackRan);
            Assert.Equal(1, scheduler.CallCount);
        }

        [Fact]
        public void RegisterReaderCompletedIsDetachedDuringReset()
        {
            var callbackRan = false;
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, writerScheduler: scheduler, readerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.RegisterReaderCompleted((exception, state) => { callbackRan = true; }, null);

            pipe.Reader.Complete();
            pipe.Writer.Complete();
            pipe.Reset();

            Assert.True(callbackRan);
            callbackRan = false;

            pipe.Reader.Complete();
            pipe.Writer.Complete();

            Assert.Equal(1, scheduler.CallCount);
            Assert.False(callbackRan);
        }

        [Fact]
        public void RegisterReaderCompletedPassesException()
        {
            var callbackRan = false;
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            var readerException = new Exception();

            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    callbackRan = true;
                    Assert.Same(readerException, exception);
                }, null);
            pipe.Reader.Complete(readerException);

            Assert.True(callbackRan);
        }

        [Fact]
        public void RegisterReaderCompletedPassesState()
        {
            var callbackRan = false;
            var callbackState = new object();
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState, state);
                    callbackRan = true;
                }, callbackState);
            pipe.Reader.Complete();

            Assert.True(callbackRan);
        }

        [Fact]
        public void RegisterReaderCompletedRanBeforeFlushContinuation()
        {
            var callbackRan = false;
            var continuationRan = false;
            var pipe = new Pipe(new PipeOptions(_pool, pauseWriterThreshold: 5, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.False(continuationRan);
                    callbackRan = true;
                }, null);

            PipeWriter buffer = pipe.Writer.WriteEmpty(10);
            ValueTaskAwaiter<FlushResult> awaiter = buffer.FlushAsync().GetAwaiter();

            Assert.False(awaiter.IsCompleted);
            awaiter.OnCompleted(() => { continuationRan = true; });
            pipe.Reader.Complete();

            Assert.True(callbackRan);
            pipe.Writer.Complete();
        }

        [Fact]
        public void RegisterReaderCompletedRunsInRegistrationOrder()
        {
            var callbackState1 = new object();
            var callbackState2 = new object();
            var callbackState3 = new object();
            var counter = 0;

            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState1, state);
                    Assert.Equal(0, counter);
                    counter++;
                }, callbackState1);

            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState2, state);
                    Assert.Equal(1, counter);
                    counter++;
                }, callbackState2);

            pipe.Writer.RegisterReaderCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState3, state);
                    Assert.Equal(2, counter);
                    counter++;
                }, callbackState3);

            pipe.Reader.Complete();

            Assert.Equal(3, counter);
        }

        [Fact]
        public void RegisterReaderCompletedThrowsWithNullCallback()
        {
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            Assert.Throws<ArgumentNullException>(() => pipe.Writer.RegisterReaderCompleted(null, null));
        }

        [Fact]
        public void RegisterReaderCompletedUsingWriterScheduler()
        {
            var callbackRan = false;
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, writerScheduler: scheduler, readerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.RegisterReaderCompleted((exception, state) => { callbackRan = true; }, null);
            pipe.Reader.Complete();

            Assert.True(callbackRan);
            Assert.Equal(1, scheduler.CallCount);
        }

        [Fact]
        public void RegisterWriterCompletedContinuesOnException()
        {
            var callbackState1 = new object();
            var callbackState2 = new object();
            var exception1 = new Exception();
            var exception2 = new Exception();

            var counter = 0;

            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState1, state);
                    Assert.Equal(0, counter);
                    counter++;
                    throw exception1;
                }, callbackState1);

            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState2, state);
                    Assert.Equal(1, counter);
                    counter++;
                    throw exception2;
                }, callbackState2);

            var aggregateException = Assert.Throws<AggregateException>(() => pipe.Writer.Complete());
            Assert.Equal(exception1, aggregateException.InnerExceptions[0]);
            Assert.Equal(exception2, aggregateException.InnerExceptions[1]);
            Assert.Equal(2, counter);
        }

        [Fact]
        public void RegisterWriterCompletedExceptionSurfacesToReaderScheduler()
        {
            var exception = new Exception();
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, scheduler, PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.RegisterWriterCompleted((e, state) => throw exception, null);
            pipe.Writer.Complete();

            Assert.Equal(1, scheduler.CallCount);
            var aggregateException = Assert.IsType<AggregateException>(scheduler.LastException);
            Assert.Equal(aggregateException.InnerExceptions[0], exception);
        }

        [Fact]
        public void RegisterWriterCompletedExecutedSchedulerIfCompleted()
        {
            var callbackRan = false;
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, scheduler, PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Writer.Complete();

            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Null(exception);
                    callbackRan = true;
                }, null);

            Assert.True(callbackRan);
            Assert.Equal(1, scheduler.CallCount);
        }

        [Fact]
        public void RegisterWriterCompletedIsDetachedDuringReset()
        {
            var callbackRan = false;
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, scheduler, PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.RegisterWriterCompleted((exception, state) => { callbackRan = true; }, null);
            pipe.Reader.Complete();
            pipe.Writer.Complete();
            pipe.Reset();

            Assert.True(callbackRan);
            callbackRan = false;

            pipe.Reader.Complete();
            pipe.Writer.Complete();

            Assert.Equal(1, scheduler.CallCount);
            Assert.False(callbackRan);
        }

        [Fact]
        public void RegisterWriterCompletedPassesException()
        {
            var callbackRan = false;
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            var readerException = new Exception();

            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    callbackRan = true;
                    Assert.Same(readerException, exception);
                }, null);
            pipe.Writer.Complete(readerException);

            Assert.True(callbackRan);
        }

        [Fact]
        public void RegisterWriterCompletedPassesState()
        {
            var callbackRan = false;
            var callbackState = new object();
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState, state);
                    callbackRan = true;
                }, callbackState);
            pipe.Writer.Complete();

            Assert.True(callbackRan);
        }

        [Fact]
        public void RegisterWriterCompletedRanBeforeReadContinuation()
        {
            var callbackRan = false;
            var continuationRan = false;
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    callbackRan = true;
                    Assert.False(continuationRan);
                }, null);

            ValueTask<ReadResult> awaiter = pipe.Reader.ReadAsync();
            Assert.False(awaiter.IsCompleted);
            awaiter.GetAwaiter().OnCompleted(() => { continuationRan = true; });
            pipe.Writer.Complete();

            Assert.True(callbackRan);
        }

        [Fact]
        public void RegisterWriterCompletedRunsInRegistrationOrder()
        {
            var callbackState1 = new object();
            var callbackState2 = new object();
            var callbackState3 = new object();
            var counter = 0;

            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState1, state);
                    Assert.Equal(0, counter);
                    counter++;
                }, callbackState1);

            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState2, state);
                    Assert.Equal(1, counter);
                    counter++;
                }, callbackState2);

            pipe.Reader.RegisterWriterCompleted(
                (exception, state) =>
                {
                    Assert.Equal(callbackState3, state);
                    Assert.Equal(2, counter);
                    counter++;
                }, callbackState3);

            pipe.Writer.Complete();

            Assert.Equal(3, counter);
        }

        [Fact]
        public void RegisterWriterCompletedThrowsWithNullCallback()
        {
            var pipe = new Pipe(new PipeOptions(_pool, readerScheduler: PipeScheduler.Inline, writerScheduler: PipeScheduler.Inline, useSynchronizationContext: false));

            Assert.Throws<ArgumentNullException>(() => pipe.Reader.RegisterWriterCompleted(null, null));
        }

        [Fact]
        public void RegisterWriterCompletedUsingReaderScheduler()
        {
            var callbackRan = false;
            var scheduler = new TestScheduler();
            var pipe = new Pipe(new PipeOptions(_pool, scheduler, PipeScheduler.Inline, useSynchronizationContext: false));
            pipe.Reader.RegisterWriterCompleted((exception, state) => { callbackRan = true; }, null);
            pipe.Writer.Complete();

            Assert.True(callbackRan);
            Assert.Equal(1, scheduler.CallCount);
        }
    }
}
