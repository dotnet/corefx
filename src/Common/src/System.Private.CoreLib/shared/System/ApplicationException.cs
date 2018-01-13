// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=============================================================================
**
**
**
** Purpose: The base class for all "less serious" exceptions that must be
**          declared or caught.
**
**
=============================================================================*/

using System.Runtime.Serialization;

namespace System
{
    // The ApplicationException is the base class for nonfatal, 
    // application errors that occur.  These exceptions are generated 
    // (i.e., thrown) by an application, not the Runtime. Applications that need 
    // to create their own exceptions do so by extending this class. 
    // ApplicationException extends but adds no new functionality to 
    // RecoverableException.
    // 
    [Serializable]
    public class ApplicationException : Exception
    {
        // Creates a new ApplicationException with its message string set to
        // the empty string, its HRESULT set to COR_E_APPLICATION, 
        // and its ExceptionInfo reference set to null. 
        public ApplicationException()
            : base(SR.Arg_ApplicationException)
        {
            HResult = __HResults.COR_E_APPLICATION;
        }

        // Creates a new ApplicationException with its message string set to
        // message, its HRESULT set to COR_E_APPLICATION, 
        // and its ExceptionInfo reference set to null. 
        // 
        public ApplicationException(String message)
            : base(message)
        {
            HResult = __HResults.COR_E_APPLICATION;
        }

        public ApplicationException(String message, Exception innerException)
            : base(message, innerException)
        {
            HResult = __HResults.COR_E_APPLICATION;
        }

        protected ApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
