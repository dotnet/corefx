// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class PropertyBuilder : PropertyInfo
    {
        private readonly TypeBuilder _containingType;
        private readonly string _name;

        public PropertyBuilder(TypeBuilder containingType, string name, PropertyAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            _containingType = containingType;
            _name = name;
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

        public override string Name => _name;

        [Obsolete("TODO", error: false)]
        public override Type PropertyType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public void SetGetMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
