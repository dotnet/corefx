// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Xunit;

#pragma warning disable 0219  // field is never used

namespace System.Reflection.Tests
{
    public static class MethodBaseTests
    {
        [Fact]
        public static void Test_GetCurrentMethod()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            Assert.Equal("Test_GetCurrentMethod", m.Name);
            Assert.True(m.IsStatic);
            Assert.True(m.IsPublic);
            Assert.True(m.DeclaringType == typeof(MethodBaseTests));
        }

        [Fact]
        public static void Test_GetCurrentMethod_Inlineable()
        {
            // Verify that the result is not affected by inlining optimizations
            MethodBase m = GetCurrentMethod_InlineableWrapper();
            Assert.Equal("GetCurrentMethod_InlineableWrapper", m.Name);
            Assert.True(m.IsStatic);
            Assert.False(m.IsPublic);
            Assert.True(m.DeclaringType == typeof(MethodBaseTests));
        }

        private static MethodBase GetCurrentMethod_InlineableWrapper()
        {
            return MethodBase.GetCurrentMethod();
        }

        [Theory]
        [InlineData("MyOtherMethod", BindingFlags.Static | BindingFlags.Public, "MyOtherMethod", BindingFlags.Static | BindingFlags.Public, true)]  // Same methods
        [InlineData("MyOtherMethod", BindingFlags.Static | BindingFlags.Public, "MyOtherMethod", BindingFlags.Static | BindingFlags.NonPublic, false)]  // Two methods of the same name
        [InlineData("MyAnotherMethod", BindingFlags.Static | BindingFlags.NonPublic, "MyOtherMethod", BindingFlags.Static | BindingFlags.NonPublic, false)]  // Two similar methods with different names
        public static void TestEqualityMethods(string methodName1, BindingFlags bindingFlags1, string methodName2, BindingFlags bindingFlags2, bool expected)
        {
            MethodBase mb1 = typeof(MethodBaseTests).GetMethod(methodName1, bindingFlags1);
            MethodBase mb2 = typeof(MethodBaseTests).GetMethod(methodName2, bindingFlags2);
            Assert.Equal(expected, mb1 == mb2);
            Assert.NotEqual(expected, mb1 != mb2);
        }

        [Fact]
        public static void TestMethodBody()
        {
            MethodBase mbase = typeof(MethodBaseTests).GetMethod("MyOtherMethod", BindingFlags.Static | BindingFlags.Public);
            MethodBody mb = mbase.GetMethodBody();
            Assert.True(mb.InitLocals);  // local variables are initialized
#if DEBUG
            Assert.Equal(2, mb.MaxStackSize);
            Assert.Equal(3, mb.LocalVariables.Count);

            foreach (LocalVariableInfo lvi in mb.LocalVariables)
            {
                if (lvi.LocalIndex == 0) { Assert.Equal(typeof(int), lvi.LocalType); }
                if (lvi.LocalIndex == 1) { Assert.Equal(typeof(string), lvi.LocalType); }
                if (lvi.LocalIndex == 2) { Assert.Equal(typeof(bool), lvi.LocalType); }
            }
#else
            Assert.Equal(1, mb.MaxStackSize);
            Assert.Equal(2, mb.LocalVariables.Count);

            foreach (LocalVariableInfo lvi in mb.LocalVariables)
            {
                if (lvi.LocalIndex == 0) { Assert.Equal(typeof(int), lvi.LocalType); }
                if (lvi.LocalIndex == 1) { Assert.Equal(typeof(string), lvi.LocalType); }
            }
#endif
        }

        public static MethodBase MyFakeGenericMethod<T>()
        {
            MethodBase m = MethodBase.GetCurrentMethod();
            return m;
        }

        private static int MyAnotherMethod(int x)
        {
            return x+1;
        }

        private static int MyOtherMethod(int x)
        {
            return x+1;
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public static void MyOtherMethod(object arg)
        {
            int var1 = 2;
            string var2 = "I am a string";

            if (arg == null)
            {
                throw new ArgumentNullException("Input arg cannot be null.");
            }
        }
#pragma warning restore xUnit1013 // Public method should be marked as test

        [Fact]
        public static void Test_GetCurrentMethod_ConstructedGenericMethod()
        {
            MethodInfo mi = typeof(MethodBaseTests).GetMethod(nameof(MyFakeGenericMethod), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodBase m = mi.MakeGenericMethod(typeof(byte));

            Assert.Equal(nameof(MyFakeGenericMethod), m.Name);
            Assert.Equal(typeof(MethodBaseTests), m.ReflectedType);
            Assert.True(m.IsGenericMethod);
            Assert.False(m.IsGenericMethodDefinition);
            Assert.True(m.IsConstructedGenericMethod);
            Assert.Equal(1, m.GetGenericArguments().Length);
            Assert.Equal(typeof(byte), m.GetGenericArguments()[0]);
        }

        [Fact]
        public static void Test_GetCurrentMethod_GenericMethodDefinition()
        {
            MethodBase m = typeof(MethodBaseTests).GetMethod(nameof(MyFakeGenericMethod), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.Equal(nameof(MyFakeGenericMethod), m.Name);
            Assert.Equal(typeof(MethodBaseTests), m.ReflectedType);
            Assert.True(m.IsGenericMethod);
            Assert.True(m.IsGenericMethodDefinition);
            Assert.False(m.IsConstructedGenericMethod);
            Assert.Equal(1, m.GetGenericArguments().Length);
            Assert.Equal("T", m.GetGenericArguments()[0].Name);
        }
    }
}
