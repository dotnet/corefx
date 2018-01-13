// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
**
** Purpose: An exception class which is thrown into a thread to cause it to
**          abort. This is a special non-catchable exception and results in
**            the thread's death.  This is thrown by the VM only and can NOT be
**          thrown by any user thread, and subclassing this is useless.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System.Threading
{
    [Serializable]
    public sealed class ThreadAbortException : SystemException
    {
        private ThreadAbortException()
        {
            HResult = __HResults.COR_E_THREADABORTED;
        }

        internal ThreadAbortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public object ExceptionState => null;
    }
}
