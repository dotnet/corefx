// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class TypeReference : TypeInfo, IEquatable<TypeReference>
    {
        private readonly Module _containingModule;
        private readonly TypeInfo _containingTypeOpt;
        private readonly Reflection.TypeInfo _info;

        internal System.Type SystemType => _info.AsType();

        internal TypeReference(Module containingModule, Reflection.TypeInfo info)
        {
            _containingTypeOpt = null;
            _containingModule = containingModule;
            _info = info;
        }

        internal TypeReference(TypeInfo containingType, Reflection.TypeInfo info)
        {
            _containingTypeOpt = containingType;
            _containingModule = containingType.Module;
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
        public override TypeAttributes Attributes
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
        public override MethodBase DeclaringMethod
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
        public override GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override int GenericParameterPosition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type[] GenericTypeArguments
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Guid GUID
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
        public override bool IsGenericParameter
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

        [Obsolete("TODO", error: false)]
        public override bool IsGenericTypeDefinition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool IsSerializable
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
        public override int GetArrayRank()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type[] GetGenericParameterConstraints()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type GetGenericTypeDefinition()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type MakeArrayType()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type MakeArrayType(int rank)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type MakeByRefType()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type MakeGenericType(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type MakePointerType()
        {
            throw new NotImplementedException();
        }
    }
}
#endif