// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class ModuleBuilder : Module
    {
        private readonly AssemblyBuilder _containingAssembly;
        private readonly string _name;
        private readonly List<TypeBuilder> _types;

        public ModuleBuilder(AssemblyBuilder containingAssembly, string name)
        {
            _containingAssembly = containingAssembly;
            _name = name;
            _types = new List<TypeBuilder>();
        }

        public override Assembly Assembly => _containingAssembly;

        [Obsolete("TODO", error: false)]
        public override string FullyQualifiedName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public void CreateGlobalFunctions()
        {
                throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public EnumBuilder DefineEnum(string name, TypeAttributes visibility, Type underlyingType)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes, Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public TypeBuilder DefineType(string name)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public TypeBuilder DefineType(string name, TypeAttributes attr)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, int typesize)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packsize)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packingSize, int typesize)
        {
            throw new NotImplementedException();
        }

        public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            var type = new TypeBuilder(this, name, attr, parent, interfaces);
            _types.Add(type);
            return type;
        }

        [Obsolete("TODO", error: false)]
        public FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public MethodInfo GetArrayMethod(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
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
    }
}
#endif