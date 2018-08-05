// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.Tests.Common;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public partial class GetObjectForIUnknownTests
    {
        public static IEnumerable<object[]> GetObjectForIUnknown_Valid_TestData()
        {
            yield return new object[] { new object() };
            yield return new object[] { 10 };
            yield return new object[] { "string" };

            yield return new object[] { new NonGenericClass() };
            yield return new object[] { new GenericClass<string>() };
            yield return new object[] { new Dictionary<string, int>() };
            yield return new object[] { new NonGenericStruct() };
            yield return new object[] { new GenericStruct<string>() };
            yield return new object[] { Int32Enum.Value1 };

            yield return new object[] { new int[] { 10 } };
            yield return new object[] { new int[][] { new int[] { 10 } } };
            yield return new object[] { new int[,] { { 10 } } };

            MethodInfo method = typeof(GetObjectForIUnknownTests).GetMethod(nameof(NonGenericMethod));
            Delegate d = method.CreateDelegate(typeof(NonGenericDelegate));
            yield return new object[] { d };

            yield return new object[] { new KeyValuePair<string, int>("key", 10) };
        }

        [Theory]
        [MemberData(nameof(GetObjectForIUnknown_Valid_TestData))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetObjectForIUnknown_ValidPointer_ReturnsExpected(object o)
        {
            IntPtr ptr = Marshal.GetIUnknownForObject(o);
            try
            {
                Assert.NotEqual(IntPtr.Zero, ptr);
                Assert.Equal(o, Marshal.GetObjectForIUnknown(ptr));
            }
            finally
            {
                Marshal.Release(ptr);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void GetObjectForIUnknown_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetObjectForIUnknown(IntPtr.Zero));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetObjectForIUnknown_NullPointer_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("pUnk", () => Marshal.GetObjectForIUnknown(IntPtr.Zero));
        }

        public static void NonGenericMethod(int i) { }
        public delegate void NonGenericDelegate(int i);

        public enum Int32Enum : int { Value1, Value2 }
    }
}
