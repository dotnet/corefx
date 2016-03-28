// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class ConstructorBuilder : ConstructorInfo
    {
        private readonly TypeBuilder _containingType;

        public ConstructorBuilder(TypeBuilder typeBuilder, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes)
        {
            _containingType = typeBuilder;
        }

        [Obsolete("TODO", error: false)]
        public override Type DeclaringType
        {
            get
            {
               throw new NotImplementedException();
            }
        }

        public override string Name => IsStatic ? ".cctor" : ".ctor";

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetParameters()
        {
           throw new NotImplementedException();
        }
    }
}
#endif
