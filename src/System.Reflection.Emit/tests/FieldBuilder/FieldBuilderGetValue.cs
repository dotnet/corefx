// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class FieldBuilderGetValue
    {
        private TypeBuilder TypeBuilder
        {
            get
            {
                if (null == _typeBuilder)
                {
                    AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(
                        new AssemblyName("FieldBuilderGetValue_Assembly"), AssemblyBuilderAccess.Run);
                    ModuleBuilder module = TestLibrary.Utilities.GetModuleBuilder(assembly, "FieldBuilderGetValue_Module");
                    _typeBuilder = module.DefineType("FieldBuilderGetValue_Type", TypeAttributes.Abstract);
                }

                return _typeBuilder;
            }
        }

        private TypeBuilder _typeBuilder;

        [Fact]
        public void TestThrowsExceptionOnNull()
        {
            FieldBuilder field = TypeBuilder.DefineField("Field_NegTest1", typeof(int), FieldAttributes.Public);
            Assert.Throws<NotSupportedException>(() => { field.GetValue(null); });
        }
    }
}
