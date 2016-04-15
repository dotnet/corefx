// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class LocalVariableInfo
    {
        [Obsolete("TODO", error: false)]
        public virtual bool IsPinned
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual int LocalIndex
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual Type LocalType
        {
            get
            {
                throw new InvalidOperationException();
            }
        }
    }
}
#endif