// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using Xunit;

namespace System.Reflection.Emit.ILGeneration.Tests
{
    public class LocalBuilderLocalIndex
    {
        private const string TestDynamicAssemblyName = "TestDynamicAssembly";
        private const string TestModuleName = "TestModuleName";
        private const string TestTypeName = "TestTypeName";
        private const string TestMethodName = "TestMethodName";

        private TypeBuilder GetCustomType(string name, AssemblyBuilderAccess access)
        {
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = name;
            AssemblyBuilder myAsmBuilder = AssemblyBuilder.DefineDynamicAssembly(myAsmName, access);
            ModuleBuilder moduleBuilder = TestLibrary.Utilities.GetModuleBuilder(myAsmBuilder, TestModuleName);
            return moduleBuilder.DefineType(TestTypeName);
        }

        [Fact]
        public void PosTest1()
        {
            TypeBuilder typeBuilder = this.GetCustomType(TestDynamicAssemblyName, AssemblyBuilderAccess.Run);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(TestMethodName, MethodAttributes.Public);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ret);
            LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(string));
            Assert.Equal(localBuilder.LocalIndex, 0);
        }

        [Fact]
        public void PosTest2()
        {
            TypeBuilder typeBuilder = this.GetCustomType(TestDynamicAssemblyName, AssemblyBuilderAccess.Run);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(TestMethodName, MethodAttributes.Public);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ret);
            for (int i = 0; i < 1000; i++)
            {
                LocalBuilder localBuilder = iLGenerator.DeclareLocal(typeof(char));
                Assert.Equal(localBuilder.LocalIndex, i);
            }
        }
    }
}
