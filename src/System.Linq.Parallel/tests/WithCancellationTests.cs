// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
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

        // [Regression Test]
        // Use of the async channel can block both the consumer and producer threads.. before the cancellation work
        // these had no means of being awoken.
        //
        // However, only the producers need to wake up on cancellation as the consumer
        // will wake up once all the producers have gone away (via AsynchronousOneToOneChannel.SetDone())
        //
        // To specifically verify this test, we want to know that the Async channels were blocked in TryEnqueChunk before Dispose() is called
        //  -> this was verified manually, but is not simple to automate
        [Fact]
        [OuterLoop]  // explicit timeouts / delays
        public static void ChannelCancellation_ProducerBlocked()
        {
            Debug.WriteLine("PlinqCancellationTests.ChannelCancellation_ProducerBlocked()");

            Debug.WriteLine("        Query running (should be few seconds max)..");
            var query1 = Enumerable.Range(0, 100000000)  //provide 100million elements to ensure all the cores get >64K ints. Good up to 1600cores
                .AsParallel()
                .Select(x => x);
            var enumerator1 = query1.GetEnumerator();
            enumerator1.MoveNext();
            Task.Delay(1000).Wait();
            enumerator1.MoveNext();
            enumerator1.Dispose(); //can potentially hang

            Debug.WriteLine("        Done (success).");
        }

        // Plinq suppresses OCE(externalCT) occurring in worker threads and then throws a single OCE(ct)
        // if a manual OCE(ct) is thrown but ct is not canceled, Plinq should not suppress it, else things
        // get confusing...
        // ONLY an OCE(ct) for ct.IsCancellationRequested=true is co-operative cancellation
        [Fact]
        public static void OnlySuppressOCEifCTCanceled()
        {
            AggregateException caughtException = null;
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken externalToken = cts.Token;
            try
            {
                Enumerable.Range(1, 10).AsParallel()
                    .WithCancellation(externalToken)
                    .Select(
                      x =>
                      {
                          if (x % 2 == 0) throw new OperationCanceledException(externalToken);
                          return x;
                      }
                    )
                 .ToArray();
            }
            catch (AggregateException ae)
            {
                caughtException = ae;
            }

            Assert.NotNull(caughtException);
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

        // throwing a fake OCE(ct) when the ct isn't canceled should produce an AggregateException.
        [Fact]
        public static void SetOperationsThrowAggregateOnCancelOrDispose_2()
        {
            try
            {
                CancellationTokenSource cs = new CancellationTokenSource();
                var plinq = Enumerable.Range(0, 50)
                    .AsParallel().WithCancellation(cs.Token)
                    .WithDegreeOfParallelism(1)
                    .Union(Enumerable.Range(0, 10).AsParallel().Select<int, int>(x => { throw new OperationCanceledException(cs.Token); }));

                var walker = plinq.GetEnumerator();
                while (walker.MoveNext())
                {
                }
                Assert.True(false, string.Format("PlinqCancellationTests.SetOperationsThrowAggregateOnCancelOrDispose_2:  failed.  AggregateException was expected, but no exception occurred."));
            }
            catch (AggregateException)
            {
                // expected
            }
            catch (Exception e)
            {
                Assert.True(false, string.Format("PlinqCancellationTests.SetOperationsThrowAggregateOnCancelOrDispose_2.  failed.  AggregateException was expected, but some other exception occurred." + e.ToString()));
            }
        }

        // If a query is cancelled and immediately disposed, the dispose should not throw an OCE.
        [Fact]
        public static void CancelThenDispose()
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
    }
}
