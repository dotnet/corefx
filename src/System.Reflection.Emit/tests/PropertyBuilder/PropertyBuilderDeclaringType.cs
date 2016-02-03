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
    public class PropertyBuilderTest6
    {
        private const string DynamicAssemblyName = "TestDynamicAssembly";
        private const string DynamicModuleName = "TestDynamicModule";
        private const string DynamicTypeName = "TestDynamicType";
        private const string DynamicNestedTypeName = "TestDynamicNestedType";
        private const string DynamicDerivedTypeName = "TestDynamicDerivedType";
        private const string DynamicFieldName = "TestDynamicFieldA";
        private const string DynamicPropertyName = "TestDynamicProperty";
        private const string DynamicMethodName = "DynamicMethodA";

        private TypeBuilder GetTypeBuilder(string typeName, TypeAttributes typeAtt)
        {
            AssemblyName myAssemblyName = new AssemblyName();
            myAssemblyName.Name = DynamicAssemblyName;
            AssemblyBuilder myAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                                                                        myAssemblyName,
                                                                        AssemblyBuilderAccess.Run);

            ModuleBuilder myModuleBuilder = TestLibrary.Utilities.GetModuleBuilder(
                                                                        myAssemblyBuilder,
                                                                        DynamicModuleName);

            return myModuleBuilder.DefineType(typeName, typeAtt);
        }

        [Fact]
        public void TestPropertyInRootClass()
        {
            Type actualType;
            TypeBuilder myTypeBuilder;
            PropertyBuilder myPropertyBuilder;

            myTypeBuilder = GetTypeBuilder(
                                    DynamicTypeName,
                                    TypeAttributes.Class |
                                    TypeAttributes.Public);

            myPropertyBuilder = myTypeBuilder.DefineProperty(
                                    DynamicPropertyName,
                                    PropertyAttributes.None,
                                    typeof(int),
                                    null);
            actualType = myPropertyBuilder.DeclaringType;
            Assert.Equal(actualType, myTypeBuilder.AsType());
        }

        [Fact]
        public void TestPropertyInNestedClass()
        {
            Type actualType;
            TypeBuilder myTypeBuilder, myNestedTypeBuilder;
            PropertyBuilder myPropertyBuilder;

            myTypeBuilder = GetTypeBuilder(
                                    DynamicTypeName,
                                    TypeAttributes.Class |
                                    TypeAttributes.Public);
            myNestedTypeBuilder = myTypeBuilder.DefineNestedType(
                                                    DynamicNestedTypeName,
                                                    TypeAttributes.Class |
                                                    TypeAttributes.NestedPublic);

            myPropertyBuilder = myNestedTypeBuilder.DefineProperty(
                                    DynamicPropertyName,
                                    PropertyAttributes.None,
                                    typeof(int),
                                    null);
            actualType = myPropertyBuilder.DeclaringType;
            Assert.Equal(myNestedTypeBuilder.AsType(), actualType);
        }

        [Fact]
        public void TestPropertyInRootInterface()
        {
            Type actualType;
            TypeBuilder myTypeBuilder;
            PropertyBuilder myPropertyBuilder;

            myTypeBuilder = GetTypeBuilder(
                                    DynamicTypeName,
                                    TypeAttributes.Class |
                                    TypeAttributes.Public);
            myPropertyBuilder = myTypeBuilder.DefineProperty(
                                    DynamicPropertyName,
                                    PropertyAttributes.None,
                                    typeof(int),
                                    null);
            actualType = myPropertyBuilder.DeclaringType;
            Assert.Equal(myTypeBuilder.AsType(), actualType);
        }

        [Fact]
        public void TestPropertyInDerivedClass()
        {
            Type actualType;
            TypeBuilder myTypeBuilder, myDerivedTypeBuilder;
            PropertyBuilder myPropertyBuilder;

            myTypeBuilder = GetTypeBuilder(
                                    DynamicTypeName,
                                    TypeAttributes.Class |
                                    TypeAttributes.Public);
            myDerivedTypeBuilder = GetTypeBuilder(
                                            DynamicDerivedTypeName,
                                            TypeAttributes.Class |
                                            TypeAttributes.Public);
            myDerivedTypeBuilder.SetParent(myTypeBuilder.AsType());

            myPropertyBuilder = myDerivedTypeBuilder.DefineProperty(
                                    DynamicPropertyName,
                                    PropertyAttributes.None,
                                    typeof(int),
                                    null);
            actualType = myPropertyBuilder.DeclaringType;
            Assert.Equal(myDerivedTypeBuilder.AsType(), actualType);
        }

        [Fact]
        public void TestPropertyInBaseClass()
        {
            Type actualType;
            TypeBuilder myTypeBuilder, myDerivedTypeBuilder;
            PropertyBuilder myPropertyBuilder;

            myTypeBuilder = GetTypeBuilder(
                                    DynamicTypeName,
                                    TypeAttributes.Abstract |
                                    TypeAttributes.Public);
            myDerivedTypeBuilder = GetTypeBuilder(
                                    DynamicDerivedTypeName,
                                    TypeAttributes.Abstract |
                                    TypeAttributes.Public |
                                    TypeAttributes.Interface);
            myDerivedTypeBuilder.SetParent(myTypeBuilder.AsType());
            myPropertyBuilder = myTypeBuilder.DefineProperty(
                                    DynamicPropertyName,
                                    PropertyAttributes.None,
                                    typeof(int),
                                    null);
            actualType = myPropertyBuilder.DeclaringType;
            Assert.Equal(myTypeBuilder.AsType(), actualType);
        }
    }
}
