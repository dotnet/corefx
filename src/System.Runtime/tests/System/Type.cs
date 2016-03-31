// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using Xunit;

internal class Outside
{
    public class Inside
    { }
}

internal class Outside<T>
{
    public class Inside<U>
    { }
}

public static class TypeTests
{
    [Theory]
    [InlineData(typeof(int), null)]
    [InlineData(typeof(int[]), null)]
    [InlineData(typeof(Outside.Inside), typeof(Outside))]
    [InlineData(typeof(Outside.Inside[]), null)]
    [InlineData(typeof(Outside<int>), null)]
    [InlineData(typeof(Outside<int>.Inside<double>), typeof(Outside<>))]
    public static void TestDeclaringType(Type t, Type expected)
    {
        Assert.Equal(expected, t.DeclaringType);
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(IList<int>))]
    [InlineData(typeof(IList<>))]
    public static void TestGenericParameterPositionInvalid(Type t)
    {
        Assert.Throws<InvalidOperationException>(() => t.GenericParameterPosition);
    }

    [Theory]
    [InlineData(typeof(int), new Type[0])]
    [InlineData(typeof(IDictionary<int, string>), new[] { typeof(int), typeof(string) })]
    [InlineData(typeof(IList<int>), new[] { typeof(int) })]
    [InlineData(typeof(IList<>), new Type[0])]
    public static void TestGenericTypeArguments(Type t, Type[] expected)
    {
        Type[] result = t.GenericTypeArguments;
        Assert.Equal(expected.Length, result.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], result[i]);
        }
    }

    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), true)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IList<>), false)]
    public static void TestHasElementType(Type t, bool expected)
    {
        Assert.Equal(expected, t.HasElementType);
    }
    
    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), true)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IList<>), false)]
    public static void TestIsArray(Type t, bool expected)
    {
        Assert.Equal(expected, t.IsArray);
    }
    
    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IList<>), false)]
    public static void TestIsByRef(Type t, bool expected)
    {
        Assert.Equal(expected, t.IsByRef);
        Assert.True(t.MakeByRefType().IsByRef);
    }
    
    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IList<>), false)]
    [InlineData(typeof(int *), true)]
    public static void testIsPointer(Type t, bool expected)
    {
        Assert.Equal(expected, t.IsPointer);
        Assert.True(t.MakePointerType().IsPointer);
    }

    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(IList<int>), true)]
    [InlineData(typeof(IList<>), false)]
    public static void TestIsConstructedGenericType(Type t, bool expected)
    {
        Assert.Equal(expected, t.IsConstructedGenericType);
    }

    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IList<>), false)]
    public static void TestIsGenericParameter(Type t, bool expected)
    {
        Assert.Equal(expected, t.IsGenericParameter);
    }

    [Theory]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(Outside.Inside), true)]
    [InlineData(typeof(Outside.Inside[]), false)]
    [InlineData(typeof(Outside<int>), false)]
    [InlineData(typeof(Outside<int>.Inside<double>), true)]
    public static void TestIsNested(Type t, bool expected)
    {
        Assert.Equal(expected, t.IsNested);
    }

    [Theory]
    [InlineData(typeof(int), typeof(int))]
    [InlineData(typeof(int[]), typeof(int[]))]
    [InlineData(typeof(Outside<int>), typeof(Outside<int>))]
    public static void TestTypeHandle(Type t1, Type t2)
    {
        RuntimeTypeHandle r1 = t1.TypeHandle;
        RuntimeTypeHandle r2 = t2.TypeHandle;
        Assert.Equal(r1, r2);

        Assert.Equal(t1, Type.GetTypeFromHandle(r1));
        Assert.Equal(t1, Type.GetTypeFromHandle(r2));
    }

    [Fact]
    public static void TestGetTypeFromDefaultHandle()
    {
        Assert.Null(Type.GetTypeFromHandle(default(RuntimeTypeHandle)));
    }

    [Theory]
    [InlineData(typeof(int[]), 1)]
    [InlineData(typeof(int[,,]), 3)]
    public static void TestGetArrayRank(Type t, int expected)
    {
        Assert.Equal(expected, t.GetArrayRank());
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(IList<int>))]
    [InlineData(typeof(IList<>))]
    public static void TestGetArrayRankInvalid(Type t)
    {
        Assert.Throws<ArgumentException>(() => t.GetArrayRank());
    }

    [Theory]
    [InlineData(typeof(int), null)]
    [InlineData(typeof(Outside.Inside), null)]
    [InlineData(typeof(int[]), typeof(int))]
    [InlineData(typeof(Outside<int>.Inside<double>[]), typeof(Outside<int>.Inside<double>))]
    [InlineData(typeof(Outside<int>), null)]
    [InlineData(typeof(Outside<int>.Inside<double>), null)]
    public static void TestGetElementType(Type t, Type expected)
    {
        Assert.Equal(expected, t.GetElementType());
    }

    [Theory]
    [InlineData(typeof(int), typeof(int[]))]
    public static void TestMakeArrayType(Type t, Type tArrayExpected)
    {
        Type tArray = t.MakeArrayType();

        Assert.Equal(tArrayExpected, tArray);
        Assert.Equal(t, tArray.GetElementType());

        Assert.True(tArray.IsArray);
        Assert.True(tArray.HasElementType);

        string s1 = t.ToString();
        string s2 = tArray.ToString();
        Assert.Equal(s2, s1 + "[]");
    }

    [Theory]
    [InlineData(typeof(int))]
    public static void TestMakeByRefType(Type t)
    {
        Type tRef1 = t.MakeByRefType();
        Type tRef2 = t.MakeByRefType();

        Assert.Equal(tRef1, tRef2);

        Assert.True(tRef1.IsByRef);
        Assert.True(tRef1.HasElementType);

        Assert.Equal(t, tRef1.GetElementType());

        string s1 = t.ToString();
        string s2 = tRef1.ToString();
        Assert.Equal(s2, s1 + "&");
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
    [InlineData("Outside`1[System.Nullable`1[System.Boolean]]", typeof(Outside<bool?>))]
    public static void TestGetTypeByName(string typeName, Type expectedType)
    {
        Type t = Type.GetType(typeName, throwOnError: false, ignoreCase: false);
        Assert.Equal(expectedType, t);

        t = Type.GetType(typeName.ToLower(), throwOnError: false, ignoreCase: true);
        Assert.Equal(expectedType, t);
    }

    [Theory]
    [InlineData("system.nullable`1[system.int32]", typeof(TypeLoadException), false)]
    [InlineData("System.NonExistingType", typeof(TypeLoadException), false)]
    [InlineData("", typeof(TypeLoadException), false)]
    [InlineData("System.Int32[,*,]", typeof(ArgumentException), false)]
    [InlineData("Outside`2", typeof(TypeLoadException), false)]
    [InlineData("Outside`1[System.Boolean, System.Int32]", typeof(ArgumentException), true)]
    public static void TestGetTypeByNameInvalid(string typeName, Type expectedException, bool alwaysThrowsException)
    {
        if (!alwaysThrowsException)
        {
            Type t = Type.GetType(typeName, throwOnError: false, ignoreCase: false);
            Assert.Null(t);
        }

        Assert.Throws(expectedException, () => Type.GetType(typeName, throwOnError: true, ignoreCase: false));
    }

    [Fact]
    public static void Delimiter()
    {
        Assert.NotNull(Type.Delimiter);
    }

    [Theory]
    [InlineData(typeof(bool), TypeCode.Boolean)]
    [InlineData(typeof(byte), TypeCode.Byte)]
    [InlineData(typeof(char), TypeCode.Char)]
    [InlineData(typeof(DateTime), TypeCode.DateTime)]
    [InlineData(typeof(decimal), TypeCode.Decimal)]
    [InlineData(typeof(double), TypeCode.Double)]
    [InlineData(null, TypeCode.Empty)]
    [InlineData(typeof(short), TypeCode.Int16)]
    [InlineData(typeof(int), TypeCode.Int32)]
    [InlineData(typeof(long), TypeCode.Int64)]
    [InlineData(typeof(object), TypeCode.Object)]
    [InlineData(typeof(System.Nullable), TypeCode.Object)]
    [InlineData(typeof(Nullable<int>), TypeCode.Object)]
    [InlineData(typeof(Dictionary<,>), TypeCode.Object)]
    [InlineData(typeof(Exception), TypeCode.Object)]
    [InlineData(typeof(sbyte), TypeCode.SByte)]
    [InlineData(typeof(float), TypeCode.Single)]
    [InlineData(typeof(string), TypeCode.String)]
    [InlineData(typeof(ushort), TypeCode.UInt16)]
    [InlineData(typeof(uint), TypeCode.UInt32)]
    [InlineData(typeof(ulong), TypeCode.UInt64)]
    public static void GetTypeCode(Type t, TypeCode typeCode)
    {
        Assert.Equal(typeCode, Type.GetTypeCode(t));
    }
}
