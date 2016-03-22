// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class Type : MemberInfo, IEquatable<Type>
    {
        public abstract bool Equals(Type other);
        public abstract override int GetHashCode();
        public sealed override bool Equals(object obj) => Equals(obj as Type);
        public static bool operator ==(Type x, Type y) => x.Equals(y);
        public static bool operator !=(Type x, Type y) => !x.Equals(y);

        public abstract TypeInfo GetTypeInfo();

        [Obsolete("TODO", error: false)]
        public abstract string AssemblyQualifiedName
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract string FullName
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract int GenericParameterPosition
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public abstract Type[] GenericTypeArguments
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool HasElementType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsArray
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsByRef
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsConstructedGenericType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract bool IsGenericParameter
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public bool IsNested
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public virtual bool IsPointer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract string Namespace
        {
            get;
        }

        [Obsolete("TODO", error: false)]
        public virtual RuntimeTypeHandle TypeHandle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public abstract int GetArrayRank();

        [Obsolete("TODO", error: false)]
        public abstract Type GetElementType();

        [Obsolete("TODO", error: false)]
        public abstract Type GetGenericTypeDefinition();

        [Obsolete("TODO", error: false)]
        public abstract Type MakeArrayType();

        [Obsolete("TODO", error: false)]
        public abstract Type MakeArrayType(int rank);

        [Obsolete("TODO", error: false)]
        public abstract Type MakeByRefType();

        [Obsolete("TODO", error: false)]
        public abstract Type MakeGenericType(params Type[] typeArguments);

        [Obsolete("TODO", error: false)]
        public abstract Type MakePointerType();
    }
}
#endif
