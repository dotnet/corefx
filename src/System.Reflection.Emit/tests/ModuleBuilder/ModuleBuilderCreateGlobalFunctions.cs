// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ModuleBuilderCreateGlobalFunctions
    {
        [Fact]
        public void TestWithSingleGlobalMethod()
        {
            ModuleBuilder m1 = CreateModule("Foo", "Bar");
            MethodBuilder mb = m1.DefineGlobalMethod("MyMethod1", MethodAttributes.Static | MethodAttributes.Public,
                null, null);
            ILGenerator ilg = mb.GetILGenerator();
            ilg.EmitWriteLine("Hello World from global method.");
            ilg.Emit(OpCodes.Ret);
            m1.CreateGlobalFunctions();
        }

        [Fact]
        public void TestWithMultipleGlobalMethods()
        {
            ModuleBuilder m1 = CreateModule("Baz", "Quux");
            MethodBuilder mb = m1.DefineGlobalMethod("MyMethod1", MethodAttributes.Static | MethodAttributes.Public,
                null, null);
            ILGenerator ilg = mb.GetILGenerator();
            ilg.EmitWriteLine("Hello World from global method.");
            ilg.Emit(OpCodes.Ret);
            mb = m1.DefineGlobalMethod("MyMethod2", MethodAttributes.Static | MethodAttributes.Public,
             null, null);
            ilg = mb.GetILGenerator();
            ilg.EmitWriteLine("Hello World from global method again!");
            m1.CreateGlobalFunctions();
        }

        [Fact]
        public void TestThrowsExceptionOnMultipleCallsToCreateGlobalFunctions()
        {
            ModuleBuilder m1 = CreateModule("Grip", "Fang");
            MethodBuilder mb = m1.DefineGlobalMethod("MyMethod1", MethodAttributes.Static | MethodAttributes.Public,
                null, null);
            ILGenerator ilg = mb.GetILGenerator();
            ilg.EmitWriteLine("Hello World from global method.");
            ilg.Emit(OpCodes.Ret);
            m1.CreateGlobalFunctions();
            Assert.Throws<InvalidOperationException>(() => { m1.CreateGlobalFunctions(); });
        }

        public ModuleBuilder CreateModule(string assemblyName, string modName)
        {
            AssemblyName asmName;
            AssemblyBuilder asmBuilder;
            ModuleBuilder modBuilder;

            // create the dynamic module
            asmName = new AssemblyName(assemblyName);
            asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            modBuilder = TestLibrary.Utilities.GetModuleBuilder(asmBuilder, "Module1");

            return modBuilder;
        }
    }
}
