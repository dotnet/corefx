// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
**
** Purpose: Exception class for denoting an object was in a state that
** made calling a method illegal.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System
{
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class InvalidOperationException : SystemException
    {
        public InvalidOperationException()
            : base(SR.Arg_InvalidOperationException)
        {
            HResult = HResults.COR_E_INVALIDOPERATION;
        }

        public InvalidOperationException(string message)
            : base(message)
        {
            HResult = HResults.COR_E_INVALIDOPERATION;
        }

        public InvalidOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = HResults.COR_E_INVALIDOPERATION;
        }

        protected InvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
