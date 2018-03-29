// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.SpanTests
{
    public static partial class SpanTests
    {
        // Calling Span APIs via Reflection is not supported yet.
        // These tests check that using reflection results in graceful failures. See https://github.com/dotnet/coreclr/issues/17296

        [Fact]
        public static void MemoryExtensions_AsSpan()
        {
            Type type = typeof(MemoryExtensions);

            Reflection.MethodInfo method = type.GetMethod("AsSpan", new Type[] { typeof(string) });
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { "Hello" }));

            method = type.GetMethod("AsSpan", new Type[] { typeof(string), typeof(int) });
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { "Hello", 1 }));

            method = type.GetMethod("AsSpan", new Type[] { typeof(string), typeof(int), typeof(int) });
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { "Hello", 1, 1 }));
        }

        [Fact]
        public static void MemoryExtensions_CompareTo()
        {
            Type type = typeof(MemoryExtensions);

            Reflection.MethodInfo method = type.GetMethod("CompareTo");
            Console.WriteLine(method.Name + " : " + method.ToString());

            ReadOnlySpan<char> span = new char[10];
            ReadOnlySpan<char> other = new char[10];
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { default, default, StringComparison.Ordinal }));
        }
    }
}
