// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class MethodBase : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public bool IsPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsStatic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsVirtual
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract ParameterInfo[] GetParameters();

        [Obsolete("TODO", error: false)]
        public object Invoke(object obj, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
#endif