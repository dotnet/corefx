// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class MethodBuilder : MethodInfo
    {
        private readonly TypeBuilder _containingType;
        private readonly string _name;

        public MethodBuilder(TypeBuilder containingType, string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            _containingType = containingType;
            _name = name;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public virtual MethodAttributes Attributes
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

        public override string Name => _name;

        [Obsolete("TODO", error: false)]
        public override Type ReturnType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string strParamName)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public ILGenerator GetILGenerator()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
#endif
