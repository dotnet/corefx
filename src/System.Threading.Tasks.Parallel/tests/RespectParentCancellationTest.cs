// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//
// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// RespectParentCancellation.cs
//
//
// This file contains functional tests for Parallel loops with cancellation
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sytem.Threading.Tasks.Tests
{

    public sealed class RespectParentCancellationTest
    {
        private API _api;                                     // the API to be tested

        public RespectParentCancellationTest(API api)
        {
            _api = api;
        }

        /// <summary>
        /// This test cancels a Parallel.* loop in flight and checks that we get a OperationCanceledException.
        /// </summary>
        /// <returns></returns>
        internal void RealRun()
        {
            ParallelLoopResult result = new ParallelLoopResult();
            CancellationTokenSource cts = new CancellationTokenSource();
            var allowCancel = new ManualResetEventSlim();
            var wakeLoop = new ManualResetEventSlim();

            Action body = () =>
            {
                allowCancel.Set();
                wakeLoop.Wait();
            };

            Task wrappedTask = Task.Factory.StartNew(delegate
            {
                CancellationToken ct = cts.Token;
                switch (_api)
                {
                    case API.For:
                        result = Parallel.For(0, Int32.MaxValue, new ParallelOptions() { CancellationToken = ct }, (i) => body());
                        break;

                    case API.For64:
                        result = Parallel.For(0, Int64.MaxValue, new ParallelOptions() { CancellationToken = ct }, (i) => body());
                        break;

                    case API.Foreach:
                        result = Parallel.ForEach<int>(GetIEnumerable(), new ParallelOptions() { CancellationToken = ct }, (i) => body());
                        break;
                }
            }, cts.Token, TaskCreationOptions.None, TaskScheduler.Default);

            allowCancel.Wait();
            cts.Cancel();
            wakeLoop.Set();

            Assert.ThrowsAny<OperationCanceledException>(() => wrappedTask.GetAwaiter().GetResult());

            // verify result : Stopped <==> if Completed is false and LowestBreakIteration == null
            Assert.False(result.IsCompleted, "Should not be completed.");
            Assert.Null(result.LowestBreakIteration);
        }

        private IEnumerable<int> GetIEnumerable()
        {
            while (true)
            {
                yield return 0;
            }
        }

        public enum API
        {
            For,      // Parallel.For
            For64,    // Parallel.For64
            Foreach,  // Parallel.Foreach
        }

        public static class TestMethods
        {
            [Fact]
            public static void RespectParentCancellation1()
            {
                RespectParentCancellationTest test = new RespectParentCancellationTest(API.For);
                test.RealRun();
            }

            [Fact]
            public static void RespectParentCancellation2()
            {
                RespectParentCancellationTest test = new RespectParentCancellationTest(API.For64);
                test.RealRun();
            }

            [Fact]
            public static void RespectParentCancellation3()
            {
                RespectParentCancellationTest test = new RespectParentCancellationTest(API.Foreach);
                test.RealRun();
            }
        }
    }
}
