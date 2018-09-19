// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Internal.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.Threading;
using Windows.Foundation;
using Windows.UI.Core;

namespace System.Threading
{
#if FEATURE_APPX
    [WindowsRuntimeImport]
    [Guid("DFA2DC9C-1A2D-4917-98F2-939AF1D6E0C8")]
    public delegate void DispatcherQueueHandler();

    public enum DispatcherQueuePriority
    {
        Low = -10,
        Normal = 0,
        High = 10
    }

    [ComImport]
    [WindowsRuntimeImport]
    [Guid("603E88E4-A338-4FFE-A457-A5CFB9CEB899")]
    internal interface IDispatcherQueue
    {
        // This returns a DispatcherQueueTimer but we don't use this method, so I
        // just made it 'object' to avoid declaring it.
        object CreateTimer();

        bool TryEnqueue(DispatcherQueueHandler callback);

        bool TryEnqueue(DispatcherQueuePriority priority, DispatcherQueueHandler callback);
    }

    #region class WinRTSynchronizationContextFactory

    internal sealed class WinRTSynchronizationContextFactory
    {
        //
        // It's important that we always return the same SynchronizationContext object for any particular ICoreDispatcher
        // object, as long as any existing instance is still reachable.  This allows reference equality checks against the
        // SynchronizationContext to determine if two instances represent the same dispatcher.  Async frameworks rely on this.
        // To accomplish this, we use a ConditionalWeakTable to track which instances of WinRTSynchronizationContext are bound
        // to each ICoreDispatcher instance.
        //
        private static readonly ConditionalWeakTable<CoreDispatcher, WinRTCoreDispatcherBasedSynchronizationContext> s_coreDispatcherContextCache =
            new ConditionalWeakTable<CoreDispatcher, WinRTCoreDispatcherBasedSynchronizationContext>();

        private static readonly ConditionalWeakTable<IDispatcherQueue, WinRTDispatcherQueueBasedSynchronizationContext> s_dispatcherQueueContextCache =
            new ConditionalWeakTable<IDispatcherQueue, WinRTDispatcherQueueBasedSynchronizationContext>();

        // System.Private.Corelib will call WinRTSynchronizationContextFactory.Create() via reflection
        public static SynchronizationContext Create(object dispatcherObj)
        {
            Debug.Assert(dispatcherObj != null);
            Debug.Assert(dispatcherObj is CoreDispatcher || dispatcherObj is IDispatcherQueue);

            if (dispatcherObj is CoreDispatcher)
            {
                //
                // Get the RCW for the dispatcher
                //
                CoreDispatcher dispatcher = (CoreDispatcher)dispatcherObj;

                //
                // The dispatcher is supposed to belong to this thread
                //
                Debug.Assert(dispatcher == CoreWindow.GetForCurrentThread().Dispatcher);
                Debug.Assert(dispatcher.HasThreadAccess);

                //
                // Get the WinRTSynchronizationContext instance that represents this CoreDispatcher.
                //
                return s_coreDispatcherContextCache.GetValue(dispatcher, _dispatcher => new WinRTCoreDispatcherBasedSynchronizationContext(_dispatcher));
            }
            else // dispatcherObj is IDispatcherQueue
            {
                //
                // Get the RCW for the dispatcher
                //
                IDispatcherQueue dispatcherQueue = (IDispatcherQueue)dispatcherObj;

                //
                // Get the WinRTSynchronizationContext instance that represents this IDispatcherQueue.
                //
                return s_dispatcherQueueContextCache.GetValue(dispatcherQueue, _dispatcherQueue => new WinRTDispatcherQueueBasedSynchronizationContext(_dispatcherQueue));
            }
        }
    }

    #endregion class WinRTSynchronizationContextFactory


    #region class WinRTSynchronizationContext

    internal sealed class WinRTCoreDispatcherBasedSynchronizationContext : WinRTSynchronizationContextBase
    {
        private readonly CoreDispatcher _dispatcher;
        internal WinRTCoreDispatcherBasedSynchronizationContext(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d));
            Contract.EndContractBlock();

            var ignored = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, new Invoker(d, state).Invoke);
        }

        public override SynchronizationContext CreateCopy()
        {
            return new WinRTCoreDispatcherBasedSynchronizationContext(_dispatcher);
        }
    }

    internal sealed class WinRTDispatcherQueueBasedSynchronizationContext : WinRTSynchronizationContextBase
    {
        private readonly IDispatcherQueue _dispatcherQueue;
        internal WinRTDispatcherQueueBasedSynchronizationContext(IDispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null)
                throw new ArgumentNullException(nameof(d));
            Contract.EndContractBlock();

            // We explicitly choose to ignore the return value here. This enqueue operation might fail if the 
            // dispatcher queue was shut down before we got here. In that case, we choose to just move on and
            // pretend nothing happened.
            var ignored = _dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, new Invoker(d, state).Invoke);
        }

        public override SynchronizationContext CreateCopy()
        {
            return new WinRTDispatcherQueueBasedSynchronizationContext(_dispatcherQueue);
        }
    }

    internal abstract class WinRTSynchronizationContextBase : SynchronizationContext
    {
        #region class WinRTSynchronizationContext.Invoker

        protected class Invoker
        {
            private readonly ExecutionContext _executionContext;
            private readonly SendOrPostCallback _callback;
            private readonly object _state;

            private static readonly ContextCallback s_contextCallback = new ContextCallback(InvokeInContext);

            public Invoker(SendOrPostCallback callback, object state)
            {
                _executionContext = ExecutionContext.Capture();
                _callback = callback;
                _state = state;
            }

            public void Invoke()
            {
                if (_executionContext == null)
                    InvokeCore();
                else
                    ExecutionContext.Run(_executionContext, s_contextCallback, this);
            }

            private static void InvokeInContext(object thisObj)
            {
                ((Invoker)thisObj).InvokeCore();
            }

            private void InvokeCore()
            {
                try
                {
                    _callback(_state);
                }
                catch (Exception ex)
                {
                    //
                    // If we let exceptions propagate to CoreDispatcher, it will swallow them with the idea that someone will
                    // observe them later using the IAsyncInfo returned by CoreDispatcher.RunAsync.  However, we ignore
                    // that IAsyncInfo, because there's nothing Post can do with it (since Post returns void).
                    // So, to avoid these exceptions being lost forever, we post them to the ThreadPool.
                    //
                    if (!(ex is ThreadAbortException))
                    {
                        if (!ExceptionSupport.ReportUnhandledError(ex))
                        {
                            var edi = ExceptionDispatchInfo.Capture(ex);
                            ThreadPool.QueueUserWorkItem(o => ((ExceptionDispatchInfo)o).Throw(), edi);
                        }
                    }
                }
            }
        }

        #endregion class WinRTSynchronizationContext.Invoker

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException(SR.InvalidOperation_SendNotSupportedOnWindowsRTSynchronizationContext);
        }
    }
    #endregion class WinRTSynchronizationContext
#endif //FEATURE_APPX
}  // namespace
