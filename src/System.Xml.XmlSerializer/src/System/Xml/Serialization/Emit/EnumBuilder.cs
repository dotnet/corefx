// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class EnumBuilder : TypeInfo
    {
        private readonly ModuleBuilder _containingModule;
        private readonly string _name;

        internal EnumBuilder(ModuleBuilder containingModule, string name)
        {
            _containingModule = containingModule;
            _name = name;
        }

        public override bool Equals(Type other) => ReferenceEquals(this, other);
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

        public override Assembly Assembly => _containingModule.Assembly;
        public override Module Module => _containingModule;
        public override string Name => _name;

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
        public FieldBuilder UnderlyingField
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
        public TypeInfo CreateTypeInfo()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public FieldBuilder DefineLiteral(string literalName, object literalValue)
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
        public override Type MakePointerType()
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
        public override Type MakeGenericType(params Type[] typeArguments)
        {
            throw new NotImplementedException();
        }
    }
}
#endif