// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using System.Reflection;
using System.Reflection.Emit;
using TestLibrary;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class TypeBuilderGetMethod
    {
        [Fact]
        public void TestOnGenericTypeMethod()
        {
            AssemblyName myAsmName =
                new AssemblyName("TypeBuilderGetMethodTest");
            AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(
                 myAsmName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModule = TestLibrary.Utilities.GetModuleBuilder(myAssembly, "Module1");

            TypeBuilder myType = myModule.DefineType("Sample",
                TypeAttributes.Class | TypeAttributes.Public);

            string[] typeParamNames = { "T" };
            GenericTypeParameterBuilder[] typeParams =
                myType.DefineGenericParameters(typeParamNames);

            MethodBuilder genMethod = myType.DefineMethod("GM",
                MethodAttributes.Public | MethodAttributes.Static);
            string[] methodParamNames = { "U" };
            GenericTypeParameterBuilder[] methodParams =
                genMethod.DefineGenericParameters(methodParamNames);

            genMethod.SetSignature(null, null, null,
                new Type[] { methodParams[0].AsType() }, null, null);

            MethodInfo SampleOfIntGM = TypeBuilder.GetMethod(myType.AsType(),
                genMethod);

            Assert.True(SampleOfIntGM.IsGenericMethodDefinition);
            Assert.Equal("U", SampleOfIntGM.GetGenericArguments()[0].ToString());
        }

        [Fact]
        public void TestOnConstructedTypeMethod()
        {
            AssemblyName myAsmName =
                new AssemblyName("TypeBuilderGetMethodTest");
            AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(
                 myAsmName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModule = TestLibrary.Utilities.GetModuleBuilder(myAssembly, "Module1");



            TypeBuilder myType = myModule.DefineType("Sample",
                TypeAttributes.Class | TypeAttributes.Public);

            string[] typeParamNames = { "T" };
            GenericTypeParameterBuilder[] typeParams =
                myType.DefineGenericParameters(typeParamNames);

            MethodBuilder genMethod = myType.DefineMethod("GM",
                MethodAttributes.Public | MethodAttributes.Static);
            string[] methodParamNames = { "U" };
            GenericTypeParameterBuilder[] methodParams =
                genMethod.DefineGenericParameters(methodParamNames);

            genMethod.SetSignature(null, null, null,
                new Type[] { methodParams[0].AsType() }, null, null);

            Type SampleOfInt =
                myType.MakeGenericType(typeof(int));
            MethodInfo SampleOfIntGM = TypeBuilder.GetMethod(SampleOfInt,
                genMethod);

            Assert.True(SampleOfIntGM.IsGenericMethodDefinition);
            Assert.Equal("U", SampleOfIntGM.GetGenericArguments()[0].ToString());
        }

        [Fact]
        public void TestThrowsExceptionForTypeNotTypeBuilder()
        {
            Assert.Throws<ArgumentException>(() => { TypeBuilder.GetMethod(typeof(int), typeof(int).GetMethod("Parse", new Type[] { typeof(string) })); });
        }

        [Fact]
        public void TestThrowsExceptionForMethodDefinitionNotInTypeGenericDefinition()
        {
            AssemblyName myAsmName =
                new AssemblyName("TypeBuilderGetMethodTest");
            AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(
                 myAsmName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModule = TestLibrary.Utilities.GetModuleBuilder(myAssembly, "Module1");



            TypeBuilder myType = myModule.DefineType("Sample",
                TypeAttributes.Class | TypeAttributes.Public);

            string[] typeParamNames = { "T" };
            GenericTypeParameterBuilder[] typeParams =
                myType.DefineGenericParameters(typeParamNames);

            MethodBuilder genMethod = myType.DefineMethod("GM",
                MethodAttributes.Public | MethodAttributes.Static);
            string[] methodParamNames = { "U" };
            GenericTypeParameterBuilder[] methodParams =
                genMethod.DefineGenericParameters(methodParamNames);

            genMethod.SetSignature(null, null, null,
                new Type[] { methodParams[0].AsType() }, null, null);

            Type SampleOfInt =
                myType.MakeGenericType(typeof(int));
            MethodInfo genMethod2 = genMethod.MakeGenericMethod(typeof(int));
            Assert.Throws<ArgumentException>(() =>
            {
                MethodInfo SampleOfIntGM = TypeBuilder.GetMethod(SampleOfInt, genMethod2);
            });
        }

        [Fact]
        public void TestThrowsExceptionOnMethodNotGenericTypeDefinition()
        {
            AssemblyName myAsmName =
                new AssemblyName("TypeBuilderGetMethodTest");
            AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(
                 myAsmName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModule = TestLibrary.Utilities.GetModuleBuilder(myAssembly, "Module1");



            TypeBuilder myType = myModule.DefineType("Sample",
                TypeAttributes.Class | TypeAttributes.Public);
            string[] typeParamNames = { "T" };
            GenericTypeParameterBuilder[] typeParams =
                myType.DefineGenericParameters(typeParamNames);

            TypeBuilder myType2 = myModule.DefineType("Sample2",
                TypeAttributes.Class | TypeAttributes.Public);
            GenericTypeParameterBuilder[] typeParams2 =
                myType2.DefineGenericParameters(typeParamNames);

            MethodBuilder genMethod = myType.DefineMethod("GM",
                MethodAttributes.Public | MethodAttributes.Static);
            string[] methodParamNames = { "U" };
            GenericTypeParameterBuilder[] methodParams =
                genMethod.DefineGenericParameters(methodParamNames);
            genMethod.SetSignature(null, null, null,
                new Type[] { methodParams[0].AsType() }, null, null);

            MethodBuilder genMethod2 = myType2.DefineMethod("GM", MethodAttributes.Public | MethodAttributes.Static);
            GenericTypeParameterBuilder[] methodParams2 =
                genMethod2.DefineGenericParameters(methodParamNames);
            genMethod2.SetSignature(null, null, null,
                new Type[] { methodParams[0].AsType() }, null, null);

            Type SampleOfInt =
                myType.MakeGenericType(typeof(int));

            Assert.Throws<ArgumentException>(() =>
            {
                MethodInfo SampleOfIntGM = TypeBuilder.GetMethod(SampleOfInt, genMethod2);
            });
        }

        [Fact]
        public void TestThrowsExceptionForTypeIsNotGeneric()
        {
            AssemblyName myAsmName =
                new AssemblyName("TypeBuilderGetMethodTest");
            AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(
                    myAsmName, AssemblyBuilderAccess.Run);
            ModuleBuilder myModule = TestLibrary.Utilities.GetModuleBuilder(myAssembly, "Module1");
            TypeBuilder myType = myModule.DefineType("Sample",
                TypeAttributes.Class | TypeAttributes.Public);

            MethodBuilder genMethod = myType.DefineMethod("GM",
                MethodAttributes.Public | MethodAttributes.Static);
            string[] methodParamNames = { "U" };
            GenericTypeParameterBuilder[] methodParams =
                genMethod.DefineGenericParameters(methodParamNames);

            genMethod.SetSignature(null, null, null,
                new Type[] { methodParams[0].AsType() }, null, null);

            Assert.Throws<ArgumentException>(() =>
            {
                MethodInfo SampleOfIntGM = TypeBuilder.GetMethod(myType.AsType(), genMethod);
            });
        }
    }
}
