// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.ILGeneration.Tests
{
    public class SignatureHelperGetHashCode
    {
        [Fact]
        public void PosTest1()
        {
            AssemblyBuilder myAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly_SignatureHelperAddArgument"), AssemblyBuilderAccess.Run);
            ModuleBuilder myModule = TestLibrary.Utilities.GetModuleBuilder(myAssembly, "Module_SignatureHelperAddArgument");
            SignatureHelper sHelper1 = SignatureHelper.GetFieldSigHelper(myModule);
            SignatureHelper sHelper2 = SignatureHelper.GetFieldSigHelper(myModule);

            int expectedValue;
            int actualValue;
            expectedValue = sHelper2.GetHashCode();
            actualValue = sHelper1.GetHashCode();
            Assert.Equal(expectedValue, actualValue);
        }
    }
}
