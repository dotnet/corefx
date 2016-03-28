// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class FieldInfo : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public abstract Type FieldType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsStatic
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif