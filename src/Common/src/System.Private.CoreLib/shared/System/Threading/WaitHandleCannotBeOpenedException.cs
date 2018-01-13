// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Threading
{
    [Serializable]
    public class WaitHandleCannotBeOpenedException : ApplicationException
    {
        public WaitHandleCannotBeOpenedException() : base(SR.Threading_WaitHandleCannotBeOpenedException)
        {
            HResult = __HResults.COR_E_WAITHANDLECANNOTBEOPENED;
        }

        public WaitHandleCannotBeOpenedException(String message) : base(message)
        {
            HResult = __HResults.COR_E_WAITHANDLECANNOTBEOPENED;
        }

        public WaitHandleCannotBeOpenedException(String message, Exception innerException) : base(message, innerException)
        {
            HResult = __HResults.COR_E_WAITHANDLECANNOTBEOPENED;
        }

        protected WaitHandleCannotBeOpenedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
