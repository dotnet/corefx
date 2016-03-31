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
    public class EnumBuilderUnderlyingField
    {
        private AssemblyBuilder _myAssemblyBuilder;

        private ModuleBuilder CreateCallee()
        {
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = "EnumAssembly";
            _myAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);
            return TestLibrary.Utilities.GetModuleBuilder(_myAssemblyBuilder, "EnumModule.mod");
        }

        [Fact]
        public void TestUnderlyingFieldProperty()
        {
            var myModuleBuilder = CreateCallee();
            var myEnumBuilder = myModuleBuilder.DefineEnum("myEnum", TypeAttributes.Public, typeof(int));
            FieldBuilder fieldBuilder1 = myEnumBuilder.DefineLiteral("field1", 1);
            myEnumBuilder.AsType();
            FieldBuilder myUnderlyingField = myEnumBuilder.UnderlyingField;
            Assert.NotNull(myUnderlyingField);
        }

        [Fact]
        public void TestUnderlyingFieldWithNoLiteralDefined()
        {
            var myModuleBuilder = CreateCallee();
            var myEnumBuilder = myModuleBuilder.DefineEnum("myEnum", TypeAttributes.Public, typeof(int));
            FieldBuilder myUnderlyingField = myEnumBuilder.UnderlyingField;
            Assert.NotNull(myUnderlyingField);
        }
    }
}
