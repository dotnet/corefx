// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.Tests.Common;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public partial class IsComObjectTests
    {
        public static IEnumerable<object[]> IsComObject_TestData()
        {
            yield return new object[] { new object() };
            yield return new object[] { 0 };
            yield return new object[] { "string" };

            yield return new object[] { new NonGenericClass() };
            yield return new object[] { new GenericClass<int>() };
            yield return new object[] { new Dictionary<string, int>() };
            yield return new object[] { new NonGenericStruct() };
            yield return new object[] { new GenericStruct<string>() };
            yield return new object[] { Int32Enum.Value1 };

            yield return new object[] { new int[] { 10 } };
            yield return new object[] { new int[][] { new int[] { 10 } } };
            yield return new object[] { new int[,] { { 10 } } };

            MethodInfo method = typeof(IsComObjectTests).GetMethod(nameof(NonGenericMethod));
            Delegate d = method.CreateDelegate(typeof(NonGenericDelegate));
            yield return new object[] { d };

            yield return new object[] { new KeyValuePair<string, int>("key", 10) };
        }

        [Theory]
        [MemberData(nameof(IsComObject_TestData))]
        public void IsComObject_NonComObject_ReturnsFalse(object value)
        {
            Assert.False(Marshal.IsComObject(value));
        }

        [Fact]
        public void IsComObject_NullObject_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("o", () => Marshal.IsComObject(null));
        }

        public static void NonGenericMethod(int i) { }
        public delegate void NonGenericDelegate(int i);

        public enum Int32Enum : int { Value1, Value2 }
    }
}
