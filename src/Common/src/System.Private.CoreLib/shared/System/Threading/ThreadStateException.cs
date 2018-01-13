// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
**
** Purpose: An exception class to indicate that the Thread class is in an
**          invalid state for the method.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System.Threading
{
    [Serializable]
    public class ThreadStateException : SystemException
    {
        public ThreadStateException()
            : base(SR.Arg_ThreadStateException)
        {
            HResult = __HResults.COR_E_THREADSTATE;
        }

        public ThreadStateException(String message)
            : base(message)
        {
            HResult = __HResults.COR_E_THREADSTATE;
        }

        public ThreadStateException(String message, Exception innerException)
            : base(message, innerException)
        {
            HResult = __HResults.COR_E_THREADSTATE;
        }

        protected ThreadStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
