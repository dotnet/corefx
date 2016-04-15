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
        private readonly PropertyAttributes _attributes;
        private readonly Type _returnType;
        private readonly Type[] _parameterTypes;

        public PropertyBuilder(TypeBuilder containingType, string name, PropertyAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            _containingType = containingType;
            _name = name;
            _attributes = attributes;
            _returnType = returnType;
            _parameterTypes = parameterTypes;
        }

        public override Module Module => _containingType.Module;

        [Obsolete("TODO", error: false)]
        public override PropertyAttributes Attributes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public override bool CanWrite
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
        public override Type PropertyType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete("TODO", error: false)]
        public void AddOtherMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override object GetValue(object obj, object[] index)
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
        public void SetGetMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public void SetSetMethod(MethodBuilder mdBuilder)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public override void SetValue(object obj, object value, object[] index)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
