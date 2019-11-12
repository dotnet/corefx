// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.ComponentModel.Tests
{
    public class AsyncOperationFinalizerTests
    {
        [Fact]
        public void Finalizer_OperationCompleted_DoesNotCallOperationCompleted()
        {
            RemoteExecutor.Invoke(() =>
            {
                Completed();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }).Dispose();
        }

        private void Completed()
        {
            // This is in a helper method to ensure the JIT doesn't artifically extend the lifetime of the operation.
            var tracker = new OperationCompletedTracker();
            AsyncOperationManager.SynchronizationContext = tracker;
            AsyncOperation operation = AsyncOperationManager.CreateOperation(new object());

            Assert.False(tracker.OperationDidComplete);
            operation.OperationCompleted();
            Assert.True(tracker.OperationDidComplete);
        }

        [Fact]
        public void Finalizer_OperationNotCompleted_CompletesOperation()
        {
            RemoteExecutor.Invoke(() =>
            {
                var tracker = new OperationCompletedTracker();
                NotCompleted(tracker);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.True(tracker.OperationDidComplete);
            }).Dispose();
        }

        private void NotCompleted(OperationCompletedTracker tracker)
        {
            // This is in a helper method to ensure the JIT doesn't artifically extend the lifetime of the operation.
            AsyncOperationManager.SynchronizationContext = tracker;
            AsyncOperation operation = AsyncOperationManager.CreateOperation(new object());
            Assert.False(tracker.OperationDidComplete);
        }

        public class OperationCompletedTracker : SynchronizationContext
        {
            public bool OperationDidComplete { get; set; }

            public override void OperationCompleted()
            {
                Assert.False(OperationDidComplete);
                OperationDidComplete = true;
            }
        }
    }
}
