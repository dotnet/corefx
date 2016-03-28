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
        public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs)
        {
        }
    }
}
#endif