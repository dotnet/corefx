// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class PropertyInfo : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public virtual MethodInfo GetMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract Type PropertyType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual MethodInfo SetMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif
