// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class TypeTests
    {
        [Fact]
        public static void TestEmptyTypes()
        {
            Assert.NotNull(Type.EmptyTypes);
            Assert.Same(Type.EmptyTypes, Type.EmptyTypes);
            Assert.Empty(Type.EmptyTypes);
        }

        [Fact]
        public static void TestMissing()
        {
            Assert.NotNull(Type.Missing);
            Assert.Same(Type.Missing, Type.Missing);
        }

        [Theory]
        [InlineData(typeof(int), null)]
        [InlineData(typeof(int[]), null)]
        [InlineData(typeof(int*), null)]
        [InlineData(typeof(Outside.Inside), typeof(Outside))]
        [InlineData(typeof(Outside.Inside[]), null)]
        [InlineData(typeof(Outside<int>), null)]
        [InlineData(typeof(Outside<int>.Inside<double>), typeof(Outside<>))]
        public static void TestDeclaringType(Type type, Type expected)
        {
            Assert.Equal(expected, type.DeclaringType);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(int*))]
        [InlineData(typeof(IList<int>))]
        [InlineData(typeof(IList<>))]
        public static void TestGenericParameterPosition_Invalid(Type type)
        {
            Assert.Throws<InvalidOperationException>(() => type.GenericParameterPosition);
        }

        [Theory]
        [InlineData(typeof(int), new Type[0])]
        [InlineData(typeof(int[]), new Type[0])]
        [InlineData(typeof(int*), new Type[0])]
        [InlineData(typeof(IDictionary<int, string>), new Type[] { typeof(int), typeof(string) })]
        [InlineData(typeof(IList<int>), new Type[] { typeof(int) })]
        [InlineData(typeof(IList<>), new Type[0])]
        public static void TestGenericTypeArguments(Type type, Type[] expected)
        {
            Assert.Equal(expected, type.GenericTypeArguments);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(int*), true)]
        [InlineData(typeof(IList<int>), false)]
        [InlineData(typeof(IList<>), false)]
        public static void TestHasElementType(Type type, bool expected)
        {
            Assert.Equal(expected, type.HasElementType);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(int*), false)]
        [InlineData(typeof(IList<int>), false)]
        [InlineData(typeof(IList<>), false)]
        public static void TestIsArray(Type type, bool expected)
        {
            Assert.Equal(expected, type.IsArray);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), false)]
        [InlineData(typeof(int*), false)]
        [InlineData(typeof(IList<int>), false)]
        [InlineData(typeof(IList<>), false)]
        public static void TestIsByRef(Type type, bool expected)
        {
            Assert.Equal(expected, type.IsByRef);
            Assert.True(type.MakeByRefType().IsByRef);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), false)]
        [InlineData(typeof(int*), false)]
        [InlineData(typeof(IList<int>), true)]
        [InlineData(typeof(IList<>), false)]
        public static void TestIsConstructedGenericType(Type type, bool expected)
        {
            Assert.Equal(expected, type.IsConstructedGenericType);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), false)]
        [InlineData(typeof(int*), false)]
        [InlineData(typeof(IList<int>), false)]
        [InlineData(typeof(IList<>), false)]
        public static void TestIsGenericParameter(Type type, bool expected)
        {
            Assert.Equal(expected, type.IsGenericParameter);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), false)]
        [InlineData(typeof(int*), false)]
        [InlineData(typeof(Outside.Inside), true)]
        [InlineData(typeof(Outside.Inside[]), false)]
        [InlineData(typeof(Outside<int>), false)]
        [InlineData(typeof(Outside<int>.Inside<double>), true)]
        public static void TestIsNested(Type type, bool expected)
        {
            Assert.Equal(expected, type.IsNested);
        }

        [Theory]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int[]), false)]
        [InlineData(typeof(int*), true)]
        [InlineData(typeof(IList<int>), false)]
        [InlineData(typeof(IList<>), false)]
        public static void TestIsPointer(Type type, bool expected)
        {
            Assert.Equal(expected, type.IsPointer);
            Assert.True(type.MakePointerType().IsPointer);
        }

        [Theory]
        [InlineData("System.Nullable`1[System.Int32]", typeof(int?))]
        [InlineData("System.Int32*", typeof(int*))]
        [InlineData("System.Int32**", typeof(int**))]
        [InlineData("System.Runtime.Tests.Outside`1", typeof(Outside<>))]
        [InlineData("System.Runtime.Tests.Outside`1+Inside`1", typeof(Outside<>.Inside<>))]
        [InlineData("System.Runtime.Tests.Outside[]", typeof(Outside[]))]
        [InlineData("System.Runtime.Tests.Outside[,,]", typeof(Outside[,,]))]
        [InlineData("System.Runtime.Tests.Outside[][]", typeof(Outside[][]))]
        [InlineData("System.Runtime.Tests.Outside`1[System.Nullable`1[System.Boolean]]", typeof(Outside<bool?>))]
        public static void TestGetType(string typeName, Type expectedType)
        {
            Assert.Equal(expectedType, Type.GetType(typeName, throwOnError: false, ignoreCase: false));
            Assert.Equal(expectedType, Type.GetType(typeName.ToLower(), throwOnError: false, ignoreCase: true));
        }

        [Theory]
        [InlineData("system.nullable`1[system.int32]", typeof(TypeLoadException), false)]
        [InlineData("System.NonExistingType", typeof(TypeLoadException), false)]
        [InlineData("", typeof(TypeLoadException), false)]
        [InlineData("System.Int32[,*,]", typeof(ArgumentException), false)]
        [InlineData("System.Runtime.Tests.Outside`2", typeof(TypeLoadException), false)]
        [InlineData("System.Runtime.Tests.Outside`1[System.Boolean, System.Int32]", typeof(ArgumentException), true)]
        public static void TestGetType_Invalid(string typeName, Type expectedException, bool alwaysThrowsException)
        {
            if (!alwaysThrowsException)
            {
                Assert.Null(Type.GetType(typeName, throwOnError: false, ignoreCase: false));
            }
            Assert.Throws(expectedException, () => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
        }

        [Theory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(int[]), typeof(int[]))]
        [InlineData(typeof(int*), typeof(int*))]
        [InlineData(typeof(Outside<int>), typeof(Outside<int>))]
        public static void TestTypeHandle(Type type1, Type type2)
        {
            RuntimeTypeHandle typeHandle1 = type1.TypeHandle;
            RuntimeTypeHandle typeHandle2 = type2.TypeHandle;
            Assert.Equal(typeHandle1, typeHandle2);

            Assert.Equal(type1, Type.GetTypeFromHandle(typeHandle1));
            Assert.Equal(type1, Type.GetTypeFromHandle(typeHandle2));
        }

        [Fact]
        public static void TestGetTypeFromHandle_DefaultHandle()
        {
            Assert.Null(Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
        }

        [Theory]
        [InlineData(typeof(int[]), 1)]
        [InlineData(typeof(int[,,]), 3)]
        public static void TestGetArrayRank(Type type, int expected)
        {
            Assert.Equal(expected, type.GetArrayRank());
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(int*))]
        [InlineData(typeof(IList<int>))]
        [InlineData(typeof(IList<>))]
        public static void TestGetArrayRank_Invalid(Type type)
        {
            Assert.Throws<ArgumentException>(null, () => type.GetArrayRank());
        }

        [Theory]
        [InlineData(typeof(int), null)]
        [InlineData(typeof(int*), typeof(int))]
        [InlineData(typeof(int[]), typeof(int))]
        [InlineData(typeof(Outside.Inside), null)]
        [InlineData(typeof(Outside<int>.Inside<double>[]), typeof(Outside<int>.Inside<double>))]
        [InlineData(typeof(Outside<int>), null)]
        [InlineData(typeof(Outside<int>.Inside<double>), null)]
        public static void TestGetElementType(Type type, Type expected)
        {
            Assert.Equal(expected, type.GetElementType());
        }

        [Theory]
        [InlineData(typeof(int), typeof(int[]))]
        public static void TestMakeArrayType(Type type, Type tArrayExpected)
        {
            Type arrayType = type.MakeArrayType();

            Assert.Equal(tArrayExpected, arrayType);
            Assert.Equal(type, arrayType.GetElementType());

            Assert.True(arrayType.IsArray);
            Assert.True(arrayType.HasElementType);
            
            Assert.Equal(arrayType.ToString(), type.ToString() + "[]");
        }

        [Theory]
        [InlineData(typeof(int))]
        public static void TestMakeByRefType(Type type)
        {
            Type referenceType3 = type.MakeByRefType();
            Type referenceType2 = type.MakeByRefType();

            Assert.Equal(referenceType3, referenceType2);

            Assert.True(referenceType3.IsByRef);
            Assert.True(referenceType3.HasElementType);

            Assert.Equal(type, referenceType3.GetElementType());
            
            Assert.Equal(referenceType3.ToString(), type.ToString() + "&");
        }
    }

    public class Outside
    {
        public class Inside
        {
        }
    }

    public class Outside<T>
    {
        public class Inside<U>
        {
        }
    }
}
