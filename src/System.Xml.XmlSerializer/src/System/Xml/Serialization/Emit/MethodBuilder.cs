// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class MethodBuilder : MethodInfo
    {
        private readonly TypeBuilder _containingType;
        private readonly string _name;
        private readonly MethodAttributes _attributes;
        private readonly Type _returnType;
        private readonly Type[] _parameterTypes;

        public MethodBuilder(TypeBuilder containingType, string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            _containingType = containingType;
            _name = name;
            _attributes = attributes;
            _returnType = returnType;
            _parameterTypes = parameterTypes;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override MethodAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override CallingConventions CallingConvention
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
        public bool InitLocals
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool IsGenericMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool IsGenericMethodDefinition
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _name;

        [Obsolete("TODO", error: false)]
        public override ParameterInfo ReturnParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override Type ReturnType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override MethodImplAttributes MethodImplementationFlags
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public GenericTypeParameterBuilder[] DefineGenericParameters(params string[] names)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string strParamName)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override Type[] GetGenericArguments()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override MethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public ILGenerator GetILGenerator()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public ILGenerator GetILGenerator(int size)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
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
        public void SetImplementationFlags(MethodImplAttributes attributes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetParameters(params Type[] parameterTypes)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetReturnType(Type returnType)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetSignature(Type returnType, Type[] returnTypeRequiredCustomModifiers, Type[] returnTypeOptionalCustomModifiers, Type[] parameterTypes, Type[][] parameterTypeRequiredCustomModifiers, Type[][] parameterTypeOptionalCustomModifiers)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
