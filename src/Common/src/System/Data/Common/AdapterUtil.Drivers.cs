// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;

namespace System.Data.Common
{
    internal static partial class ADP
    {

        internal static Timer CreateGlobalTimer(TimerCallback callback, object state, int dueTime, int period)
        {
            // Don't capture the current ExecutionContext and its AsyncLocals onto 
            // a global timer causing them to live forever

            Timer timer;
            bool restoreFlow = false;
            try
            {
                if (!ExecutionContext.IsFlowSuppressed())
                {
                    ExecutionContext.SuppressFlow();
                    restoreFlow = true;
                }

                timer = new Timer(callback, state, dueTime, period);
            }
            finally
            {
                // Restore the current ExecutionContext
                if (restoreFlow)
                    ExecutionContext.RestoreFlow();
            }

            return timer;
        }
    }
}
