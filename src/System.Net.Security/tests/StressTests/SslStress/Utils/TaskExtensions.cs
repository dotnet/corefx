// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SslStress.Utils
{
    public static class TaskExtensions
    {
        // Starts and awaits a collection of tasks while ensuring cancellation has been signaled
        // whenever one of them has raised an unhandled exception
        public static async Task WhenAllCancelOnFirstException(CancellationToken token, params Func<CancellationToken, Task>[] tasks)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            await Task.WhenAll(tasks.Select(RunOne));

            async Task RunOne(Func<CancellationToken, Task> task)
            {
                try
                {
                    await Task.Run(() => task(cts.Token));
                }
                catch
                {
                    cts.Cancel();
                    throw;
                }
            }
        }
    }
}
