// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Xunit;

namespace System.ComponentModel.Tests
{
    public class AsyncOperationFinalizerTests : RemoteExecutorTestBase
    {
        [Fact]
        public void Finalizer_GetViaReflection_IsPresentForCompatability()
        {
            MethodInfo finalizer = typeof(AsyncOperation).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(finalizer);
        }

        [Fact]
        public void Finalizer_OperationCompleted_DoesNotCallOperationCompleted()
        {
            RemoteInvoke(() =>
            {
                MethodInfo finalizer = typeof(AsyncOperation).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(finalizer);

                var tracker = new OperationCompletedTracker();
                AsyncOperationManager.SynchronizationContext = tracker;
                AsyncOperation operation = AsyncOperationManager.CreateOperation(new object());

                Assert.Equal(0, tracker.OperationCompletedCounter);
                operation.OperationCompleted();
                Assert.Equal(1, tracker.OperationCompletedCounter);
                
                finalizer.Invoke(operation, null);
                Assert.Equal(1, tracker.OperationCompletedCounter);

                return SuccessExitCode;
            }).Dispose();
        }

        [Fact]
        public void Finalizer_OperationCompleted_CompletesOperation()
        {
            RemoteInvoke(() =>
            {
                MethodInfo finalizer = typeof(AsyncOperation).GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.NotNull(finalizer);

                var tracker = new OperationCompletedTracker();
                AsyncOperationManager.SynchronizationContext = tracker;
                AsyncOperation operation = AsyncOperationManager.CreateOperation(new object());

                Assert.Equal(0, tracker.OperationCompletedCounter);
                finalizer.Invoke(operation, null);
                Assert.Equal(1, tracker.OperationCompletedCounter);

                return SuccessExitCode;
            }).Dispose();
        }

        public class OperationCompletedTracker : SynchronizationContext
        {
            public int OperationCompletedCounter { get; set; }

            public override void OperationCompleted() => OperationCompletedCounter++;
        }
    }
}
