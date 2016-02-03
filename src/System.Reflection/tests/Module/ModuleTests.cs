// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.CustomAttributesTests.Data;

[module: Attr(77, name = "AttrSimple")]
[module: Int32Attr(77, name = "Int32AttrSimple")]
[module: Int64Attr((Int64)77, name = "Int64AttrSimple")]
[module: StringAttr("hello", name = "StringAttrSimple")]
[module: EnumAttr(MyColorEnum.GREEN, name = "EnumAttrSimple")]


namespace System.Reflection.Tests
{
    public class ModuleTest
    {
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(List<>))]
        [InlineData(typeof(ModuleTest))]
        public static void AssemblyTest(TypeInfo typeInfo)
        {
            Module module = typeInfo.Module;
            Assert.Equal(typeInfo.Assembly, module.Assembly);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(List<>))]
        [InlineData(typeof(ModuleTest))]
        public static void FullyQualifiedNameTest(TypeInfo typeInfo)
        {
            Module module = typeInfo.Module;
            Assert.Contains(typeInfo.Assembly.GetName().Name, module.FullyQualifiedName, StringComparison.CurrentCultureIgnoreCase);
            Assert.Contains(typeInfo.Assembly.GetName().Name, module.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        [Theory]
        [InlineData(typeof(Attr), 77, "AttrSimple")]
        [InlineData(typeof(Int32Attr), 77, "Int32AttrSimple")]
        [InlineData(typeof(Int64Attr), (Int64)77, "Int64AttrSimple")]
        [InlineData(typeof(StringAttr), "hello", "StringAttrSimple")]
        [InlineData(typeof(EnumAttr), MyColorEnum.GREEN, "EnumAttrSimple")]        
        public static void CustomAttributesTest<CtorArg, NamedArg>(Type attrType, CtorArg expectedCtorValue, NamedArg expectedNamedValue)
        {
            Module module = typeof(ModuleTest).GetTypeInfo().Module;
            var customAttrs = module.CustomAttributes.GetEnumerator();

            CustomAttributeData attribute = module.CustomAttributes.SingleOrDefault(a => a.AttributeType.Equals(attrType));

            Assert.NotNull(attribute);

            Assert.Equal(1, attribute.ConstructorArguments.Count);
            Assert.Equal(1, attribute.NamedArguments.Count);
                        
            Assert.Equal(typeof(CtorArg), attribute.ConstructorArguments[0].ArgumentType);

            var actualCtorValue = attribute.ConstructorArguments[0].Value;
            if (typeof(CtorArg).GetTypeInfo().IsEnum)
            {
                actualCtorValue = Enum.ToObject(typeof(CtorArg), attribute.ConstructorArguments[0].Value);
            }

            Assert.Equal(expectedCtorValue, actualCtorValue);

            Assert.Equal("name", attribute.NamedArguments[0].MemberName);
            Assert.True(attribute.NamedArguments[0].IsField);

            Assert.Equal(typeof(NamedArg), attribute.NamedArguments[0].TypedValue.ArgumentType);
            Assert.Equal(expectedNamedValue, attribute.NamedArguments[0].TypedValue.Value);
        }

        [Theory]        
        [InlineData("System.Nullable`1[System.Int32]", typeof(int?))]
        [InlineData("System.Int32*", typeof(int*))]
        [InlineData("System.Int32**", typeof(int**))]
        [InlineData("Outside`1", typeof(Outside<>))]
        [InlineData("Outside`1+Inside`1", typeof(Outside<>.Inside<>))]
        [InlineData("Outside[]", typeof(Outside[]))]
        [InlineData("Outside[,,]", typeof(Outside[,,]))]
        [InlineData("Outside[][]", typeof(Outside[][]))]        
        public static void GetTypeTest(string className, Type expectedType)
        {
            Module module = expectedType.GetTypeInfo().Module;

            Type actualType = module.GetType(className, true, false);
            Assert.Equal(expectedType, actualType);

            actualType = module.GetType(className.ToLower(), false, true);
            Assert.Equal(expectedType, actualType);

            actualType = module.GetType(className.ToLower(), false, false);
            Assert.Null(actualType);

            Assert.Throws<TypeLoadException>(() => module.GetType(className.ToLower(), true, false));
        }
    }
}


public class Outside
{
    public class Inside
    { }
}

public class Outside<T>
{
    public class Inside<U>
    { }
}
