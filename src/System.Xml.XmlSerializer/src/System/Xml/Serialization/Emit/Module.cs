// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class Module
    {
        public abstract Assembly Assembly { get; }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual string FullyQualifiedName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif