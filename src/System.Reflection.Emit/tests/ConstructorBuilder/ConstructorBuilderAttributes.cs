// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ConstructorBuilderAttributes
    {
        [Fact]
        public void TestContainsAccessorAttribute()
        {
            AssemblyName an = new AssemblyName();
            an.Name = "DynamicRandomAssembly";
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder mb = TestLibrary.Utilities.GetModuleBuilder(ab, "Module1");

            TypeBuilder tb = mb.DefineType("DynamicRandomClass", TypeAttributes.Public);

            Type[] parameterTypes = { typeof(int), typeof(double) };

            ConstructorBuilder cb = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes,
                null,
                null);

            Assert.Contains("Public", cb.Attributes.ToString());
        }

        [Fact]
        public void TestContainsAccessorAttributeWithDifferentOverload()
        {
            AssemblyName an = new AssemblyName();
            an.Name = "DynamicRandomAssembly";
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);

            ModuleBuilder mb = TestLibrary.Utilities.GetModuleBuilder(ab, "Module1");

            TypeBuilder tb = mb.DefineType("DynamicRandomClass", TypeAttributes.Public);

            Type[] parameterTypes = { typeof(int), typeof(double) };

            System.Reflection.Emit.ConstructorBuilder cb =
                tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);

            Assert.Contains("Public", cb.Attributes.ToString());
        }
    }
}
