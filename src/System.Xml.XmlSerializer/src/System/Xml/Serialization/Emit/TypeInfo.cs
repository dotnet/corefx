// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class TypeInfo : Type
    {
        public override TypeInfo GetTypeInfo() => this;

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<FieldInfo> DeclaredFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual IEnumerable<PropertyInfo> DeclaredProperties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public abstract Assembly Assembly { get; }

        [Obsolete("TODO", error: false)]
        public abstract Type BaseType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool ContainsGenericParameters
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsEnum
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsGenericType
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsInterface
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsNestedPublic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public bool IsPrimitive
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
        public bool IsValueType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsAssignableFrom(TypeInfo typeInfo)
        {
            throw new NotImplementedException();
        }
    }
}
#endif