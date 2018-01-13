// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace System.Resources
{
#if PROJECTN
    [Internal.Runtime.CompilerServices.RelocatedType("System.Resources.ResourceManager")]
#endif
    [Serializable]
    public class MissingManifestResourceException : SystemException
    {
        public MissingManifestResourceException()
            : base(SR.Arg_MissingManifestResourceException)
        {
            HResult = System.__HResults.COR_E_MISSINGMANIFESTRESOURCE;
        }

        public MissingManifestResourceException(string message)
            : base(message)
        {
            HResult = System.__HResults.COR_E_MISSINGMANIFESTRESOURCE;
        }

        public MissingManifestResourceException(string message, Exception inner)
            : base(message, inner)
        {
            HResult = System.__HResults.COR_E_MISSINGMANIFESTRESOURCE;
        }

        protected MissingManifestResourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
