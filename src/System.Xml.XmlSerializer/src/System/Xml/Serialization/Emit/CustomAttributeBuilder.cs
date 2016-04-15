// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;

#if !NET_NATIVE
namespace System.Xml.Serialization.Emit
{
    internal class CustomAttributeBuilder
    {
        private readonly ConstructorInfo _ctor;
        private readonly object[] _args;

        public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs)
        {
            _ctor = con;
            _args = constructorArgs;
        }

        [Obsolete("TODO", error: false)]
        public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, FieldInfo[] namedFields, object[] fieldValues)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues)
        {
            throw new NotImplementedException();
        }

        [Obsolete("TODO", error: false)]
        public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
        {
            throw new NotImplementedException();
        }
    }
}
#endif