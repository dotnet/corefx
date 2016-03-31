// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using TestLibrary;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Xunit;
using System.Linq;

namespace System.Reflection.Emit.Tests
{
    public class MBMyAttribute3 : Attribute
    {
        public MBMyAttribute3(int mc)
        {
            m_ctorType2 = mc;
        }

        public string Field12345;
        public int m_ctorType2;
    }

    public class MethodBuilderSetCustomAttribute
    {
        [Fact]
        public void TestSetCustomAttribute()
        {
            string name = "Assembly1";
            AssemblyName asmname = new AssemblyName();
            asmname.Name = name;

            AssemblyBuilder asmbuild = AssemblyBuilder.DefineDynamicAssembly(asmname, AssemblyBuilderAccess.Run);
            ModuleBuilder modbuild = TestLibrary.Utilities.GetModuleBuilder(asmbuild, "Module1");

            TypeBuilder tpbuild = modbuild.DefineType("C1");
            MethodBuilder methbuild = tpbuild.DefineMethod("method1", MethodAttributes.Public | MethodAttributes.Static);

            ILGenerator ilgen = methbuild.GetILGenerator();
            ilgen.Emit(OpCodes.Ldc_I4, 100);
            ilgen.Emit(OpCodes.Ret);

            Type attrType = typeof(MBMyAttribute3);
            ConstructorInfo ci = attrType.GetConstructors()[0];
            FieldInfo fi = attrType.GetField("Field12345");


            CustomAttributeBuilder cab = new CustomAttributeBuilder(ci,
                                                                    new object[] { 4 },
                                                                    new FieldInfo[] { fi },
                                                                    new object[] { "hello" });
            methbuild.SetCustomAttribute(cab);
            Type tp = tpbuild.CreateTypeInfo().AsType();
            MethodInfo md = tp.GetMethod("method1");

            // VERIFY
            object[] attribs = md.GetCustomAttributes(false).Select(a => (object)a).ToArray();

            Assert.Equal(1, attribs.Length);
            MBMyAttribute3 obj = (MBMyAttribute3)attribs[0];

            Assert.Equal("hello", obj.Field12345);
            Assert.Equal(4, obj.m_ctorType2);
        }

        [Fact]
        public void TestThrowsExceptionOnNull()
        {
            string name = "Assembly1";
            AssemblyName asmname = new AssemblyName();
            asmname.Name = name;

            AssemblyBuilder asmbuild = AssemblyBuilder.DefineDynamicAssembly(asmname, AssemblyBuilderAccess.Run);
            ModuleBuilder modbuild = TestLibrary.Utilities.GetModuleBuilder(asmbuild, "Module1");
            TypeBuilder tpbuild = modbuild.DefineType("C1", TypeAttributes.Public);
            MethodBuilder methbuild = tpbuild.DefineMethod("method1", MethodAttributes.Public | MethodAttributes.Static);

            Assert.Throws<ArgumentNullException>(() => { methbuild.SetCustomAttribute(null); });
        }
    }
}
