// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Threading
{
    internal sealed partial class ExecutionContext : System.IDisposable
    {
        static ThreadLocal<ExecutionContext> threadEc = new ThreadLocal<ExecutionContext>(() => new ExecutionContext());

        internal ExecutionContext() { }

        [System.Security.SecuritySafeCriticalAttribute]
        public static System.Threading.ExecutionContext Capture()
        {
            return new ExecutionContext();
        }

        [System.Security.SecuritySafeCriticalAttribute]
        public System.Threading.ExecutionContext CreateCopy()
        {
            return new ExecutionContext();
        }

        public void Dispose() { }

        private bool isFlowSuppressed = false;
        public static bool IsFlowSuppressed()
        {
            return threadEc.Value.isFlowSuppressed;
        }

        [System.Security.SecuritySafeCriticalAttribute]
        public static void RestoreFlow()
        {
            if (!IsFlowSuppressed())
            {
                throw new InvalidOperationException();
            }
            threadEc.Value.isFlowSuppressed = false;
        }

        [System.Security.SecurityCriticalAttribute]
        public static void Run(System.Threading.ExecutionContext executionContext, System.Threading.ContextCallback callback, object state)
        {
            // Minimal implementation of ExecutionContext flow to enable APM.
            callback(state);
        }

        [System.Security.SecurityCriticalAttribute]
        public static System.Threading.AsyncFlowControl SuppressFlow()
        {
            if (IsFlowSuppressed())
            {
                throw new InvalidOperationException();
            }

            threadEc.Value.isFlowSuppressed = true;
            return new AsyncFlowControl(threadEc.Value);
        }
    }

    internal struct AsyncFlowControl : IDisposable
    {
        private ExecutionContext ec;
        public AsyncFlowControl(ExecutionContext ec)
        {
            this.ec = ec;
        }

        public void Dispose()
        {
            if (this.ec != null)
            {
                this.ec.Dispose();
                this.ec = null;
            }
        }
    }

    internal delegate void ContextCallback(object state);
}
