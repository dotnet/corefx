﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Reflection.Emit.Tests
{
    public class ParameterBuilderSetConstant
    {
        public static IEnumerable<object[]> SetConstant_ReferenceTypes_TestData()
        {
            yield return new object[] { typeof(object) };
            yield return new object[] { typeof(string) };
            yield return new object[] { typeof(UserDefinedClass) };
        }

        public static IEnumerable<object[]> SetConstant_NullableValueTypes_TestData()
        {
            yield return new object[] { typeof(bool?) };
            yield return new object[] { typeof(byte?) };
            yield return new object[] { typeof(char?) };
            yield return new object[] { typeof(DateTime?) };
            yield return new object[] { typeof(decimal?) };
            yield return new object[] { typeof(double?) };
            yield return new object[] { typeof(float?) };
            yield return new object[] { typeof(int?) };
            yield return new object[] { typeof(long?) };
            yield return new object[] { typeof(sbyte?) };
            yield return new object[] { typeof(short?) };
            yield return new object[] { typeof(uint?) };
            yield return new object[] { typeof(ulong?) };
            yield return new object[] { typeof(UserDefinedStruct?) };
            yield return new object[] { typeof(ushort?) };
        }

        public static IEnumerable<object[]> SetConstant_ValueTypes_TestData()
        {
            yield return new object[] { typeof(bool) };
            yield return new object[] { typeof(byte) };
            yield return new object[] { typeof(char) };
            yield return new object[] { typeof(DateTime) };
            yield return new object[] { typeof(decimal) };
            yield return new object[] { typeof(double) };
            yield return new object[] { typeof(float) };
            yield return new object[] { typeof(int) };
            yield return new object[] { typeof(long) };
            yield return new object[] { typeof(sbyte) };
            yield return new object[] { typeof(short) };
            yield return new object[] { typeof(uint) };
            yield return new object[] { typeof(ulong) };
            yield return new object[] { typeof(UserDefinedStruct) };
            yield return new object[] { typeof(ushort) };
        }

        [Theory]
        [MemberData(nameof(SetConstant_ReferenceTypes_TestData))]
        [MemberData(nameof(SetConstant_NullableValueTypes_TestData))]
        [MemberData(nameof(SetConstant_ValueTypes_TestData))]
        public void SetConstant_Null(Type parameterType)
        {
            TypeBuilder type = Helpers.DynamicType(TypeAttributes.Interface | TypeAttributes.Abstract);
            MethodBuilder method = type.DefineMethod("TestMethod", MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual, typeof(void), new Type[] { parameterType });
            ParameterBuilder parameter = method.DefineParameter(1, ParameterAttributes.Optional | ParameterAttributes.HasDefault, "arg");

            parameter.SetConstant(null);

            ParameterInfo createdParameter = GetCreatedParameter(type, "TestMethod", 1);
            Assert.Equal(true, createdParameter.HasDefaultValue);
            Assert.Equal(null, createdParameter.DefaultValue);
        }

        private static ParameterInfo GetCreatedParameter(TypeBuilder type, string methodName, int parameterIndex)
        {
            Type createdType = type.CreateTypeInfo().AsType();
            MethodInfo createdMethod = createdType.GetMethod(methodName);
            if (parameterIndex > 0)
            {
                return createdMethod.GetParameters()[parameterIndex - 1];
            }
            else
            {
                return createdMethod.ReturnParameter;
            }
        }

        private class UserDefinedClass
        {
        }

        private struct UserDefinedStruct
        {
        }
    }
}
