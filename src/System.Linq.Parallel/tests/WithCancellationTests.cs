// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    // a key part of cancellation testing is 'promptness'.  Those tests appear in pfxperfunittests.
    // the tests here are only regarding basic API correctness and sanity checking.
    public static class WithCancellationTests
    {
        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 16 }, MemberType = typeof(Sources))]
        public static void WithCancellation_Multiple_NotCancelable(Labeled<ParallelQuery<int>> labeled, int count)
        {
            // Multiple not-cancel-able tokens is not an error.
            labeled.Item.WithCancellation(new CancellationToken()).WithCancellation(new CancellationToken());
            CancellationToken token = new CancellationToken();
            labeled.Item.WithCancellation(token).WithCancellation(token);
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 16 }, MemberType = typeof(Sources))]
        public static void WithCancellation_Multiple_CancelableToken(Labeled<ParallelQuery<int>> labeled, int count)
        {
            CancellationToken token = new CancellationTokenSource().Token;
            Assert.Throws<InvalidOperationException>(() => labeled.Item.WithCancellation(token).WithCancellation(token));
            Assert.Throws<InvalidOperationException>(() => labeled.Item.WithCancellation(token).WithCancellation(new CancellationTokenSource().Token));
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 16 }, MemberType = typeof(Sources))]
        public static void WithCancellation_PreCanceled(Labeled<ParallelQuery<int>> labeled, int count)
        {
            // Anticipation is the query will cancel soon after starting.
            CancellationTokenSource source = new CancellationTokenSource();
            source.Cancel();
            labeled.Item.WithCancellation(source.Token);
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 16 }, MemberType = typeof(Sources))]
        public static void WithCancellation_NotCancelable(Labeled<ParallelQuery<int>> labeled, int count)
        {
            labeled.Item.WithCancellation(new CancellationToken(true));
            labeled.Item.WithCancellation(new CancellationToken(false));
        }

        [Theory]
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 16 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 16 }, MemberType = typeof(UnorderedSources))]
        public static void WithCancellation_DisposedEnumerator(Labeled<ParallelQuery<int>> labeled, int count)
        {
            // Disposing an enumerator should throw ODE and not OCE.
            ParallelQuery<int> query = labeled.Item;
            DisposedEnumerator(query);
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.Default));
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.AutoBuffered));
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.FullyBuffered));
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.NotBuffered));
        }

        // [Regression Test]
        // Use of the async channel can block both the consumer and producer threads.. before the cancellation work
        // these had no means of being awoken.
        //
        // However, only the producers need to wake up on cancellation as the consumer
        // will wake up once all the producers have gone away (via AsynchronousOneToOneChannel.SetDone())
        //
        // To specifically verify this test, check that the Async channels were blocked in TryEnqueChunk before Dispose() is called
        //  -> this was verified manually, but is not simple to automate
        [Theory]
        [OuterLoop] // explicit timeouts / delays
        // Provide enough elements to ensure all the cores get >64K ints. Good up to 1000 cores
        [MemberData(nameof(Sources.Ranges), new[] { 1024 * 16 }, MemberType = typeof(Sources))]
        [MemberData(nameof(UnorderedSources.Ranges), new[] { 1024 * 16 }, MemberType = typeof(UnorderedSources))]
        public static void WithCancellation_DisposedEnumerator_ChannelCancellation_ProducerBlocked(Labeled<ParallelQuery<int>> labeled, int count)
        {
            ParallelQuery<int> query = labeled.Item;
            // Larger size, delay may cause enumerator.Dispose() to hang
            DisposedEnumerator(query, true);
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.Default), true);
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.AutoBuffered), true);
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.FullyBuffered), true);
            DisposedEnumerator(query.WithMergeOptions(ParallelMergeOptions.NotBuffered), true);
        }

        // a specific repro where inner queries would see an ODE on the merged cancellation token source
        // when the implementation involved disposing and recreating the token on each worker thread
        [Fact]
        public static void Cancellation_ODEIssue()
        {
            AggregateException caughtException = null;
            try
            {
                Enumerable.Range(0, 1999).ToArray()
                .AsParallel().AsUnordered()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Zip<int, int, int>(
                    Enumerable.Range(1000, 20).Select<int, int>(_item => (int)_item).AsParallel().AsUnordered(),
                    (first, second) => { throw new OperationCanceledException(); })
               .ForAll(x => { });
            }
            catch (AggregateException ae)
            {
                caughtException = ae;
            }

            //the failure was an ODE coming out due to an ephemeral disposed merged cancellation token source.
            Assert.True(caughtException != null,
                                              "Cancellation_ODEIssue:  We expect an aggregate exception with OCEs in it.");
        }

        // If a query is canceled and immediately disposed, the dispose should not throw an OCE.
        [Theory]
        [MemberData("Ranges", (object)(new int[] { 16 }), MemberType = typeof(Sources))]
        public static void WithCancellation_CancelThenDispose(Labeled<ParallelQuery<int>> labeled, int count)
        {
            try
            {
                CancellationTokenSource cancel = new CancellationTokenSource();
                var q = ParallelEnumerable.Range(0, 1000).WithCancellation(cancel.Token).Select(x => x);
                IEnumerator<int> e = q.GetEnumerator();
                e.MoveNext();

                cancel.Cancel();
                e.Dispose();
            }
            catch (Exception e)
            {
                Assert.True(false, string.Format("PlinqCancellationTests.CancelThenDispose:  > Failed. Expected no exception, got " + e.GetType()));
            }
        }

        [Fact]
        public static void DontDoWorkIfTokenAlreadyCanceled()
        {
            OperationCanceledException oce = null;

            CancellationTokenSource cs = new CancellationTokenSource();
            var query = Enumerable.Range(0, 100000000)
            .Select(x =>
            {
                if (x > 0) // to avoid the "Error:unreachable code detected"
                    throw new ArgumentException("User-delegate exception.");
                return x;
            })
            .AsParallel()
            .WithCancellation(cs.Token)
            .Select(x => x);

            cs.Cancel();
            try
            {
                foreach (var item in query) //We expect an OperationCancelledException during the MoveNext
                {
                }
            }
            catch (OperationCanceledException ex)
            {
                oce = ex;
            }

            Assert.NotNull(oce);
        }

        private static void DisposedEnumerator(ParallelQuery<int> query, bool delay = false)
        {
            query = query.WithCancellation(new CancellationTokenSource().Token).Select(x => x);

            IEnumerator<int> enumerator = query.GetEnumerator();

            enumerator.MoveNext();
            if (delay) Task.Delay(1000).Wait();
            enumerator.MoveNext();
            enumerator.Dispose();

            Assert.Throws<ObjectDisposedException>(() => enumerator.MoveNext());
        }
    }
}
