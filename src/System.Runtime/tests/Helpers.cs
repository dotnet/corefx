// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;

namespace System.Tests
{
    public static class Helpers
    {
        public static Type NonRuntimeType()
        {
            AssemblyName assemblyName = new AssemblyName("AssemblyName");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder mboduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            TypeBuilder typeBuilder = mboduleBuilder.DefineType("TestType", TypeAttributes.Public);

            GenericTypeParameterBuilder[] typeParams = typeBuilder.DefineGenericParameters("T");

            return typeParams[0].UnderlyingSystemType;
        }
    }
}
