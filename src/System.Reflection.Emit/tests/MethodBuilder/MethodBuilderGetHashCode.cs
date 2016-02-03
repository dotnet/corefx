// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class MethodBuilderGetHashCode
    {
        private const string TestDynamicAssemblyName = "TestDynamicAssembly";
        private const string TestDynamicModuleName = "TestDynamicModule";
        private const string TestDynamicTypeName = "TestDynamicType";
        private const AssemblyBuilderAccess TestAssemblyBuilderAccess = AssemblyBuilderAccess.Run;
        private const TypeAttributes TestTypeAttributes = TypeAttributes.Abstract;
        private const int MinStringLength = 1;
        private const int MaxStringLength = 128;
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        private TypeBuilder GetTestTypeBuilder()
        {
            AssemblyName assemblyName = new AssemblyName(TestDynamicAssemblyName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName, TestAssemblyBuilderAccess);

            ModuleBuilder moduleBuilder = TestLibrary.Utilities.GetModuleBuilder(assemblyBuilder, TestDynamicModuleName);
            return moduleBuilder.DefineType(TestDynamicTypeName, TestTypeAttributes);
        }

        [Fact]
        public void TestForEqualObjects1()
        {
            string methodName = null;
            methodName = _generator.GetString(false, false, true, MinStringLength, MaxStringLength);
            TypeBuilder typeBuilder = GetTestTypeBuilder();
            MethodBuilder builder1 = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public);
            MethodBuilder builder2 = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public);

            Assert.Equal(builder1.GetHashCode(), builder2.GetHashCode());
        }

        [Fact]
        public void TestForEqualObjects2()
        {
            string methodName = null;
            methodName = _generator.GetString(false, false, true, MinStringLength, MaxStringLength);
            TypeBuilder typeBuilder = GetTestTypeBuilder();
            MethodBuilder builder1 = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public);
            string[] typeParamNames = { "T", "U" };
            GenericTypeParameterBuilder[] Parameters = builder1.DefineGenericParameters(typeParamNames);
            MethodBuilder builder2 = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public);
            Parameters = builder2.DefineGenericParameters(typeParamNames);

            Assert.Equal(builder1.GetHashCode(), builder2.GetHashCode());
        }

        [Fact]
        public void TestForNonEqualObjects()
        {
            string methodName1 = null;
            string methodName2 = null;
            methodName1 = _generator.GetString(false, false, true, MinStringLength, MaxStringLength);
            methodName2 = _generator.GetString(false, false, true, MinStringLength, MaxStringLength);

            TypeBuilder typeBuilder = GetTestTypeBuilder();
            MethodBuilder builder1 = typeBuilder.DefineMethod(methodName1,
                MethodAttributes.Public);
            MethodBuilder builder2 = typeBuilder.DefineMethod(methodName2,
                MethodAttributes.Public);

            Assert.NotEqual(builder1.GetHashCode(), builder2.GetHashCode());
        }
    }
}
