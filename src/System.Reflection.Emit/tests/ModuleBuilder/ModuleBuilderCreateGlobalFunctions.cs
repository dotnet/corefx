// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ModuleBuilderCreateGlobalFunctions
    {
        [Fact]
        public void CreateGlobalFunctions_SingleGlobalMethod()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            MethodBuilder method = module.DefineGlobalMethod("TestMethod", MethodAttributes.Static | MethodAttributes.Public, null, null);
            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.EmitWriteLine("Hello World from global method.");
            ilGenerator.Emit(OpCodes.Ret);

            module.CreateGlobalFunctions();
        }

        [Fact]
        public void CreateGlobalFunctions_MultipleGlobalMethods()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            MethodBuilder method = module.DefineGlobalMethod("TestMethod", MethodAttributes.Static | MethodAttributes.Public, null, null);
            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.EmitWriteLine("Hello World from global method.");
            ilGenerator.Emit(OpCodes.Ret);

            method = module.DefineGlobalMethod("MyMethod2", MethodAttributes.Static | MethodAttributes.Public,
             null, null);
            ilGenerator = method.GetILGenerator();
            ilGenerator.EmitWriteLine("Hello World from global method again!");

            module.CreateGlobalFunctions();
        }

        [Fact]
        public void CreateGlobalFunctions_CalledMultipleTimes_ThrowsInvalidOperationException()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            MethodBuilder method = module.DefineGlobalMethod("TestMethod", MethodAttributes.Static | MethodAttributes.Public, null, null);
            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.EmitWriteLine("Hello World from global method.");
            ilGenerator.Emit(OpCodes.Ret);

            module.CreateGlobalFunctions();
            Assert.Throws<InvalidOperationException>(() => module.CreateGlobalFunctions());
        }
    }
}
