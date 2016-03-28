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
        public virtual bool IsArray
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
        public abstract Type GetElementType();
    }
}
#endif
