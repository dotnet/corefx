// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection.Emit;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class TypeReference : TypeInfo, IEquatable<TypeReference>
    {
        private readonly Module _containingModule;
        private readonly Reflection.TypeInfo _info;

        internal System.Type SystemType => _info.AsType();

        internal TypeReference(Module containingModule, Reflection.TypeInfo info)
        {
            _containingModule = containingModule;
            _info = info;
        }

        public bool Equals(TypeReference other) => _info.Equals(other?._info);
        public override bool Equals(Type other) => Equals(other as TypeReference);
        public override int GetHashCode() => _info.GetHashCode();
        public override string ToString() => _info.ToString();

        public override Module Module => _containingModule;
        public override Assembly Assembly => _containingModule.Assembly;

        [Obsolete("TODO", error: false)]
        public override string AssemblyQualifiedName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type BaseType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool ContainsGenericParameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override string FullName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool IsEnum
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool IsGenericType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _info.Name;

        [Obsolete("TODO", error: false)]
        public override string Namespace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Type MakeArrayType()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Type MakeByRefType()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public virtual Type MakeGenericType(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }
}
#endif