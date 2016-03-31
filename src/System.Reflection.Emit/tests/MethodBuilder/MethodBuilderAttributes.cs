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
    public class MethodBuilderAttributes
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
        public void TestForMethodAttributes()
        {
            PosTestHelp(MethodAttributes.Abstract);
            PosTestHelp(MethodAttributes.Assembly);
            PosTestHelp(MethodAttributes.CheckAccessOnOverride);
            PosTestHelp(MethodAttributes.FamANDAssem);
            PosTestHelp(MethodAttributes.Family);
            PosTestHelp(MethodAttributes.FamORAssem);
            PosTestHelp(MethodAttributes.Final);
            PosTestHelp(MethodAttributes.HasSecurity);
            PosTestHelp(MethodAttributes.HideBySig);
            PosTestHelp(MethodAttributes.MemberAccessMask);
            PosTestHelp(MethodAttributes.NewSlot);
            PosTestHelp(MethodAttributes.PinvokeImpl);
            PosTestHelp(MethodAttributes.Private);
            PosTestHelp(MethodAttributes.PrivateScope);
            PosTestHelp(MethodAttributes.Public);
            PosTestHelp(MethodAttributes.RequireSecObject);
            PosTestHelp(MethodAttributes.ReuseSlot);
            PosTestHelp(MethodAttributes.RTSpecialName);
            PosTestHelp(MethodAttributes.SpecialName);
            PosTestHelp(MethodAttributes.Static);
            PosTestHelp(MethodAttributes.UnmanagedExport);
            PosTestHelp(MethodAttributes.Virtual);
            PosTestHelp(MethodAttributes.VtableLayoutMask);
        }

        [Fact]
        public void TestForCombinationOfMethodAttributes()
        {
            PosTestHelp(
                MethodAttributes.Abstract |
                MethodAttributes.Public |
                MethodAttributes.NewSlot |
                MethodAttributes.Virtual);
            PosTestHelp(
                MethodAttributes.Final |
                MethodAttributes.Private |
                MethodAttributes.SpecialName |
                MethodAttributes.Static);
        }

        private void PosTestHelp(MethodAttributes desiredAttribute)
        {
            string methodName = null;
            MethodAttributes actualAttribute = (MethodAttributes)(-1);
            methodName = _generator.GetString(false, MinStringLength, MaxStringLength);

            TypeBuilder typeBuilder = GetTestTypeBuilder();
            MethodBuilder builder = typeBuilder.DefineMethod(methodName,
                desiredAttribute);
            actualAttribute = builder.Attributes;

            Assert.Equal(desiredAttribute, actualAttribute);
        }
    }
}
