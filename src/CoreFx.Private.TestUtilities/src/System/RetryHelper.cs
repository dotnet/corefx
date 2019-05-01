// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System
{
    public static partial class RetryHelper
    {
        private static readonly Func<int, int> s_defaultBackoffFunc = i => i * 100;

        /// <summary>Executes the <paramref name="test"/> action up to a maximum of <paramref name="maxAttempts"/> times.</summary>
        /// <param name="maxAttempts">The maximum number of times to invoke <paramref name="test"/>.</param>
        /// <param name="test">The test to invoke.</param>
        /// <param name="backoffFunc">After a failure, invoked to determine how many milliseconds to wait before the next attempt.  It's passed the number of iterations attempted.</param>
        public static void Execute(int maxAttempts, Action test, Func<int, int> backoffFunc = null)
        {
            if (maxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            }
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            for (int i = 1; i <= maxAttempts; i++)
            {
                try
                {
                    test();
                    return;
                }
                catch when (i < maxAttempts) { }
                Thread.Sleep((backoffFunc ?? s_defaultBackoffFunc)(i));
            }
        }

        /// <summary>Executes the <paramref name="test"/> action up to a maximum of <paramref name="maxAttempts"/> times.</summary>
        /// <param name="maxAttempts">The maximum number of times to invoke <paramref name="test"/>.</param>
        /// <param name="test">The test to invoke.</param>
        /// <param name="backoffFunc">After a failure, invoked to determine how many milliseconds to wait before the next attempt.  It's passed the number of iterations attempted.</param>
        public static async Task ExecuteAsync(int maxAttempts, Func<Task> test, Func<int, int> backoffFunc = null)
        {
            if (maxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            }
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            for (int i = 1; i <= maxAttempts; i++)
            {
                try
                {
                    await test().ConfigureAwait(false);
                    return;
                }
                catch when (i < maxAttempts) { }
                await Task.Delay((backoffFunc ?? s_defaultBackoffFunc)(i)).ConfigureAwait(false);
            }
        }
    }
}
