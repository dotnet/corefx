// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;

namespace System.Net
{
    // This class ensures that the user callback runs in the caller's ExecutionContext.
    internal partial class SimpleContextAwareResult : LazyAsyncResult
    {
        private ExecutionContext _context;

        internal SimpleContextAwareResult(object myObject, object myState, AsyncCallback myCallBack)
            : base(myObject, myState, myCallBack)
        {
            _context = ExecutionContext.Capture();
        }

        // This method is guaranteed to be called only once.
        protected override void Complete(IntPtr userToken)
        {
            if (CompletedSynchronously)
            {
                Debug.Assert(ExecutionContext.Capture() == _context);

                CompleteCallback();
            }
            else
            {
                ExecutionContext.Run(_context, s => ((SimpleContextAwareResult)s).CompleteCallback(), this);
            }
        }

        private void CompleteCallback()
        {
            base.Complete(IntPtr.Zero);
        }
    }
}
