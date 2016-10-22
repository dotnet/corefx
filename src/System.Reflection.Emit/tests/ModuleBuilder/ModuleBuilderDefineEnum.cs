// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ModuleBuilderDefineEnum
    {
        public static IEnumerable<object[]> DefineEnum_TestData()
        {
            yield return new object[] { "TestEnum", TypeAttributes.Public, typeof(byte) };
            yield return new object[] { "testenum", TypeAttributes.NotPublic, typeof(sbyte) };
            yield return new object[] { "enum", TypeAttributes.Public, typeof(short) };
            yield return new object[] { "\uD800\uDC00", TypeAttributes.Public, typeof(ushort) };
            yield return new object[] { "a\0b\0c", TypeAttributes.Public, typeof(int) };
            yield return new object[] { "Name", TypeAttributes.Public, typeof(uint) };
            yield return new object[] { "Name", TypeAttributes.Public, typeof(long) };
        }

        [Theory]
        [MemberData(nameof(DefineEnum_TestData))]
        public void DefineEnum(string name, TypeAttributes visibility, Type underlyingType)
        {
            ModuleBuilder module = Helpers.DynamicModule();
            EnumBuilder enumBuilder = module.DefineEnum(name, visibility, underlyingType);
            Assert.True(enumBuilder.IsEnum);

            Assert.Equal(module.Assembly, enumBuilder.Assembly);
            Assert.Equal(module, enumBuilder.Module);

            Assert.Equal(name, enumBuilder.Name);
            Assert.Equal(Helpers.GetFullName(name), enumBuilder.FullName);
            Assert.Equal(enumBuilder.FullName + ", " + module.Assembly.FullName, enumBuilder.AssemblyQualifiedName);

            Assert.Equal(typeof(Enum), enumBuilder.BaseType);
            Assert.Null(enumBuilder.DeclaringType);

            Assert.True(enumBuilder.Attributes.HasFlag(visibility));
            Assert.Equal(underlyingType, enumBuilder.UnderlyingField.FieldType);
            
            enumBuilder.CreateTypeInfo().AsType();
        }

        {
            ModuleBuilder module = Helpers.DynamicModule();
        }

        [Fact]
        public void DefineEnum_EnumWithSameNameExists_ThrowsArgumentException()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            module.DefineEnum("Name", TypeAttributes.Public, typeof(int));
            Assert.Throws<ArgumentException>(null, () => module.DefineEnum("Name", TypeAttributes.Public, typeof(int)));
        }

        [Fact]
        public void DefineEnum_NullName_ThrowsArgumentNullException()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            Assert.Throws<ArgumentNullException>("fullname", () => module.DefineEnum(null, TypeAttributes.Public, typeof(object)));
        }

        [Theory]
        [InlineData("")]
        public void DefineEnum_EmptyName_ThrowsArgumentNullException(string name)
        {
            ModuleBuilder module = Helpers.DynamicModule();
            Assert.Throws<ArgumentException>("fullname", () => module.DefineEnum(name, TypeAttributes.Public, typeof(object)));
        }

        [Theory]
        [InlineData(TypeAttributes.Abstract, "name")]
        [InlineData(TypeAttributes.AutoClass, "name")]
        [InlineData(TypeAttributes.BeforeFieldInit, "name")]
        [InlineData(TypeAttributes.ClassSemanticsMask, "name")]
        [InlineData(TypeAttributes.CustomFormatClass, "name")]
        [InlineData(TypeAttributes.CustomFormatMask, "name")]
        [InlineData(TypeAttributes.ExplicitLayout, "name")]
        [InlineData(TypeAttributes.HasSecurity, "name")]
        [InlineData(TypeAttributes.Import, "name")]
        [InlineData(TypeAttributes.ClassSemanticsMask, "name")]
        [InlineData(TypeAttributes.LayoutMask, "name")]
        [InlineData(TypeAttributes.RTSpecialName, "name")]
        [InlineData(TypeAttributes.Sealed, "name")]
        [InlineData(TypeAttributes.SequentialLayout, "name")]
        [InlineData(TypeAttributes.Serializable, "name")]
        [InlineData(TypeAttributes.SpecialName, "name")]
        [InlineData(TypeAttributes.CustomFormatClass, "name")]
        [InlineData(TypeAttributes.UnicodeClass, "name")]
        [InlineData(TypeAttributes.NestedAssembly, null)]
        [InlineData(TypeAttributes.NestedFamANDAssem, null)]
        [InlineData(TypeAttributes.NestedFamily, null)]
        [InlineData(TypeAttributes.NestedFamORAssem, null)]
        [InlineData(TypeAttributes.NestedPrivate, null)]
        [InlineData(TypeAttributes.NestedPublic, null)]
        public void DefineEnum_IncorrectVisibilityAttributes_ThrowsArgumentException(TypeAttributes visibility, string paramName)
        {
            ModuleBuilder module = Helpers.DynamicModule();
            Assert.Throws<ArgumentException>(paramName, () => module.DefineEnum("Enum", visibility, typeof(int)));
        }

        {
            ModuleBuilder module = Helpers.DynamicModule();
            EnumBuilder enumBuilder = module.DefineEnum("MyEnum", visibility, typeof(string));
            Assert.Throws<TypeLoadException>(() => enumBuilder.CreateTypeInfo().AsType());
        }

        {
        }

        {
        }

        [Theory]
        public void DefineEnum_InvalidUnderlyingType_ThrowsTypeLoadExceptionOnCreation(Type underlyingType)
        {
            ModuleBuilder module = Helpers.DynamicModule();
            EnumBuilder enumBuilder = module.DefineEnum("Name", TypeAttributes.Public, underlyingType);
            Assert.Equal(underlyingType, enumBuilder.UnderlyingField.FieldType);
            Assert.Throws<TypeLoadException>(() => enumBuilder.CreateTypeInfo().AsType());
        }
    }
}
