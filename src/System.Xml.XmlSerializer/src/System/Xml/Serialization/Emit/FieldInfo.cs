// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class FieldInfo : MemberInfo
    {
        [Obsolete("TODO", error: false)]
        public abstract FieldAttributes Attributes
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract Type FieldType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFamily
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFamilyAndAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsFamilyOrAssembly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsInitOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsLiteral
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPrivate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsSpecialName
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
        public static FieldInfo GetFieldFromHandle(RuntimeFieldHandle handle)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public static FieldInfo GetFieldFromHandle(RuntimeFieldHandle handle, RuntimeTypeHandle declaringType)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public abstract object GetValue(object obj);

        [Obsolete("TODO", error: false)]
        public void SetValue(object obj, object value)
        {
            throw new NotImplementedException();
        }
    }
}
#endif