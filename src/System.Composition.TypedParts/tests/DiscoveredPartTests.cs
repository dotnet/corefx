// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Collections.Generic;
using Xunit;

namespace System.Composition.TypedParts.Tests
{
    public static class DiscoveredPartTests
    {
        [Fact]
        public static void ParameterInfoComparer_GetHashCode()
        {
            var comparerType = Type.GetType("System.Composition.TypedParts.Discovery.DiscoveredType.ParameterInfoComparer");
            FieldInfo instanceProperty = comparerType.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            var sut = (IEqualityComparer<ParameterInfo>)instanceProperty.GetValue(null);

            var testsType = typeof(DiscoveredPartTests);
            MethodInfo method1 = testsType.GetMethod(nameof(Method1), BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo method2 = testsType.GetMethod(nameof(Method2), BindingFlags.NonPublic | BindingFlags.Static);
            ParameterInfo[] method1Parameters = method1.GetParameters();
            ParameterInfo[] method2Parameters = method2.GetParameters();

            Assert.True(sut.Equals(method1Parameters[0], method1Parameters[0]));
            Assert.Equal(sut.GetHashCode(method1Parameters[0]), sut.GetHashCode(method1Parameters[0]));

            Assert.False(sut.Equals(method1Parameters[0], method1Parameters[1]));
            Assert.NotEqual(sut.GetHashCode(method1Parameters[0]), sut.GetHashCode(method1Parameters[1]));

            Assert.False(sut.Equals(method1Parameters[0], method2Parameters[0]));
            Assert.NotEqual(sut.GetHashCode(method1Parameters[0]), sut.GetHashCode(method2Parameters[0]));
        }

        private static void Method1(int param0, int param1) => throw null;
        private static void Method2(int param0, int param1) => throw null;
    }
}
