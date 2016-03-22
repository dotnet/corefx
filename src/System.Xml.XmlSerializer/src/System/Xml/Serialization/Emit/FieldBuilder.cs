// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal sealed class FieldBuilder : FieldInfo
    {
        private readonly TypeBuilder _containingType;
        private readonly string _name;
        private readonly Type _type;
        private readonly FieldAttributes _attributes;

        internal FieldBuilder(TypeBuilder containingType, string name, Type type, FieldAttributes attributes)
        {
            _containingType = containingType;
            _name = name;
            _type = type;
            _attributes = attributes;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override FieldAttributes Attributes
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
        public override Type FieldType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name => _name;

        [Obsolete("TODO", error: false)]
        public override object GetValue(object obj)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetConstant(object defaultValue)
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
        public void SetOffset(int iOffset)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
