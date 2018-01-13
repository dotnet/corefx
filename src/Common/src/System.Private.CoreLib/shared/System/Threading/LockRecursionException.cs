// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace System.Threading
{
    public class LockRecursionException : System.Exception
    {
        public LockRecursionException()
        {
        }

        public LockRecursionException(string message)
            : base(message)
        {
        }

        public LockRecursionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LockRecursionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
