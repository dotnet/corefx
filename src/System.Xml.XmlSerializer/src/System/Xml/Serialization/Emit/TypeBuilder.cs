// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class TypeBuilder : TypeInfo
    {
        private readonly ModuleBuilder _containingModule;
        private readonly string _name;

        private readonly List<ConstructorBuilder> _ctors;
        private List<FieldBuilder> _lazyFields;
        private List<MethodBuilder> _lazyMethods; 
        private List<PropertyBuilder> _lazyProperties;

        internal TypeBuilder(ModuleBuilder containingModule, string name, TypeAttributes attr, Type parent, Type[] interfaces)
        {
            _containingModule = containingModule;
            _name = name;
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

        public override string Name => _name;

        public override Assembly Assembly => _containingModule.Assembly;

        [Obsolete("TODO", error: false)]
        public override Type BaseType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Module Module => _containingModule;

        [Obsolete("TODO", error: false)]
        public override bool ContainsGenericParameters
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

        public ConstructorBuilder DefineConstructor(MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes)
        {
            var result = new ConstructorBuilder(this, attributes, callingConvention, parameterTypes);
            _ctors.Add(result);
            return result;
        }

        public ConstructorBuilder DefineDefaultConstructor(MethodAttributes attributes)
        {
            var result = DefineConstructor(attributes, CallingConventions.Standard, Array.Empty<Type>());

#if TODO // emit call to parent ctor:
             ConstructorBuilder constBuilder;
 
            // get the parent class's default constructor
            // We really don't want(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic) here.  We really want
            // constructors visible from the subclass, but that is not currently
            // available in BindingFlags.  This more open binding is open to
            // runtime binding failures(like if we resolve to a private
            // constructor).
            ConstructorInfo con = null;
 
            if (m_typeParent is TypeBuilderInstantiation)
            {
                Type genericTypeDefinition = m_typeParent.GetGenericTypeDefinition();
 
                if (genericTypeDefinition is TypeBuilder)
                    genericTypeDefinition = ((TypeBuilder)genericTypeDefinition).m_bakedRuntimeType;
 
                if (genericTypeDefinition == null)
                    throw new NotSupportedException(Environment.GetResourceString("NotSupported_DynamicModule"));
 
                Type inst = genericTypeDefinition.MakeGenericType(m_typeParent.GetGenericArguments());
 
                if (inst is TypeBuilderInstantiation)
                    con = TypeBuilder.GetConstructor(inst, genericTypeDefinition.GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
                else                
                    con = inst.GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            }
 
            if (con == null)
            {
                con = m_typeParent.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            }
 
            if (con == null)
                throw new NotSupportedException(Environment.GetResourceString("NotSupported_NoParentDefaultConstructor"));
 
            // Define the constructor Builder
            constBuilder = DefineConstructor(attributes, CallingConventions.Standard, null);
            m_constructorCount++;
 
            // generate the code to call the parent's default constructor
            ILGenerator il = constBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call,con);
            il.Emit(OpCodes.Ret);
#endif

            return result;
        }

        public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
        {
            var field = new FieldBuilder(this, fieldName, type, attributes);

            if (_lazyFields == null)
            {
                _lazyFields.Add(field);
            }
            
            return field;
        }

        public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            var method = new MethodBuilder(this, name, attributes, returnType, parameterTypes);

            if (_lazyMethods == null)
            {
                _lazyMethods.Add(method);
            }

            return method;
        }

        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            var property = new PropertyBuilder(this, name, attributes, returnType, parameterTypes);

            if (_lazyProperties == null)
            {
                _lazyProperties.Add(property);
            }

            return property;
        }

        [Obsolete("TODO", error: false)]
        public override bool IsAssignableFrom(TypeInfo typeInfo)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }
    }
}
#endif