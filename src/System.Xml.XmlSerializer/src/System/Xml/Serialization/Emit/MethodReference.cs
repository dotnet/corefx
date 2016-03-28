// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class MethodReference : MethodInfo
    {
        private readonly TypeInfo _containingType;
        private readonly Reflection.MethodInfo _info;

        public MethodReference(TypeInfo containingType, Reflection.MethodInfo info)
        {
            _containingType = containingType;
            _info = info;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override Type DeclaringType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _info.Name;  

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
#endif