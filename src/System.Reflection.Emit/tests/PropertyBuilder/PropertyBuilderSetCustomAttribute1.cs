// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class PropertyBuilderTest12
    {
        private const string DynamicAssemblyName = "TestDynamicAssembly";
        private const string DynamicModuleName = "TestDynamicModule";
        private const string DynamicTypeName = "TestDynamicType";
        private const string DynamicFieldName = "TestDynamicFieldA";
        private const string DynamicPropertyName = "TestDynamicProperty";
        private const string DynamicMethodName = "DynamicMethodA";
        private readonly RandomDataGenerator _generator = new RandomDataGenerator();

        private TypeBuilder GetTypeBuilder(TypeAttributes typeAtt)
        {
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = DynamicAssemblyName;
            AssemblyBuilder myAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(myAssemblyName,
                                                                            AssemblyBuilderAccess.Run);

            ModuleBuilder myModuleBuilder = TestLibrary.Utilities.GetModuleBuilder(myAssemblyBuilder,
                                                                                DynamicModuleName);

            return myModuleBuilder.DefineType(DynamicTypeName, typeAtt);
        }

        [Fact]
        public void TestSetCustomAttribute()
        {
            MethodAttributes getMethodAttr = MethodAttributes.Public |
                                             MethodAttributes.SpecialName |
                                             MethodAttributes.HideBySig;
            Type returnType = typeof(int);
            Type[] ctorParamTypes = new Type[] { typeof(int) };
            int expectedValue = _generator.GetInt32();
            object[] ctorParamValues = new object[] { expectedValue };
            CustomAttributeBuilder customAttrBuilder = new CustomAttributeBuilder(
                                                            typeof(PBMyAttribute).GetConstructor(ctorParamTypes),
                                                            ctorParamValues);
            object[] actualCustomAttrs;

            actualCustomAttrs = ExecutePosTest(
                                        customAttrBuilder,
                                        getMethodAttr,
                                        returnType,
                                        new Type[0],
                                        BindingFlags.Public |
                                        BindingFlags.Instance |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Static);
            Assert.Equal(1, actualCustomAttrs.Length);
            Assert.True(actualCustomAttrs[0] is PBMyAttribute);
            Assert.Equal(expectedValue, (actualCustomAttrs[0] as PBMyAttribute).Value);
        }

        private object[] ExecutePosTest(
                            CustomAttributeBuilder customAttrBuilder,
                            MethodAttributes getMethodAttr,
                            Type returnType,
                            Type[] paramTypes,
                            BindingFlags bindingAttr)
        {
            TypeBuilder myTypeBuilder = GetTypeBuilder(TypeAttributes.Class | TypeAttributes.Public);

            PropertyBuilder myPropertyBuilder = myTypeBuilder.DefineProperty(DynamicPropertyName,
                                                                             PropertyAttributes.HasDefault,
                                                                             returnType, null);
            myPropertyBuilder.SetCustomAttribute(customAttrBuilder);
            // Define the "get" accessor method for DynamicPropertyName
            MethodBuilder myMethodBuilder = myTypeBuilder.DefineMethod(DynamicMethodName,
                                                                       getMethodAttr, returnType, paramTypes);
            ILGenerator methodILGenerator = myMethodBuilder.GetILGenerator();
            methodILGenerator.Emit(OpCodes.Ldarg_0);
            methodILGenerator.Emit(OpCodes.Ret);

            // Map the 'get' method created above to our PropertyBuilder
            myPropertyBuilder.SetGetMethod(myMethodBuilder);

            Type myType = myTypeBuilder.CreateTypeInfo().AsType();

            PropertyInfo myProperty = myType.GetProperty(DynamicPropertyName, bindingAttr);
            return myProperty.GetCustomAttributes(false).Select(a => (object)a).ToArray();
        }

        [Fact]
        public void TestThrowsExceptionOnNullBuilder()
        {
            MethodAttributes getMethodAttr = MethodAttributes.Public |
                                             MethodAttributes.SpecialName |
                                             MethodAttributes.HideBySig;

            Type returnType = typeof(int);
            Type[] paramTypes = new Type[0];

            Assert.Throws<ArgumentNullException>(() =>
            {
                ExecutePosTest(null as CustomAttributeBuilder, getMethodAttr, returnType, paramTypes, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            });
        }

        [Fact]
        public void TestThrowsExceptionForCreateTypeCalled()
        {
            MethodAttributes getMethodAttr = MethodAttributes.Public |
                                             MethodAttributes.SpecialName |
                                             MethodAttributes.HideBySig;

            TypeBuilder myTypeBuilder;
            PropertyBuilder myPropertyBuilder;
            MethodBuilder myMethodBuilder;
            Type returnType = typeof(int);
            Type[] paramTypes = new Type[0];

            Type[] ctorParamTypes = new Type[] { typeof(int) };
            int expectedValue = _generator.GetInt32();
            object[] ctorParamValues = new object[] { expectedValue };
            CustomAttributeBuilder customAttrBuilder = new CustomAttributeBuilder(
                                                            typeof(PBMyAttribute).GetConstructor(ctorParamTypes),
                                                            ctorParamValues);

            myTypeBuilder = GetTypeBuilder(TypeAttributes.Class | TypeAttributes.Public);
            myPropertyBuilder = myTypeBuilder.DefineProperty(DynamicPropertyName,
                                                             PropertyAttributes.None,
                                                             returnType,
                                                             paramTypes);
            myMethodBuilder = myTypeBuilder.DefineMethod(DynamicMethodName,
                                             getMethodAttr,
                                             typeof(int),
                                             paramTypes);
            ILGenerator methodILGenerator = myMethodBuilder.GetILGenerator();

            methodILGenerator.Emit(OpCodes.Ldarg_0);
            methodILGenerator.Emit(OpCodes.Ldarg_1);
            methodILGenerator.Emit(OpCodes.Ret);
            myPropertyBuilder.SetGetMethod(myMethodBuilder);
            myTypeBuilder.CreateTypeInfo().AsType();
            Assert.Throws<InvalidOperationException>(() => { myPropertyBuilder.SetCustomAttribute(customAttrBuilder); });
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class PBMyAttribute : Attribute
    {
        private int _value;

        public int Value
        {
            get { return _value; }
        }

        public PBMyAttribute(int value)
        {
            _value = value;
        }

        public PBMyAttribute() : this(0)
        { }
    }
}
