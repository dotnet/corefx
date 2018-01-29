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
        private static readonly UnboundedChannelOptions UnboundedDefaultOptions = new UnboundedChannelOptions();
        private static readonly UnboundedChannelOptions UnboundedSingleReaderOptions = new UnboundedChannelOptions() { SingleReader = true };
        private static readonly UnboundedChannelOptions UnboundedSingleWriterOptions = new UnboundedChannelOptions() { SingleWriter = true };
        private static readonly UnboundedChannelOptions UnboundedSingleReaderWriterOptions =
                                    new UnboundedChannelOptions() { SingleReader = true, SingleWriter = true };

        private static readonly UnboundedChannelOptions UnboundedSynchronousOptions = new UnboundedChannelOptions() { AllowSynchronousContinuations = true };
        private static readonly UnboundedChannelOptions UnboundedSynchronousSingleReaderOptions =
                                    new UnboundedChannelOptions() { AllowSynchronousContinuations = true, SingleReader = true };
        private static readonly UnboundedChannelOptions UnboundedSynchronousSingleWriterOptions =
                                    new UnboundedChannelOptions() { AllowSynchronousContinuations = true, SingleWriter = true };
        private static readonly UnboundedChannelOptions UnboundedSynchronousSingleReaderWriterOptions =
                                    new UnboundedChannelOptions() { AllowSynchronousContinuations = true, SingleReader = true, SingleWriter = true };

        private const int DEFAULT_BOUNDED_CHANNEL_CAPACITY = 1000;
        private static readonly BoundedChannelOptions BoundedDefaultOptions = new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY);
        private static readonly BoundedChannelOptions BoundedSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { SingleReader = true, SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedSynchronousOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true };
        private static readonly BoundedChannelOptions BoundedSynchronousSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true, SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSynchronousSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true, SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSynchronousSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true, SingleReader = true, SingleWriter = true };

        private const int SMALL_BOUNDED_CHANNEL_CAPACITY = 1;
        private static readonly BoundedChannelOptions BoundedSmallCapacityOptions = new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY);
        private static readonly BoundedChannelOptions BoundedSmallCapacitySingleReaderOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSmallCapacitySingleWriterOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSmallCapacitySingleReaderWriterOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { SingleReader = true, SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedSynchronousSmallCapacityOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true };
        private static readonly BoundedChannelOptions BoundedSynchronousSmallCapacitySingleReaderOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true, SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSynchronousSmallCapacitySingleWriterOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true, SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSynchronousSmallCapacitySingleReaderWriterOptions =
                                    new BoundedChannelOptions(SMALL_BOUNDED_CHANNEL_CAPACITY) { AllowSynchronousContinuations = true, SingleReader = true, SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedDropNewestOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { FullMode = BoundedChannelFullMode.DropNewest };
        private static readonly BoundedChannelOptions BoundedDropNewestSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropNewest,
                                                        SingleReader = true };
        private static readonly BoundedChannelOptions BoundedDropNewestSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropNewest,
                                                        SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedDropNewestSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropNewest,
                                                        SingleReader = true,
                                                        SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedSynchronousDropNewestOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        AllowSynchronousContinuations = true,
                                                        FullMode = BoundedChannelFullMode.DropNewest };
        private static readonly BoundedChannelOptions BoundedSynchronousDropNewestSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropNewest,
                                            SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSynchronousDropNewestSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropNewest,
                                            SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSynchronousDropNewestSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropNewest,
                                            SingleReader = true,
                                            SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedDropOldestOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { FullMode = BoundedChannelFullMode.DropOldest };
        private static readonly BoundedChannelOptions BoundedDropOldestSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropOldest,
                                                        SingleReader = true };
        private static readonly BoundedChannelOptions BoundedDropOldestSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropOldest,
                                                        SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedDropOldestSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropOldest,
                                                        SingleReader = true,
                                                        SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedSynchronousDropOldestOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        AllowSynchronousContinuations = true,
                                                        FullMode = BoundedChannelFullMode.DropOldest };
        private static readonly BoundedChannelOptions BoundedSynchronousDropOldestSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropOldest,
                                            SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSynchronousDropOldestSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropOldest,
                                            SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSynchronousDropOldestSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropOldest,
                                            SingleReader = true,
                                            SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedDropWriteOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) { FullMode = BoundedChannelFullMode.DropWrite };
        private static readonly BoundedChannelOptions BoundedDropWriteSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropWrite,
                                                        SingleReader = true };
        private static readonly BoundedChannelOptions BoundedDropWriteSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropWrite,
                                                        SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedDropWriteSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        FullMode = BoundedChannelFullMode.DropWrite,
                                                        SingleReader = true,
                                                        SingleWriter = true };

        private static readonly BoundedChannelOptions BoundedSynchronousDropWriteOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                                        AllowSynchronousContinuations = true,
                                                        FullMode = BoundedChannelFullMode.DropWrite };
        private static readonly BoundedChannelOptions BoundedSynchronousDropWriteSingleReaderOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropWrite,
                                            SingleReader = true };
        private static readonly BoundedChannelOptions BoundedSynchronousDropWriteSingleWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropWrite,
                                            SingleWriter = true };
        private static readonly BoundedChannelOptions BoundedSynchronousDropWriteSingleReaderWriterOptions =
                                    new BoundedChannelOptions(DEFAULT_BOUNDED_CHANNEL_CAPACITY) {
                                            AllowSynchronousContinuations = true,
                                            FullMode = BoundedChannelFullMode.DropWrite,
                                            SingleReader = true,
                                            SingleWriter = true };

        private static readonly UnbufferedChannelOptions UnbufferedDefaultOptions = new UnbufferedChannelOptions();
        private static readonly UnbufferedChannelOptions UnbufferedSingleReaderOptions = new UnbufferedChannelOptions() { SingleReader = true };
        private static readonly UnbufferedChannelOptions UnbufferedSingleWriterOptions = new UnbufferedChannelOptions() { SingleWriter = true };
        private static readonly UnbufferedChannelOptions UnbufferedSingleReaderWriterOptions =
                                    new UnbufferedChannelOptions() { SingleReader = true, SingleWriter = true };

        private static readonly UnbufferedChannelOptions UnbufferedSynchronousOptions = new UnbufferedChannelOptions() { AllowSynchronousContinuations = true };
        private static readonly UnbufferedChannelOptions UnbufferedSynchronousSingleReaderOptions =
                                    new UnbufferedChannelOptions() { AllowSynchronousContinuations = true, SingleReader = true };
        private static readonly UnbufferedChannelOptions UnbufferedSynchronousSingleWriterOptions =
                                    new UnbufferedChannelOptions() { AllowSynchronousContinuations = true, SingleWriter = true };
        private static readonly UnbufferedChannelOptions UnbufferedSynchronousSingleReaderWriterOptions =
                                    new UnbufferedChannelOptions() { AllowSynchronousContinuations = true, SingleReader = true, SingleWriter = true };


        public static IEnumerable<object[]> TestData()
        {

            //
            // Unbounded Channel
            //
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedDefaultOptions), UnboundedDefaultOptions };
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSingleReaderOptions), UnboundedSingleReaderOptions };
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSingleReaderWriterOptions), UnboundedSingleReaderWriterOptions };
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSingleWriterOptions), UnboundedSingleWriterOptions };

            //
            // Unbounded Channel (AllowSynchronousContinuations)
            //
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSynchronousOptions), UnboundedSynchronousOptions };
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSynchronousSingleReaderOptions), UnboundedSynchronousSingleReaderOptions };
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSynchronousSingleReaderWriterOptions), UnboundedSynchronousSingleReaderWriterOptions };
            yield return new object[] { Channel.CreateUnbounded<int>(UnboundedSynchronousSingleWriterOptions), UnboundedSynchronousSingleWriterOptions };

            //
            // Bounded Channel DEFAULT_BOUNDED_CHANNEL_CAPACITY
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedDefaultOptions), BoundedDefaultOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSingleReaderOptions), BoundedSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSingleWriterOptions), BoundedSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSingleReaderWriterOptions), BoundedSingleReaderWriterOptions };

            //
            // Bounded Channel DEFAULT_BOUNDED_CHANNEL_CAPACITY (AllowSynchronousContinuations)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousOptions), BoundedSynchronousOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSingleReaderOptions), BoundedSynchronousSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSingleWriterOptions), BoundedSynchronousSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSingleReaderWriterOptions), BoundedSynchronousSingleReaderWriterOptions };

            //
            // Bounded Channel - small capacity
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedSmallCapacityOptions), BoundedSmallCapacityOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSmallCapacitySingleReaderOptions), BoundedSmallCapacitySingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSmallCapacitySingleWriterOptions), BoundedSmallCapacitySingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSmallCapacitySingleReaderWriterOptions), BoundedSmallCapacitySingleReaderWriterOptions };

            //
            // Bounded Channel - small capacity (AllowSynchronousContinuations)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSmallCapacityOptions), BoundedSynchronousSmallCapacityOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSmallCapacitySingleReaderOptions), BoundedSynchronousSmallCapacitySingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSmallCapacitySingleWriterOptions), BoundedSynchronousSmallCapacitySingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousSmallCapacitySingleReaderWriterOptions), BoundedSynchronousSmallCapacitySingleReaderWriterOptions };

            //
            // Bounded Channel (BoundedChannelFullMode.DropNewest)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropNewestOptions), BoundedDropNewestOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropNewestSingleReaderOptions), BoundedDropNewestSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropNewestSingleWriterOptions), BoundedDropNewestSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropNewestSingleReaderWriterOptions), BoundedDropNewestSingleReaderWriterOptions };

            //
            // Bounded Channel ( AllowSynchronousContinuations,  BoundedChannelFullMode.DropNewest)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropNewestOptions), BoundedSynchronousDropNewestOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropNewestSingleReaderOptions), BoundedSynchronousDropNewestSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropNewestSingleWriterOptions), BoundedSynchronousDropNewestSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropNewestSingleReaderWriterOptions), BoundedSynchronousDropNewestSingleReaderWriterOptions };

            //
            // Bounded Channel (BoundedChannelFullMode.DropOldest)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropOldestOptions), BoundedDropOldestOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropOldestSingleReaderOptions), BoundedDropOldestSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropOldestSingleWriterOptions), BoundedDropOldestSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropOldestSingleReaderWriterOptions), BoundedDropOldestSingleReaderWriterOptions };

            //
            // Bounded Channel ( AllowSynchronousContinuations,  BoundedChannelFullMode.DropOldest)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropOldestOptions), BoundedSynchronousDropOldestOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropOldestSingleReaderOptions), BoundedSynchronousDropOldestSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropOldestSingleWriterOptions), BoundedSynchronousDropOldestSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropOldestSingleReaderWriterOptions), BoundedSynchronousDropOldestSingleReaderWriterOptions };

            //
            // Bounded Channel (BoundedChannelFullMode.DropWrite)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropWriteOptions), BoundedDropWriteOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropWriteSingleReaderOptions), BoundedDropWriteSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropWriteSingleWriterOptions), BoundedDropWriteSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedDropWriteSingleReaderWriterOptions), BoundedDropWriteSingleReaderWriterOptions };

            //
            // Bounded Channel ( AllowSynchronousContinuations,  BoundedChannelFullMode.DropWrite)
            //
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropWriteOptions), BoundedSynchronousDropWriteOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropWriteSingleReaderOptions), BoundedSynchronousDropWriteSingleReaderOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropWriteSingleWriterOptions), BoundedSynchronousDropWriteSingleWriterOptions };
            yield return new object[] { Channel.CreateBounded<int>(BoundedSynchronousDropWriteSingleReaderWriterOptions), BoundedSynchronousDropWriteSingleReaderWriterOptions };

            //
            // Unbuffered Channel
            //
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedDefaultOptions), UnbufferedDefaultOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSingleReaderOptions), UnbufferedSingleReaderOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSingleReaderWriterOptions), UnbufferedSingleReaderWriterOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSingleWriterOptions), UnbufferedSingleWriterOptions };

            //
            // Unbuffered Channel (AllowSynchronousContinuations)
            //
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSynchronousOptions), UnbufferedSynchronousOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSynchronousSingleReaderOptions), UnbufferedSynchronousSingleReaderOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSynchronousSingleReaderWriterOptions), UnbufferedSynchronousSingleReaderWriterOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSynchronousSingleWriterOptions), UnbufferedSynchronousSingleWriterOptions };
            yield return new object[] { Channel.CreateUnbuffered<int>(UnbufferedSynchronousOptions), UnbufferedSynchronousOptions };
        }

        const int MAX_NUMBER_TO_WRITE_TO_CHANNEL = 1_000_000;
        private static readonly int MaxTaskCounts = Math.Max(2, Environment.ProcessorCount);

        [OuterLoop]
        [Theory]
        [MemberData(nameof(TestData))]
        public void RunInStressMode(Channel<int> channel, ChannelOptions options)
        {
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
            int wroteCount = 0;

            for (int i=0; i < readerTasksCount; i++)
            {
                taskList.Add(new Task(() => {
                    while (true)
                    {
                        try
                        {
                            if (!reader.TryRead(out int value))
                            {
                                value = reader.ReadAsync().GetAwaiter().GetResult();
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
            for (int i=0; i < writerTasksCount; i++)
            {
                taskList.Add(new Task(() => {
                    while (true)
                    {
                        int incrementedValue = Interlocked.Increment(ref numberToWriteToQueue);
                        if (incrementedValue >= MAX_NUMBER_TO_WRITE_TO_CHANNEL)
                        {
                            // we have reached the max we can write.
                            if (incrementedValue == MAX_NUMBER_TO_WRITE_TO_CHANNEL)
                            {
                                while (wroteCount < MAX_NUMBER_TO_WRITE_TO_CHANNEL)
                                {
                                    // we have some tasks is currently writing and not finished yet. wait them to finish.
                                    Task.Delay(100);
                                }

                                writer.Complete();
                            }
                            break;
                        }

                        if (!writer.TryWrite(incrementedValue))
                        {
                            writer.WriteAsync(incrementedValue).GetAwaiter().GetResult();
                        }
                        Interlocked.Increment(ref wroteCount);
                    }
                }));
            }

            foreach (Task t in taskList)
            {
                t.Start();
            }

            Task.WaitAll(taskList.ToArray());

            Assert.Equal(MAX_NUMBER_TO_WRITE_TO_CHANNEL, wroteCount);
            Assert.True(readCount <= MAX_NUMBER_TO_WRITE_TO_CHANNEL);

            if (shouldReadAllWrittenValues)
            {
                Assert.Equal(MAX_NUMBER_TO_WRITE_TO_CHANNEL, readCount);
            }

            // string message = $"{channel.GetType().Name}:  " +
            //                  $"SingleReader: {options.SingleReader}, " +
            //                  $"SingleWriter: {options.SingleWriter}, " +
            //                  $"AllowSynchronousContinuations: {options.AllowSynchronousContinuations}, " +
            //                  (boundedOptions == null ? "" : $"Mode: {boundedOptions.FullMode}, Capacity: {boundedOptions.Capacity},") +
            //                  $"read: {readCount}, " +
            //                  $"wrote: {wroteCount}, " +
            //                  $"Max: {MAX_NUMBER_TO_WRITE_TO_CHANNEL}";
            // Console.WriteLine(message);
        }
    }
}
