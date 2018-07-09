// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Runtime.InteropServices
{
    public partial class MarshalTests
    {
        public static IEnumerable<object[]> IsComObject_Windows_TestData()
        {
            yield return new object[] { new ComImportObject(), true };
            yield return new object[] { new SubComImportObject(), true };
            yield return new object[] { new InterfaceAndComImportObject(), true };
            yield return new object[] { new InterfaceComImportObject(), false };
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotWindowsNanoServer))]
        [MemberData(nameof(IsComObject_Windows_TestData))]
        [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Not approved COM object for app")]
        public void IsComObject_Windows_ReturnsExpected(object value, bool expected)
        {
            Assert.Equal(expected, Marshal.IsComObject(value));
        }

        [Fact]
        public void GenerateGuidForType_ComObject_ReturnsComGuid()
        {
            Assert.Equal(new Guid("927971f5-0939-11d1-8be1-00c04fd8d503"), Marshal.GenerateGuidForType(typeof(ComImportObject)));
        }

        [Fact]
        public void GenerateProgIdForType_ImportType_ThrowsArgumentException()
        {
            AssertExtensions.Throws<ArgumentException>("type", () => Marshal.GenerateProgIdForType(typeof(ComImportObject)));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetComObjectData is not implemented in .NET Core.")]
        public void GetComObjectData_NetFramework_ReturnsExpected()
        {
            Type type = Type.GetTypeFromCLSID(new Guid("927971f5-0939-11d1-8be1-00c04fd8d503"));
            object comObject = Activator.CreateInstance(type);

            Assert.Null(Marshal.GetComObjectData(comObject, "key"));

            Marshal.SetComObjectData(comObject, "key", 1);
            Assert.Equal(1, Marshal.GetComObjectData(comObject, "key"));
            Assert.Null(Marshal.GetComObjectData(comObject, "noSuchKey"));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetIDispatchForObject is not implemented in .NET Core.")]
        public static void GetIDispatchForObject_ComObject_ReturnsNonZero()
        {
            Type type = Type.GetTypeFromCLSID(new Guid("927971f5-0939-11d1-8be1-00c04fd8d503"));
            object comObject = Activator.CreateInstance(type);

            IntPtr iDispatch = Marshal.GetIDispatchForObject(comObject);
            Assert.NotEqual(IntPtr.Zero, iDispatch);
            Marshal.Release(iDispatch);
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.SetComObjectData is not implemented in .NET Core.")]
        public void SetComObjectData_NetFramework_ReturnsExpected()
        {
            Type type = Type.GetTypeFromCLSID(new Guid("927971f5-0939-11d1-8be1-00c04fd8d503"));
            object comObject = Activator.CreateInstance(type);

            Assert.True(Marshal.SetComObjectData(comObject, "key", 1));
            Assert.Equal(1, Marshal.GetComObjectData(comObject, "key"));

            Assert.False(Marshal.SetComObjectData(comObject, "key", 2));
            Assert.Equal(1, Marshal.GetComObjectData(comObject, "key"));

            Assert.True(Marshal.SetComObjectData(comObject, "otherKey", 2));
            Assert.Equal(2, Marshal.GetComObjectData(comObject, "otherKey"));
        }
    }

    [ComImport]
    [Guid("927971f5-0939-11d1-8be1-00c04fd8d503")]
    public interface IComImportObject { }

    public class InterfaceComImportObject : IComImportObject { }

    [ComImport]
    [Guid("927971f5-0939-11d1-8be1-00c04fd8d503")]
    public class InterfaceAndComImportObject : IComImportObject { }

    [ComImport]
    [Guid("927971f5-0939-11d1-8be1-00c04fd8d503")]
    public class ComImportObject { }

    public class SubComImportObject : ComImportObject { }
}
