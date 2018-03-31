// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Reflection;
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

            MethodInfo method = type.GetMethod("AsSpan", new Type[] { typeof(string) });
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

            MethodInfo method = type.GetMethod("CompareTo");

            int result = (int)method.Invoke(null, new object[] { default, default, StringComparison.Ordinal });
            Assert.Equal(0, result);
        }

        [Fact]
        public static void MemoryExtensions_Contains()
        {
            Type type = typeof(MemoryExtensions);

            MethodInfo method = type.GetMethod("Contains");

            bool result = (bool)method.Invoke(null, new object[] { default, default, StringComparison.Ordinal });
            Assert.True(result);
        }

        [Fact]
        public static void MemoryExtensions_IsWhiteSpace()
        {
            Type type = typeof(MemoryExtensions);

            MethodInfo method = type.GetMethod("IsWhiteSpace");

            bool result = (bool)method.Invoke(null, new object[] { default });
            Assert.True(result);
        }

        [Fact]
        public static void MemoryExtensions_ToLower()
        {
            Type type = typeof(MemoryExtensions);

            MethodInfo method = type.GetMethod("ToLower");

            int result = (int)method.Invoke(null, new object[] { default, default, CultureInfo.InvariantCulture });
            Assert.Equal(0, result);
        }

        [Fact]
        public static void MemoryExtensions_ToLowerInvariant()
        {
            Type type = typeof(MemoryExtensions);

            MethodInfo method = type.GetMethod("ToLowerInvariant");

            int result = (int)method.Invoke(null, new object[] { default, default });
            Assert.Equal(0, result);
        }

        [Fact]
        public static void MemoryExtensions_ToUpper()
        {
            Type type = typeof(MemoryExtensions);

            MethodInfo method = type.GetMethod("ToUpper");

            int result = (int)method.Invoke(null, new object[] { default, default, CultureInfo.InvariantCulture });
            Assert.Equal(0, result);
        }

        [Fact]
        public static void MemoryExtensions_ToUpperInvariant()
        {
            Type type = typeof(MemoryExtensions);

            MethodInfo method = type.GetMethod("ToUpperInvariant");

            int result = (int)method.Invoke(null, new object[] { default, default });
            Assert.Equal(0, result);
        }

        [Fact]
        public static void Span_Ctor()
        {
            Type type = typeof(Span<int>);

            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(int[]) });
            Assert.Throws<TargetException>(() => ctor.Invoke(new object[] { new int[10] }));

            ctor = type.GetConstructor(new Type[] { typeof(int[]), typeof(int), typeof(int) });
            Assert.Throws<TargetException>(() => ctor.Invoke(new object[] { new int[10], 1, 1 }));

            ctor = type.GetConstructor(new Type[] { typeof(void*), typeof(int) });
            Assert.Throws<TargetException>(() => ctor.Invoke(new object[] { null, 1 }));
        }

        [Fact]
        public static void Span_Empty()
        {
            Type type = typeof(Span<int>);

            PropertyInfo property = type.GetProperty("Empty");
            Assert.Throws<NotSupportedException>(() => property.GetValue(null));
        }

        [Fact]
        public static void Span_Equality()
        {
            Type type = typeof(Span<int>);

            MethodInfo method = type.GetMethod("op_Equality");
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { default }));

            method = type.GetMethod("op_Inequality");
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { default }));
        }

        [Fact]
        public static void Memory_Span()
        {
            Type type = typeof(Memory<int>);

            PropertyInfo property = type.GetProperty("Span");
            Assert.Throws<NotSupportedException>(() => property.GetValue(null));
        }

        [Fact]
        public static void ReadOnlySpan_Ctor()
        {
            Type type = typeof(ReadOnlySpan<int>);

            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(int[]) });
            Assert.Throws<TargetException>(() => ctor.Invoke(new object[] { new int[10] }));

            ctor = type.GetConstructor(new Type[] { typeof(int[]), typeof(int), typeof(int) });
            Assert.Throws<TargetException>(() => ctor.Invoke(new object[] { new int[10], 1, 1 }));

            ctor = type.GetConstructor(new Type[] { typeof(void*), typeof(int) });
            Assert.Throws<TargetException>(() => ctor.Invoke(new object[] { null, 1 }));
        }

        [Fact]
        public static void ReadOnlySpan_Empty()
        {
            Type type = typeof(ReadOnlySpan<int>);

            PropertyInfo property = type.GetProperty("Empty");
            Assert.Throws<NotSupportedException>(() => property.GetValue(null));
        }

        [Fact]
        public static void ReadOnlySpan_Equality()
        {
            Type type = typeof(ReadOnlySpan<int>);

            MethodInfo method = type.GetMethod("op_Equality");
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { default }));

            method = type.GetMethod("op_Inequality");
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, new object[] { default }));
        }

        [Fact]
        public static void Memory_ReadOnlySpan()
        {
            Type type = typeof(ReadOnlyMemory<int>);

            PropertyInfo property = type.GetProperty("Span");
            Assert.Throws<NotSupportedException>(() => property.GetValue(null));
        }

        [Fact]
        public static void MemoryManager_GetSpan()
        {
            Type type = typeof(MemoryManager<int>);

            MethodInfo method = type.GetMethod("GetSpan");
            Assert.Throws<NotSupportedException>(() => method.Invoke(null, null));
        }

        [Fact]
        public static void BinaryPrimitives_ReadInt16LittleEndian()
        {
            Type type = typeof(BinaryPrimitives);

            MethodInfo method = type.GetMethod("ReadInt16LittleEndian");
            Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { default }));

            method = type.GetMethod("TryReadInt16LittleEndian");
            bool result = (bool)method.Invoke(null, new object[] { default, null });
            Assert.False(result);
        }
    }
}
