// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
    internal static class TaskExtensions
    {
        public static TaskCompletionSource<TResult> ToApm<TResult>(
            this Task<TResult> task,
            AsyncCallback callback,
            object state)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(completedTask => 
            {
                bool shouldInvokeCallback = false;

                if (completedTask.IsFaulted)
                {
                    shouldInvokeCallback = tcs.TrySetException(completedTask.Exception.InnerExceptions);
                }
                else if (completedTask.IsCanceled)
                {
                    shouldInvokeCallback = tcs.TrySetCanceled();
                }
                else
                {
                    shouldInvokeCallback = tcs.TrySetResult(completedTask.Result);
                }

                // Only invoke the callback if it exists AND we were able to transition the TCS
                // to the terminal state. If we couldn't transition the task it is because it was
                // already transitioned to a Canceled state via a previous HttpWebRequest.Abort() call.
                if (shouldInvokeCallback)
                {
                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                }
                else
                {
                    // Verify that the current status of the tcs.Task is 'Canceled'.  This
                    // occurred due to a previous call of tcs.TrySetCanceled() from the
                    // HttpWebRequest.Abort() method.
                    Debug.Assert(tcs.Task.IsCanceled);
                }
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return tcs;
        }

    }
}
