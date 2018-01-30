// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Xunit;

namespace System.Threading.Channels.Tests
{
    public class StressTests
    {
        public static IEnumerable<object[]> TestData()
        {
            foreach (bool singleReader in new [] {false, true})
                foreach (bool singleWriter in new [] {false, true})
                    foreach (bool allowSynchronousContinuations in new [] {false, true})
                    {
                        Func<ChannelOptions, Channel<int>> unbounded = o => Channel.CreateUnbounded<int>((UnboundedChannelOptions)o);
                        yield return new object[] { unbounded, new UnboundedChannelOptions
                                                                    {
                                                                        SingleReader = singleReader,
                                                                        SingleWriter = singleWriter,
                                                                        AllowSynchronousContinuations = allowSynchronousContinuations
                                                                    }};
                    }

            for (BoundedChannelFullMode bco = BoundedChannelFullMode.Wait;  bco <= BoundedChannelFullMode.DropWrite; bco++)
                foreach (int capacity in new [] { 1, 1000 })
                    foreach (bool singleReader in new [] {false, true})
                        foreach (bool singleWriter in new [] {false, true})
                            foreach (bool allowSynchronousContinuations in new [] {false, true})
                            {
                                Func<ChannelOptions, Channel<int>> bounded = o => Channel.CreateBounded<int>((BoundedChannelOptions)o);
                                yield return new object[] { bounded, new BoundedChannelOptions(capacity)
                                                                        {
                                                                            SingleReader = singleReader,
                                                                            SingleWriter = singleWriter,
                                                                            AllowSynchronousContinuations = allowSynchronousContinuations,
                                                                            FullMode = bco
                                                                        }};
                            }

        }

        const int MaxNumberToWriteToChannel = 1_000_000;
        private static readonly int MaxTaskCounts = Math.Max(2, Environment.ProcessorCount);

        [OuterLoop]
        [Theory]
        [MemberData(nameof(TestData))]
        public void RunInStressMode(Func<ChannelOptions, Channel<int>> channelCreator, ChannelOptions options)
        {
            Channel<int> channel = channelCreator(options);
            ChannelReader<int> reader = channel.Reader;
            ChannelWriter<int> writer = channel.Writer;
            BoundedChannelOptions boundedOptions = options as BoundedChannelOptions;
            bool shouldReadAllWrittenValues = boundedOptions == null || boundedOptions.FullMode == BoundedChannelFullMode.Wait;

            List<Task> taskList = new List<Task>();

            int readerTasksCount;
            int writerTasksCount;

            if (options.SingleReader)
            {
                readerTasksCount = 1;
                writerTasksCount = options.SingleWriter ? 1 : MaxTaskCounts - 1;
            }
            else if (options.SingleWriter)
            {
                writerTasksCount = 1;
                readerTasksCount = MaxTaskCounts - 1;
            }
            else
            {
                readerTasksCount = MaxTaskCounts / 2;
                writerTasksCount = MaxTaskCounts - readerTasksCount;
            }

            int readCount = 0;

            for (int i=0; i < readerTasksCount; i++)
            {
                taskList.Add(Task.Run(async delegate
                {
                    while (true)
                    {
                        try
                        {
                            if (!reader.TryRead(out int value))
                            {
                                value = await reader.ReadAsync();
                            }
                            Interlocked.Increment(ref readCount);
                        }
                        catch (ChannelClosedException)
                        {
                            break;
                        }
                    }
                }));
            }

            int numberToWriteToQueue = -1;
            int remainingWriters = writerTasksCount;

            for (int i=0; i < writerTasksCount; i++)
            {
                taskList.Add(Task.Run(async delegate
                {
                    int num = Interlocked.Increment(ref numberToWriteToQueue);
                    while (num < MaxNumberToWriteToChannel)
                    {
                        await writer.WriteAsync(num);
                        num = Interlocked.Increment(ref numberToWriteToQueue);
                    }

                    if (Interlocked.Decrement(ref remainingWriters) == 0)
                        writer.Complete();
                }));
            }

            Task.WaitAll(taskList.ToArray());

            Assert.True(readCount <= MaxNumberToWriteToChannel);

            if (shouldReadAllWrittenValues)
            {
                Assert.Equal(MaxNumberToWriteToChannel, readCount);
            }

            // string message = $"{channel.GetType().Name}:  " +
            //                  $"SingleReader: {options.SingleReader}, " +
            //                  $"SingleWriter: {options.SingleWriter}, " +
            //                  $"AllowSynchronousContinuations: {options.AllowSynchronousContinuations}, " +
            //                  (boundedOptions == null ? "" : $"Mode: {boundedOptions.FullMode}, Capacity: {boundedOptions.Capacity},") +
            //                  $"read: {readCount}, " +
            //                  $"Max: {MaxNumberToWriteToChannel}";
            // Console.WriteLine(message);
        }
    }
}
