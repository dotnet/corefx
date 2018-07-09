// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using Xunit;

namespace System.Runtime.InteropServices
{
    public partial class MarshalTests
    {
        public static readonly object[][] StringData =
        {
            new object[] { "pizza" },
            new object[] { "pepperoni" },
            new object[] { "password" },
            new object[] { "P4ssw0rdAa1!" },
        };

        private static SecureString ToSecureString(string data)
        {
            var str = new SecureString();
            foreach (char c in data)
            {
                str.AppendChar(c);
            }
            str.MakeReadOnly();
            return str;
        }

        [Theory]
        [MemberData(nameof(StringData))]
        [PlatformSpecific(TestPlatforms.Windows)]  // SecureStringToBSTR not supported on Unix
        public void SecureStringToBSTR_InvokePtrToStringBSTR_ReturnsExpected(string data)
        {
            using (SecureString secureString = ToSecureString(data))
            {
                IntPtr bstr = Marshal.SecureStringToBSTR(secureString);
                try
                {
                    string actual = Marshal.PtrToStringBSTR(bstr);
                    Assert.Equal(data, actual);
                }
                finally
                {
                    Marshal.ZeroFreeBSTR(bstr);
                }
            }
        }

        [Fact]
        public void SecureStringToBSTR_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => Marshal.SecureStringToBSTR(null));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void SecureStringToCoTaskMemAnsi_InvokePtrToStringAnsi_Roundtrips(string data)
        {
            using (SecureString secureString = ToSecureString(data))
            {
                IntPtr ptr = Marshal.SecureStringToCoTaskMemAnsi(secureString);
                try
                {
                    string actual = Marshal.PtrToStringAnsi(ptr);
                    Assert.Equal(data, actual);
                }
                finally
                {
                    Marshal.ZeroFreeCoTaskMemAnsi(ptr);
                }
            }
        }

        [Fact]
        public void SecureStringToCoTaskMemAnsi_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => Marshal.SecureStringToCoTaskMemAnsi(null));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void SecureStringToCoTaskMemUnicode_PtrToStringUni_Roundtrips(string data)
        {
            using (SecureString secureString = ToSecureString(data))
            {
                IntPtr ptr = Marshal.SecureStringToCoTaskMemUnicode(secureString);
                try
                {
                    string actual = Marshal.PtrToStringUni(ptr);
                    Assert.Equal(data, actual);
                }
                finally
                {
                    Marshal.ZeroFreeCoTaskMemUnicode(ptr);
                }
            }
        }

        [Fact]
        public void SecureStringToCoTaskMemUnicode_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => Marshal.SecureStringToCoTaskMemUnicode(null));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void SecureStringToGlobalAllocAnsi_InvokePtrToStringAnsi_Roundtrips(string data)
        {
            using (SecureString secureString = ToSecureString(data))
            {
                IntPtr ptr = Marshal.SecureStringToGlobalAllocAnsi(secureString);
                try
                {
                    string actual = Marshal.PtrToStringAnsi(ptr);
                    Assert.Equal(data, actual);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocAnsi(ptr);
                }
            }
        }

        [Fact]
        public void SecureStringToGlobalAllocAnsi_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => Marshal.SecureStringToGlobalAllocAnsi(null));
        }

        [Theory]
        [MemberData(nameof(StringData))]
        public void SecureStringToGlobalAllocUnicode_InvokePtrToStringUni_Roundtrips(string data)
        {
            using (SecureString secureString = ToSecureString(data))
            {
                IntPtr ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                try
                {
                    string actual = Marshal.PtrToStringUni(ptr);
                    Assert.Equal(data, actual);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }
        }

        [Fact]
        public void SecureStringToGlobalAllocUnicode_NullString_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("s", () => Marshal.SecureStringToGlobalAllocUnicode(null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AllocHGlobal_Int32_ReadableWritable(int size)
        {
            IntPtr p = Marshal.AllocHGlobal(size);
            Assert.NotEqual(IntPtr.Zero, p);
            try
            {
                WriteBytes(p, size);
                VerifyBytes(p, size);
            }
            finally
            {
                Marshal.FreeHGlobal(p);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AllocHGlobal_IntPtr_ReadableWritable(int size)
        {
            IntPtr p = Marshal.AllocHGlobal((IntPtr)size);
            Assert.NotEqual(IntPtr.Zero, p);
            try
            {
                WriteBytes(p, size);
                VerifyBytes(p, size);
            }
            finally
            {
                Marshal.FreeHGlobal(p);
            }
        }

        [Fact]
        public void ReAllocHGlobal_Invoke_DataCopied()
        {
            const int Size = 3;
            IntPtr p1 = Marshal.AllocHGlobal((IntPtr)Size);
            IntPtr p2 = p1;
            try
            {
                WriteBytes(p1, Size);
                int add = 1;
                do
                {
                    p2 = Marshal.ReAllocHGlobal(p2, (IntPtr)(Size + add));
                    VerifyBytes(p2, Size);
                    add++;
                }
                while (p2 == p1); // stop once we've validated moved case
            }
            finally
            {
                Marshal.FreeHGlobal(p2);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AllocCoTaskMem_Int32_ReadableWritable(int size)
        {
            IntPtr p = Marshal.AllocCoTaskMem(size);
            Assert.NotEqual(IntPtr.Zero, p);
            try
            {
                WriteBytes(p, size);
                VerifyBytes(p, size);
            }
            finally
            {
                Marshal.FreeCoTaskMem(p);
            }
        }

        [Fact]
        public void ReAllocCoTaskMem_Invoke_DataCopied()
        {
            const int Size = 3;
            IntPtr p1 = Marshal.AllocCoTaskMem(Size);
            IntPtr p2 = p1;
            try
            {
                WriteBytes(p1, Size);
                int add = 1;
                do
                {
                    p2 = Marshal.ReAllocCoTaskMem(p2, Size + add);
                    VerifyBytes(p2, Size);
                    add++;
                }
                while (p2 == p1); // stop once we've validated moved case
            }
            finally
            {
                Marshal.FreeCoTaskMem(p2);
            }
        }

        private static void WriteBytes(IntPtr p, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Marshal.WriteByte(p + i, (byte)i);
            }
        }

        private static void VerifyBytes(IntPtr p, int length)
        {
            for (int i = 0; i < length; i++)
            {
                Assert.Equal((byte)i, Marshal.ReadByte(p + i));
            }
        }

        [Fact]
        public void GetExceptionForHR_ThrowExceptionForHR_ThrowsSameException()
        {
            int errorCode = -2146231029;
            COMException getHRException = Marshal.GetExceptionForHR(errorCode) as COMException;
            Assert.Equal(errorCode, getHRException.HResult);
            try
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
            catch (COMException e)
            {
                Assert.Equal(errorCode, e.HResult);
                Assert.Equal(e.HResult, getHRException.HResult);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetExceptionForHR_InvalidHR_ReturnsNull(int errorCode)
        {
            Assert.Null(Marshal.GetExceptionForHR(errorCode));
            Assert.Null(Marshal.GetExceptionForHR(errorCode, IntPtr.Zero));
        }

        [Fact]
        public void GetHRForException_ValidException_ReturnsValidHResult()
        {
            var e = new Exception();

            try
            {
                Assert.InRange(Marshal.GetHRForException(e), int.MinValue, -1);
                Assert.Equal(e.HResult, Marshal.GetHRForException(e));
            }
            finally
            {
                // This GetExceptionForHR call is needed to 'eat' the IErrorInfo put to TLS by
                // Marshal.GetHRForException call above. Otherwise other Marshal.GetExceptionForHR
                // calls would randomly return previous exception objects passed to
                // Marshal.GetHRForException.
                // The correct way is to use Marshal.GetHRForException at interop boundary only, but for the
                // time being we'll keep this code as-is.
                Marshal.GetExceptionForHR(e.HResult);
            }
        }

        [Fact]
        public void GetHRForException_CustomException_ReturnsExpected()
        {
            var exception = new CustomHRException();
            Assert.Equal(10, Marshal.GetHRForException(exception));
        }

        public class CustomHRException : Exception
        {
            public CustomHRException() : base()
            {
                HResult = 10;
            }
        }

        [Fact]
        public void GetHRForException_NullException_ReturnsZero()
        {
            Assert.Equal(0, Marshal.GetHRForException(null));
        }

        public static IEnumerable<object[]> GenerateGuidForType_Valid_TestData()
        {
            yield return new object[] { typeof(int) };
            yield return new object[] { typeof(int).MakePointerType() };
            //yield return new object[] { typeof(int).MakeByRefType() };
            yield return new object[] { typeof(string) };
            yield return new object[] { typeof(string[]) };

            yield return new object[] { typeof(NonGenericClass) };
            yield return new object[] { typeof(GenericClass<string>) };
            yield return new object[] { typeof(AbstractClass) };

            yield return new object[] { typeof(NonGenericStruct) };
            yield return new object[] { typeof(GenericStruct<string>) };

            yield return new object[] { typeof(NonGenericInterface) };
            yield return new object[] { typeof(GenericInterface<string>) };

            yield return new object[] { typeof(GenericClass<>) };
            //yield return new object[] { typeof(GenericClass<>).GetTypeInfo().GenericTypeParameters[0] };

            yield return new object[] { typeof(ClassWithGuidAttribute) };
        }

        [Theory]
        [MemberData(nameof(GenerateGuidForType_Valid_TestData))]
        public void GenerateGuidForType_ValidType_ReturnsExpected(Type type)
        {
            if (type.HasElementType)
            {
                if (PlatformDetection.IsNetCore)
                {
                    Assert.Equal(Guid.Empty, type.GUID);
                    Assert.Equal(type.GUID, Marshal.GenerateGuidForType(type));
                }
                else
                {
                    Assert.NotEqual(type.GUID, Marshal.GenerateGuidForType(type));
                }
            }
            else
            {
                Assert.Equal(type.GUID, Marshal.GenerateGuidForType(type));
            }
        }

        [Fact]
        [ActiveIssue(30926, ~TargetFrameworkMonikers.NetFramework)]
        public void GenerateGuidForType_NullType_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("type", () => Marshal.GenerateGuidForType(null));
        }

        [Fact]
        [ActiveIssue(30926, ~TargetFrameworkMonikers.NetFramework)]
        public void GenerateGuidForType_NotRuntimeType_ThrowsArgumentException()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Type");
            AssertExtensions.Throws<ArgumentException>("type", () => Marshal.GenerateGuidForType(typeBuilder));
        }

        public static IEnumerable<object[]> GenerateProgIdForType_Valid_TestData()
        {
            yield return new object[] { typeof(int), typeof(int).FullName };
            yield return new object[] { typeof(NonGenericClass), typeof(NonGenericClass).FullName };
            yield return new object[] { typeof(AbstractClass), typeof(AbstractClass).FullName };
            yield return new object[] { typeof(NonGenericStruct), typeof(NonGenericStruct).FullName };
            yield return new object[] { typeof(ClassWithProgID), "TestProgID" };
            yield return new object[] { typeof(ClassWithNullProgID), "" };
        }

        [Theory]
        [MemberData(nameof(GenerateProgIdForType_Valid_TestData))]
        public void GenerateProgIdForType_ValidType_ReturnsExpected(Type type, string expected)
        {
            Assert.Equal(expected, Marshal.GenerateProgIdForType(type));
        }

        [ComVisible(true)]
        [ProgId("TestProgID")]
        public class ClassWithProgID
        {
        }

        [ComVisible(true)]
        [ProgId(null)]
        public class ClassWithNullProgID
        {
        }

        [Fact]
        public void GenerateProgIdForType_NullType_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("type", () => Marshal.GenerateProgIdForType(null));
        }

        public static IEnumerable<object[]> GenerateProgIdForType_Invalid_TestData()
        {
            yield return new object[] { typeof(int).MakePointerType() };
            yield return new object[] { typeof(int).MakeByRefType() };
            yield return new object[] { typeof(string[]) };

            yield return new object[] { typeof(GenericClass<string>) };
            yield return new object[] { typeof(GenericStruct<string>) };
            yield return new object[] { typeof(NonGenericInterface) };
            yield return new object[] { typeof(GenericInterface<string>) };

            yield return new object[] { typeof(GenericClass<>) };
            yield return new object[] { typeof(GenericClass<>).GetTypeInfo().GenericTypeParameters[0] };
        }

        [Theory]
        [ActiveIssue(30927, ~TargetFrameworkMonikers.NetFramework)]
        [MemberData(nameof(GenerateProgIdForType_Invalid_TestData))]
        public void GenerateProgIdForType_InvalidType_ThrowsArgumentException(Type type)
        {
            AssertExtensions.Throws<ArgumentException>("type", () => Marshal.GenerateProgIdForType(type));
        }

        [Fact]
        [ActiveIssue(30927, ~TargetFrameworkMonikers.NetFramework)]
        public void GenerateProgIdForType_NotRuntimeType_ThrowsNotSupportedException()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Type");
            Assert.Throws<NotSupportedException>(() => Marshal.GenerateProgIdForType(typeBuilder));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Marshal.GetComObjectData is not implemented in .NET Core.")]
        public void GetComObjectData_NetCore_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetComObjectData(null, null));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetComObjectData is not implemented in .NET Core.")]
        public void GetComObjectData_NullObj_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("obj", () => Marshal.GetComObjectData(null, new object()));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetComObjectData is not implemented in .NET Core.")]
        public void GetComObjectData_NullKey_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("key", () => Marshal.GetComObjectData(new object(), null));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetComObjectData is not implemented in .NET Core.")]
        public void GetComObjectData_NonComObjectObj_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentException>("obj", () => Marshal.GetComObjectData(1, 2));
        }

        [Fact]
        public void GetHINSTANCE_NormalModule_ReturnsSameInstance()
        {
            IntPtr ptr = Marshal.GetHINSTANCE(typeof(int).Module);
            Assert.NotEqual(IntPtr.Zero, ptr);
            Assert.Equal(ptr, Marshal.GetHINSTANCE(typeof(string).Module));
        }

        [Fact]
        public void GetHINSTANCE_ModuleBuilder_ReturnsSameInstance()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");

            IntPtr ptr = Marshal.GetHINSTANCE(moduleBuilder);
            Assert.NotEqual(IntPtr.Zero, ptr);
            Assert.Equal(ptr, Marshal.GetHINSTANCE(moduleBuilder));
        }

        [Fact]
        public void GetHINSTANCE_NullModule_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("m", () => Marshal.GetHINSTANCE(null));
        }

        [Fact]
        [ActiveIssue(30925, TestPlatforms.AnyUnix)]
        public void GetHINSTANCE_NonRuntimeModule_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("m", "Module must be a runtime Module object.", () => Marshal.GetHINSTANCE(new NonRuntimeModule()));
        }

        private class NonRuntimeModule : Module
        {
            public NonRuntimeModule()
            {
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Marshal.GetIDispatchForObject is not implemented in .NET Core.")]
        public void GetIDispatchForObject_Invoke_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetIDispatchForObject(null));
        }

        public static IEnumerable<object[]> GetIDispatchForObject_Vakid_TestData()
        {
            yield return new object[] { new NonGenericClass() };
            yield return new object[] { new NonGenericStruct() };
        }

        [Theory]
        [MemberData(nameof(GetIDispatchForObject_Vakid_TestData))]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetIDispatchForObject is not implemented in .NET Core.")]
        public void GetIDispatchForObject_ValidObject_ReturnsNonZero(object o)
        {
            IntPtr iDispatch = Marshal.GetIDispatchForObject(o);
            try
            {
                Assert.NotEqual(IntPtr.Zero, iDispatch);
            }
            finally
            {
                Marshal.Release(iDispatch);
            }
        }

        public static IEnumerable<object[]> GetIDispatchForObject_Invalid_TestData()
        {
            yield return new object[] { new GenericClass<string>() };
            yield return new object[] { new GenericStruct<string>() };
        }

        [Theory]
        [MemberData(nameof(GetIDispatchForObject_Invalid_TestData))]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetIDispatchForObject is not implemented in .NET Core.")]
        public void GetIDispatchForObject_InvalidObject_ThrowsInvalidCastException(object o)
        {
            Assert.Throws<InvalidCastException>(() => Marshal.GetIDispatchForObject(o));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.GetIDispatchForObject is not implemented in .NET Core.")]
        public void GetIDispatchForObject_NullObject_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("o", () => Marshal.GetIDispatchForObject(null));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetTypedObjectForIUnknown_GenericTypeParameter_ReturnsExpected()
        {
            Type type = typeof(GenericClass<>).GetTypeInfo().GenericTypeParameters[0];
            IntPtr iUnknown = Marshal.GetIUnknownForObject(new object());
            try
            {
                object typedObject = Marshal.GetTypedObjectForIUnknown(iUnknown, type);
                Assert.IsType<object>(typedObject);
            }
            finally
            {
                Marshal.Release(iUnknown);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void GetTypedObjectForIUnknown_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetTypedObjectForIUnknown(IntPtr.Zero, typeof(int)));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetTypedObjectForIUnknown_ZeroUnknown_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("pUnk", () => Marshal.GetTypedObjectForIUnknown(IntPtr.Zero, typeof(int)));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void GetTypedObjectForIUnknown_NullType_ThrowsArgumentNullException()
        {
            IntPtr iUnknown = Marshal.GetIUnknownForObject(new object());
            try
            {
                AssertExtensions.Throws<ArgumentNullException>("t", () => Marshal.GetTypedObjectForIUnknown(iUnknown, null));
            }
            finally
            {
                Marshal.Release(iUnknown);
            }
        }

        public static IEnumerable<object[]> GetTypedObjectForIUnknown_Invalid_TestData()
        {
            yield return new object[] { typeof(GenericClass<string>) };
            yield return new object[] { typeof(GenericStruct<string>) };
            yield return new object[] { typeof(GenericInterface<string>) };

            yield return new object[] { typeof(GenericClass<>) };

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Assembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Type");
            yield return new object[] { typeBuilder };
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [MemberData(nameof(GetTypedObjectForIUnknown_Invalid_TestData))]
        public void GetTypedObjectForIUnknown_InvalidType_ThrowsArgumentException(Type type)
        {
            IntPtr iUnknown = Marshal.GetIUnknownForObject(new object());
            try
            {
                AssertExtensions.Throws<ArgumentException>("t", () => Marshal.GetTypedObjectForIUnknown(iUnknown, type));
            }
            finally
            {
                Marshal.Release(iUnknown);
            }
        }

        [Theory]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(typeof(AbstractClass))]
        [InlineData(typeof(NonGenericClass))]
        [InlineData(typeof(NonGenericStruct))]
        [InlineData(typeof(NonGenericInterface))]
        public void GetTypedObjectForIUnknown_UncastableType_ThrowsInvalidCastException(Type type)
        {
            IntPtr iUnknown = Marshal.GetIUnknownForObject(new object());
            try
            {
                Assert.Throws<InvalidCastException>(() => Marshal.GetTypedObjectForIUnknown(iUnknown, type));
            }
            finally
            {
                Marshal.Release(iUnknown);
            }
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Marshal.SetComObjectData is not implemented in .NET Core.")]
        public void SetComObjectData_NetCore_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.SetComObjectData(null, null, null));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.SetComObjectData is not implemented in .NET Core.")]
        public void SetComObjectData_NullObj_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("obj", () => Marshal.SetComObjectData(null, new object(), 3));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.SetComObjectData is not implemented in .NET Core.")]
        public void SetComObjectData_NullKey_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("key", () => Marshal.SetComObjectData(new object(), null, 3));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework, "Marshal.SetComObjectData is not implemented in .NET Core.")]
        public void SetComObjectData_NonComObjectObj_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentException>("obj", () => Marshal.SetComObjectData(1, 2, 3));
        }

        [Fact]
        public void Prelink_ValidMethod_Success()
        {
            MethodInfo method = typeof(MarshalTests).GetMethod(nameof(Prelink_ValidMethod_Success));
            Marshal.Prelink(method);
            Marshal.Prelink(method);
        }

        [Fact]
        public void Prelink_RuntimeSuppliedMethod_Success()
        {
            MethodInfo method = typeof(Math).GetMethod(nameof(Math.Abs), new Type[] { typeof(double) });
            Marshal.Prelink(method);
            Marshal.Prelink(method);
        }

        [Fact]
        public void Prelink_NullMethod_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("m", () => Marshal.Prelink(null));
        }

        public  static IEnumerable<object[]> PrelinkAll_TestData()
        {
            yield return new object[] { typeof(int) };
            yield return new object[] { typeof(Math) };
            yield return new object[] { typeof(int).MakeByRefType() };
            yield return new object[] { typeof(int).MakePointerType() };
            yield return new object[] { typeof(int[]) };

            yield return new object[] { typeof(NonGenericClass) };
            yield return new object[] { typeof(GenericClass<string>) };
            yield return new object[] { typeof(AbstractClass) };
            yield return new object[] { typeof(NonGenericStruct) };
            yield return new object[] { typeof(GenericStruct<string>) };
            yield return new object[] { typeof(NonGenericInterface) };
            yield return new object[] { typeof(GenericInterface<string>) };

            yield return new object[] { typeof(GenericClass<>) };
            yield return new object[] { typeof(GenericClass<>).GetTypeInfo().GenericTypeParameters[0] };
        }

        [Theory]
        [MemberData(nameof(PrelinkAll_TestData))]
        public void PrelinkAll_ValidType_Success(Type type)
        {
            Marshal.PrelinkAll(type);
            Marshal.PrelinkAll(type);
        }

        [Fact]
        public void PrelinkAll_NullType_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("c", () => Marshal.PrelinkAll(null));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public void StringToCoTaskMemAuto_PtrToStringAuto_ReturnsExpected(int len)
        {
            string s = "Hello World";
            IntPtr ptr = Marshal.StringToCoTaskMemAuto(s);
            try
            {
                string actual = Marshal.PtrToStringAuto(ptr, len);
                Assert.Equal(s.Substring(0, len), actual);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Fact]
        public void PtrToStringAuto_ZeroPtrNoLength_ReturnsNull()
        {
            Assert.Null(Marshal.PtrToStringAuto(IntPtr.Zero));
        }

        [Fact]
        public void PtrToStringAuto_ZeroPtrWithLength_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("ptr", () => Marshal.PtrToStringAuto(IntPtr.Zero, 0));
        }

        [Fact]
        public void PtrToStringAuto_NegativeLength_ThrowsArgumentException()
        {
            string s = "Hello World";
            IntPtr ptr = Marshal.StringToCoTaskMemAuto(s);
            try
            {
                AssertExtensions.Throws<ArgumentException>("len", null, () => Marshal.PtrToStringAuto(ptr, -1));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hello World")]
        public void StringToCoTaskMemAuto_NonNullString_Roundtrips(string s)
        {
            IntPtr ptr = Marshal.StringToCoTaskMemAuto(s);

            try
            {
                // Make sure the native memory is correctly laid out.
                for (int i = 0; i < s.Length; i++)
                {
                    char c = (char)Marshal.ReadInt16(IntPtr.Add(ptr, i << 1));
                    Assert.Equal(s[i], c);
                }

                // Make sure the memory roundtrips.
                Assert.Equal(s, Marshal.PtrToStringAuto(ptr));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Fact]
        public void StringToCoTaskMemAuto_NullString_ReturnsZero()
        {
            Assert.Equal(IntPtr.Zero, Marshal.StringToCoTaskMemAuto(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hello World")]
        public void StringToHGlobalAuto_NonNullString_Roundtrips(string s)
        {
            IntPtr ptr = Marshal.StringToHGlobalAuto(s);

            try
            {
                // Make sure the native memory is correctly laid out.
                for (int i = 0; i < s.Length; i++)
                {
                    char c = (char)Marshal.ReadInt16(IntPtr.Add(ptr, i << 1));
                    Assert.Equal(s[i], c);
                }

                // Make sure the memory roundtrips.
                Assert.Equal(s, Marshal.PtrToStringAuto(ptr));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Fact]
        public void StringToHGlobalAuto_NullString_ReturnsZero()
        {
            Assert.Equal(IntPtr.Zero, Marshal.StringToHGlobalAuto(null));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotNetNative))]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData(null)]
        [InlineData("")]
        public void BindToMoniker_NullOrEmptyMonikerName_ThrowsArgumentException(string monikerName)
        {
            AssertExtensions.Throws<ArgumentException>(null, () => Marshal.BindToMoniker(monikerName));
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.IsNotNetNative))]
        [PlatformSpecific(TestPlatforms.Windows)]
        [InlineData("name")]
        public void BindToMoniker_InvalidMonikerName_ThrowsArgumentException(string monikerName)
        {
            Assert.Throws<COMException>(() => Marshal.BindToMoniker(monikerName));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void BindToMoniker_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.BindToMoniker(null));
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotNetNative))]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void ChangeWrapperHandleStrength_NullObject_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("otp", () => Marshal.ChangeWrapperHandleStrength(null, fIsWeak: true));
            AssertExtensions.Throws<ArgumentNullException>("otp", () => Marshal.ChangeWrapperHandleStrength(null, fIsWeak: false));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void ChangeWrapperHandleStrength_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.ChangeWrapperHandleStrength(null, fIsWeak: true));
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.ChangeWrapperHandleStrength(null, fIsWeak: false));
        }

        public static IEnumerable<object[]> IsComObject_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { "string" };
            yield return new object[] { new int[0] };
            yield return new object[] { new NonGenericClass() };
            yield return new object[] { new GenericClass<int>() };
            yield return new object[] { new NonGenericStruct() };
            yield return new object[] { new GenericStruct<string>() };
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

        [Guid("12345678-0939-11d1-8be1-00c04fd8d503")]
        public class ClassWithGuidAttribute { }

        [ComVisible(true)]
        public class GenericClass<T> { }

        [ComVisible(true)]
        public class NonGenericClass { }

        [ComVisible(true)]
        public class AbstractClass { }

        [ComVisible(true)]
        public struct GenericStruct<T> { }

        [ComVisible(true)]
        public struct NonGenericStruct { }

        [ComVisible(true)]
        public interface GenericInterface<T> { }

        [ComVisible(true)]
        public interface NonGenericInterface { }
    }
}
