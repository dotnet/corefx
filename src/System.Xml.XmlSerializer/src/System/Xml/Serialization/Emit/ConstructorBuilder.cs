// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class ConstructorBuilder : ConstructorInfo
    {
        private readonly TypeBuilder _containingType;
        private readonly MethodAttributes _attributes;
        private readonly CallingConventions _callingConvention;
        private readonly Type[] _parameterTypes;

        public ConstructorBuilder(TypeBuilder typeBuilder, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes)
        {
            _containingType = typeBuilder;
            _attributes = attributes;
            _callingConvention = callingConvention;
            _parameterTypes = parameterTypes;
        }

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

        public override string Name => IsStatic ? ".cctor" : ".ctor";

        [Obsolete("TODO", error: false)]
        public override MethodImplAttributes MethodImplementationFlags
        {
            get
            {
               throw new NotImplementedException();
            }
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public ParameterBuilder DefineParameter(int iSequence, ParameterAttributes attributes, string strParamName)
        {
           throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public ILGenerator GetILGenerator()
        {
           throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public ILGenerator GetILGenerator(int streamSize)
        {
           throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetParameters()
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
    }
}
#endif
