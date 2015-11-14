// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Threading.Tasks.Tests
{
    internal class ValidateCorrectContextSynchronizationContext : SynchronizationContext
    {
        [ThreadStatic]
        internal static bool IsPostedInContext;

        internal int PostCount;
        internal int SendCount;

        public override void Post(SendOrPostCallback d, object state)
        {
            Interlocked.Increment(ref PostCount);
            Task.Run(() =>
            {
                IsPostedInContext = true;
                d(state);
                IsPostedInContext = false;
            });
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            Interlocked.Increment(ref SendCount);
            d(state);
        }
    }
}
