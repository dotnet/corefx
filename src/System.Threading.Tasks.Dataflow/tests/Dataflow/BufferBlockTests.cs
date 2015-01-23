﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Threading.Tasks.Dataflow.Tests
{
    public class BufferBlockTests
    {
        [Fact]
        public void TestCtor()
        {
            var block = new BufferBlock<int>();
            Assert.Equal(expected: 0, actual: block.Count);
            Assert.False(block.Completion.IsCompleted);

            block = new BufferBlock<int>(new DataflowBlockOptions { MaxMessagesPerTask = 1 });
            Assert.Equal(expected: 0, actual: block.Count);
            Assert.False(block.Completion.IsCompleted);

            block = new BufferBlock<int>(new DataflowBlockOptions { MaxMessagesPerTask = 1, CancellationToken = new CancellationToken(true) });
            Assert.Equal(expected: 0, actual: block.Count);
        }

        [Fact]
        public void TestArgumentExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => new BufferBlock<int>(null));
            DataflowTestHelpers.TestArgumentsExceptions(new BufferBlock<int>());
        }

        [Fact]
        public void TestToString()
        {
            DataflowTestHelpers.TestToString(nameFormat =>
                nameFormat != null ?
                    new BufferBlock<int>(new DataflowBlockOptions() { NameFormat = nameFormat }) :
                    new BufferBlock<int>());
        }

        [Fact]
        public async Task TestOfferMessage()
        {
            var generators = new Func<BufferBlock<int>>[]
            {
                () => new BufferBlock<int>(),
                () => new BufferBlock<int>(new DataflowBlockOptions { BoundedCapacity = 10 }),
                () => new BufferBlock<int>(new DataflowBlockOptions { BoundedCapacity = 10, MaxMessagesPerTask = 1 })
            };
            foreach (var generator in generators)
            {
                DataflowTestHelpers.TestOfferMessage_ArgumentValidation(generator());

                var target = generator();
                DataflowTestHelpers.TestOfferMessage_AcceptsDataDirectly(target);
                int ignored;
                while (target.TryReceive(out ignored)) ;
                DataflowTestHelpers.TestOfferMessage_CompleteAndOffer(target);
                await target.Completion;

                target = generator();
                await DataflowTestHelpers.TestOfferMessage_AcceptsViaLinking(target);
                while (target.TryReceive(out ignored)) ;
                DataflowTestHelpers.TestOfferMessage_CompleteAndOffer(target);
                await target.Completion;
            }
        }

        [Fact]
        public void TestPost()
        {
            foreach (int boundedCapacity in new[] { DataflowBlockOptions.Unbounded, 1 })
            {
                var bb = new BufferBlock<int>(new DataflowBlockOptions { BoundedCapacity = boundedCapacity });
                Assert.True(bb.Post(0));
                bb.Complete();
                Assert.False(bb.Post(0));
            }
        }

        [Fact]
        public Task TestCompletionTask()
        {
            return DataflowTestHelpers.TestCompletionTask(() => new BufferBlock<int>());
        }

        [Fact]
        public async Task TestLinkToOptions()
        {
            const int Messages = 1;
            foreach (bool append in DataflowTestHelpers.BooleanValues)
            {
                var bb = new BufferBlock<int>();
                var values = new int[Messages];
                var targets = new ActionBlock<int>[Messages];
                for (int i = 0; i < Messages; i++)
                {
                    int slot = i;
                    targets[i] = new ActionBlock<int>(item => values[slot] = item);
                    bb.LinkTo(targets[i], new DataflowLinkOptions { MaxMessages = 1, Append = append });
                }

                for (int i = 0; i < Messages; i++) bb.Post(i);
                bb.Complete();
                await bb.Completion;

                for (int i = 0; i < Messages; i++)
                {
                    Assert.Equal(
                        expected: append ? i : Messages - i - 1,
                        actual: values[i]);
                }
            }
        }

        [Fact]
        public void TestReceives()
        {
            for (int test = 0; test < 3; test++)
            {
                var bb = new BufferBlock<int>();
                for (int i = 0; i < 5; i++)
                {
                    bb.Post(i);
                }

                int item;
                switch (test)
                {
                    case 0:
                        IList<int> items;
                        Assert.True(bb.TryReceiveAll(out items));
                        Assert.Equal(expected: 5, actual: items.Count);
                        for (int i = 0; i < items.Count; i++)
                        {
                            Assert.Equal(expected: i, actual: items[i]);
                        }
                        Assert.False(bb.TryReceiveAll(out items));
                        break;

                    case 1:
                        for (int i = 0; i < 5; i++)
                        {
                            Assert.True(bb.TryReceive(f => true, out item));
                            Assert.Equal(expected: i, actual: item);
                        }
                        Assert.False(bb.TryReceive(f => true, out item));
                        break;

                    case 2:
                        for (int i = 0; i < 5; i++)
                        {
                            Assert.False(bb.TryReceive(f => f == i + 1, out item));
                            Assert.True(bb.TryReceive(f => f == i, out item));
                            Assert.Equal(expected: i, actual: item);
                        }
                        Assert.False(bb.TryReceive(f => true, out item));
                        break;
                }
            }
        }

        [Fact]
        [OuterLoop] // waits for a period of time
        public async Task TestCircularLinking()
        {
            for (int boundedCapacity = 1; boundedCapacity <= 3; boundedCapacity++)
            {
                var b = new BufferBlock<int>(new DataflowBlockOptions { BoundedCapacity = boundedCapacity });
                for (int i = 0; i < boundedCapacity; i++)
                {
                    b.Post(i);
                }
                using (b.LinkTo(b))
                {
                    await Task.Delay(200);
                }
                Assert.Equal(expected: boundedCapacity, actual: b.Count);
            }
        }

        [Fact]
        public async Task TestProducerConsumer()
        {
            foreach (TaskScheduler scheduler in new[] { TaskScheduler.Default, new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler })
            foreach (int maxMessagesPerTask in new[] { DataflowBlockOptions.Unbounded, 1, 2 })
            foreach (int boundedCapacity in new[] { DataflowBlockOptions.Unbounded, 1, 2 })
            {
                const int Messages = 100;
                var bb = new BufferBlock<int>(new DataflowBlockOptions 
                {
                    BoundedCapacity = boundedCapacity,
                    MaxMessagesPerTask = maxMessagesPerTask,
                    TaskScheduler = scheduler
                });
                await Task.WhenAll(
                    Task.Run(async delegate { // consumer
                        int i = 0;
                        while (await bb.OutputAvailableAsync())
                        {
                            Assert.Equal(expected: i, actual: await bb.ReceiveAsync());
                            i++;
                        }
                    }),
                    Task.Run(async delegate { // producer
                        for (int i = 0; i < Messages; i++)
                        {
                            await bb.SendAsync(i);
                        }
                        bb.Complete();
                    }));
            }
        }

        [Fact]
        public async Task TestMessagePostponement()
        {
            foreach (int boundedCapacity in new[] { 1, 3 })
            {
                const int Excess = 10;
                var b = new BufferBlock<int>(new DataflowBlockOptions { BoundedCapacity = boundedCapacity });

                var sendAsync = new Task<bool>[boundedCapacity + Excess];
                for (int i = 0; i < boundedCapacity + Excess; i++) 
                    sendAsync[i] = b.SendAsync(i);
                b.Complete();

                for (int i = 0; i < boundedCapacity; i++)
                {
                    Assert.True(sendAsync[i].IsCompleted);
                    Assert.True(sendAsync[i].Result);
                }

                for (int i = 0; i < Excess; i++)
                {
                    Assert.False(await sendAsync[boundedCapacity + i]);
                }
            }
        }

        [Fact]
        public async Task TestReserveReleaseConsume()
        {
            var bb = new BufferBlock<int>();
            bb.Post(1);
            await DataflowTestHelpers.TestReserveAndRelease(bb);

            bb = new BufferBlock<int>();
            bb.Post(2);
            await DataflowTestHelpers.TestReserveAndConsume(bb);
        }

        [Fact]
        public void TestSourceCoreSpecifics()
        {
            var messageHeader = new DataflowMessageHeader(1);
            bool consumed;
            var block = new BufferBlock<int>();
            ((ITargetBlock<int>)block).OfferMessage(messageHeader, 42, null, false);

            var target = new ActionBlock<int>(i => { });
            Assert.True(((ISourceBlock<int>)block).ReserveMessage(messageHeader, target));
            ((ISourceBlock<int>)block).ReleaseReservation(messageHeader, target);

            ((ISourceBlock<int>)block).ConsumeMessage(messageHeader, DataflowBlock.NullTarget<int>(), out consumed);
            
            Assert.True(consumed);
            Assert.Equal(expected: 0, actual: block.Count);
        }

        [Fact]
        [OuterLoop] // has a timeout
        public async Task TestOutputAvailableAsyncAfterTryReceiveAll()
        {
            Func<Task<bool>> generator = () => {
                var buffer = new BufferBlock<object>();
                buffer.Post(null);

                IList<object> items;
                buffer.TryReceiveAll(out items);

                var outputAvailableAsync = buffer.OutputAvailableAsync();

                buffer.Post(null);

                return outputAvailableAsync;
            };

            var multipleConcurrentTestsTask = Task.WhenAll(Enumerable.Repeat(0, 1000).Select(_ => generator()));
            var timeoutTask = Task.Delay(2000);
            var completedTask = await Task.WhenAny(multipleConcurrentTestsTask, timeoutTask).ConfigureAwait(false);

            Assert.True(completedTask != timeoutTask);
        }

        [Fact]
        public async Task TestCountZeroAtCompletion()
        {
            var cts = new CancellationTokenSource();
            var buffer = new BufferBlock<int>(new DataflowBlockOptions() { CancellationToken = cts.Token });
            buffer.Post(1);
            cts.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => buffer.Completion);
            Assert.Equal(expected: 0, actual: buffer.Count);

            cts = new CancellationTokenSource();
            buffer = new BufferBlock<int>();
            buffer.Post(1);
            ((IDataflowBlock)buffer).Fault(new InvalidOperationException());
            await Assert.ThrowsAnyAsync<InvalidOperationException>(() => buffer.Completion);
            Assert.Equal(expected: 0, actual: buffer.Count);
        }

        [Fact]
        public void TestCount()
        {
            var bb = new BufferBlock<int>();
            for (int i = 1; i <= 10; i++)
            {
                bb.Post(i);
                Assert.Equal(expected: i, actual: bb.Count);
            }
            for (int i = 10; i > 0; i--)
            {
                int item;
                Assert.True(bb.TryReceive(out item));
                Assert.Equal(expected: 11 - i, actual: item);
                Assert.Equal(expected: i - 1, actual: bb.Count);
            }
        }

        [Fact]
        public async Task TestChainedSendReceive()
        {
            foreach (bool post in DataflowTestHelpers.BooleanValues)
            {
                const int Iters = 10;
                var network = DataflowTestHelpers.Chain<BufferBlock<int>, int>(4, () => new BufferBlock<int>());
                for (int i = 0; i < Iters; i++)
                {
                    if (post)
                    {
                        network.Post(i);
                    }
                    else
                    {
                        await network.SendAsync(i);
                    }
                    Assert.Equal(expected: i, actual: await network.ReceiveAsync());
                }
            }
        }

        [Fact]
        public async Task TestSendAllThenReceive()
        {
            foreach (bool post in DataflowTestHelpers.BooleanValues)
            {
                const int Iters = 10;
                var network = DataflowTestHelpers.Chain<BufferBlock<int>, int>(4, () => new BufferBlock<int>());

                if (post)
                {
                    for (int i = 0; i < Iters; i++)
                    {
                        network.Post(i);
                    }
                }
                else
                {
                    await Task.WhenAll(from i in Enumerable.Range(0, Iters) select network.SendAsync(i));
                }

                for (int i = 0; i < Iters; i++)
                {
                    Assert.Equal(expected: i, actual: await network.ReceiveAsync());
                }
            }
        }

        [Fact]
        public async Task TestPrecanceled()
        {
            var bb = new BufferBlock<int>(
                new DataflowBlockOptions { CancellationToken = new CancellationToken(canceled: true) });

            int ignoredValue;
            IList<int> ignoredValues;

            IDisposable link = bb.LinkTo(DataflowBlock.NullTarget<int>());
            Assert.NotNull(link);
            link.Dispose();

            Assert.False(bb.Post(42));
            var t = bb.SendAsync(42);
            Assert.True(t.IsCompleted);
            Assert.False(t.Result);
            Assert.Equal(expected: 0, actual: bb.Count);

            Assert.False(bb.TryReceiveAll(out ignoredValues));
            Assert.False(bb.TryReceive(out ignoredValue));

            Assert.NotNull(bb.Completion);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => bb.Completion);
            bb.Complete(); // just make sure it doesn't throw
        }

        [Fact]
        public async Task TestFaultingAndCancellation()
        {
            foreach (int boundedCapacity in new[] { DataflowBlockOptions.Unbounded, 1 })
            foreach (bool fault in DataflowTestHelpers.BooleanValues)
            {
                var cts = new CancellationTokenSource();
                var bb = new BufferBlock<int>(new DataflowBlockOptions { CancellationToken = cts.Token, BoundedCapacity = boundedCapacity });

                Task<bool>[] sends = Enumerable.Range(0, 4).Select(i => bb.SendAsync(i)).ToArray();
                Assert.Equal(expected: 0, actual: await bb.ReceiveAsync());
                Assert.Equal(expected: 1, actual: await bb.ReceiveAsync());

                if (fault)
                {
                    Assert.Throws<ArgumentNullException>(() => ((IDataflowBlock)bb).Fault(null));
                    ((IDataflowBlock)bb).Fault(new InvalidCastException());
                    await Assert.ThrowsAsync<InvalidCastException>(() => bb.Completion);
                }
                else
                {
                    cts.Cancel();
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(() => bb.Completion);
                }

                await Task.WhenAll(sends);
                Assert.Equal(expected: 0, actual: bb.Count);
            }
        }
    }
}
