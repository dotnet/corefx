// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.CompilerServices;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal abstract class GenericTypeParameterBuilder : TypeInfo
    {
        private sealed class MethodParameterBuilder : GenericTypeParameterBuilder
        {
            private readonly MethodBuilder _containingMethod;

            public MethodParameterBuilder(MethodBuilder containingMethod, string name, int index)
                : base(name, index)
            {
                _containingMethod = containingMethod;
            }

            public override Module Module => _containingMethod.Module;
        }

        private sealed class TypeParameterBuilder : GenericTypeParameterBuilder
        {
            private readonly TypeBuilder _containingType;

            public TypeParameterBuilder(TypeBuilder containingType, string name, int index)
                : base(name, index)
            {
                _containingType = containingType;
            }

            public override Module Module => _containingType.Module;
        }

        private readonly string _name;
        private readonly int _index;

        internal GenericTypeParameterBuilder(string name, int index)
        {
            _name = name;
            _index = index;
        }

        public override bool Equals(Type other) => ReferenceEquals(this, other);
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

        [Obsolete("TODO", error: false)]
        public override string AssemblyQualifiedName
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
        public override bool IsGenericParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _name;

        [Obsolete("TODO", error: false)]
        public override TypeAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Assembly Assembly => Module.Assembly;

        [Obsolete("TODO", error: false)]
        public override Type BaseType
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
        public override GenericParameterAttributes GenericParameterAttributes
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
        public override bool ContainsGenericParameters
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

        [Obsolete("TODO", error: false)]
        public override string Namespace
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
        public override Type GetGenericTypeDefinition()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override bool IsAssignableFrom(TypeInfo typeInfo)
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

        [Obsolete("TODO", error: false)]
        public void SetBaseTypeConstraint(Type baseTypeConstraint)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetGenericParameterAttributes(GenericParameterAttributes genericParameterAttributes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetInterfaceConstraints(params Type[] interfaceConstraints)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override bool IsSubclassOf(Type c)
        {
            throw new NotImplementedException();
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
    }
}
#endif