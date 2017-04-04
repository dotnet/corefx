// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ICustomMarshalerTests
    {
        // To avoid having to create a native test library to reference in tests that
        // interact with native libraries, we can use a simple method from the C standard
        // library. Unfortunately, the C standard library has different names on Windows
        // vs Unix.
#if Windows
        public const string LibcLibrary = "msvcrt.dll";
#else
        public const string LibcLibrary = "libc";
#endif
        [Fact]
        public void Parameter_CustomMarshalerProvidedOnStringType_ForwardsCorrectly()
        {
            Assert.Equal(123, MarshalerOnStringTypeMethod("123"));
        }

        [Fact]
        public void Parameter_CustomMarshalerProvidedOnArrayType_ForwardsCorrectly()
        {
            Assert.Equal(123, MarshalerOnArrayTypeMethod(new string[] { "123" }));
        }

        [Fact]
        public void Parameter_CustomMarshalerProvidedOnBoxedValueType_ForwardsCorrectly()
        {
            Assert.Equal(246, MarshalerOnBoxedValueTypeMethod(123));
        }

        [Fact]
        public void Parameter_CustomMarshalerProvidedOnClassType_ForwardsCorrectly()
        {
            Assert.Equal("246", MarshalerOnClassTypeMethod(new StringContainer { Value = "123" }).Value);
        }

        [Fact]
        public void Parameter_CustomMarshalerProvided_CallsMethodsInCorrectOrdering()
        {
            Assert.Empty(OrderTrackingCustomMarshaler.Events);
            Assert.Equal("123", OrderTrackingMethod("123"));

            string[] expectedOrderingFirstCall = new string[]
            {
                "Called GetInstance",
                "Called MarshalManagedToNative",
                "Called MarshalNativeToManaged",
                "Called CleanUpNativeData"
            };
            Assert.Equal(expectedOrderingFirstCall, OrderTrackingCustomMarshaler.Events);

            // GetInstance is only called once.
            Assert.Equal("234", OrderTrackingMethod("234"));
            IEnumerable<string> expectedOrderingSecondCall = expectedOrderingFirstCall.Concat(new string[]
            {
                "Called MarshalManagedToNative",
                "Called MarshalNativeToManaged",
                "Called CleanUpNativeData"
            });
            Assert.Equal(expectedOrderingSecondCall, OrderTrackingCustomMarshaler.Events);
        }

        [Fact]
        public void Parameter_BothMarshalTypeRefAndMarshalType_PicksMarshalType()
        {
            Assert.Equal(2, BothTypeRefAndTypeMethod("123"));
        }

        [Fact]
        public void Parameter_CookieProvided_PassesCookieToGetInstance()
        {
            Assert.Equal(123, CustomCookieMethod("123"));
            Assert.Equal("Cookie", CookieTrackingCustomMarshaler.Cookie);
        }

        [Fact]
        public void Parameter_NotCustomMarshalerType_UsesSpecifiedMarshaler()
        {
            Assert.Equal(123, NonCustomMarshalerTypeMethod("123"));
        }

        [Fact]
        public void Parameter_MarshalerOnValueType_ThrowsMarshalDirectiveException()
        {
            Assert.Throws<MarshalDirectiveException>(() => MarshalerOnValueTypeMethod(0));
        }

        [Fact]
        public unsafe void Parameter_MarshalerOnPointer_ThrowsMarshalDirectiveException()
        {
            Assert.Throws<MarshalDirectiveException>(() => MarshalerOnPointerMethod(null));
        }

        [Fact]
        public void Parameter_NullICustomMarshaler_ThrowsTypeLoadException()
        {
            Assert.Throws<TypeLoadException>(() => NullCustomMarshalerMethod(""));
        }

        [Fact]
        public void Parameter_NotICustomMarshaler_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => NonICustomMarshalerMethod(""));
        }

        [Fact]
        public void Parameter_OpenGenericICustomMarshaler_ThrowsTypeLoadException()
        {
            Assert.Throws<TypeLoadException>(() => OpenGenericICustomMarshalerMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodDoesntExist_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => NoGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodInstanceMethod_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => InstanceGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodNoParameters_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => NoParametersGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodNonStringParameter_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => NonStringGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodReturnsVoid_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => VoidGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodReturnsNull_ThrowsApplicationException()
        {
            Assert.Throws<ApplicationException>(() => NullGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_GetInstanceMethodThrows_ThrowsActualException()
        {            
            Assert.Throws<NotImplementedException>(() => ThrowingGetInstanceMethod(""));
        }

        [Fact]
        public void Parameter_MarshalManagedToNativeThrows_ThrowsActualException()
        {
            Assert.Throws<NotImplementedException>(() => ThrowingMarshalManagedToNativeMethod(""));
        }

        [Fact]
        public void Parameter_CleanUpNativeDataMethodThrows_ThrowsActualException()
        {
            Assert.Throws<NotImplementedException>(() => ThrowingCleanUpNativeDataMethod(""));
        }

        [Fact]
        public void Field_ParentIsStruct_ThrowsTypeLoadException()
        {
            Assert.Throws<TypeLoadException>(() => StructWithCustomMarshalerFieldMethod(new StructWithCustomMarshalerField()));
        }

        public class StringForwardingCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { Marshal.ZeroFreeCoTaskMemAnsi(pNativeData); }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => Marshal.StringToCoTaskMemAnsi((string)ManagedObj);
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new StringForwardingCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi")]
        public static extern int MarshalerOnStringTypeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringForwardingCustomMarshaler))] string str);

        public class ArrayForwardingCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { Marshal.ZeroFreeCoTaskMemAnsi(pNativeData); }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => Marshal.StringToCoTaskMemAnsi(((string[])ManagedObj)[0]);
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new ArrayForwardingCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi")]
        public static extern int MarshalerOnArrayTypeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.Tests.ICustomMarshalerTests+ArrayForwardingCustomMarshaler")] string[] str);

        public class BoxedValueTypeCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { Marshal.ZeroFreeCoTaskMemAnsi(pNativeData); }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                int unboxedValueType = (int)ManagedObj * 2;
                return Marshal.StringToCoTaskMemAnsi(unboxedValueType.ToString());
            }

            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new BoxedValueTypeCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi")]
        public static extern int MarshalerOnBoxedValueTypeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BoxedValueTypeCustomMarshaler))] object i);

        public class StringContainer
        {
            public string Value { get; set; }
        }

        public class ClassForwardingCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) {}
            public void CleanUpNativeData(IntPtr pNativeData)
            {
                Marshal.ZeroFreeCoTaskMemAnsi(pNativeData);
            }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                return Marshal.StringToCoTaskMemAnsi(((StringContainer)ManagedObj).Value);
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                int doubleValue = pNativeData.ToInt32() * 2;
                return new StringContainer { Value = doubleValue.ToString() };
            }

            public static ICustomMarshaler GetInstance(string cookie) => new ClassForwardingCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ClassForwardingCustomMarshaler))]
        public static extern StringContainer MarshalerOnClassTypeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ClassForwardingCustomMarshaler))] StringContainer str);

        // This should only be used *once*, as it uses static state.
        public class OrderTrackingCustomMarshaler : ICustomMarshaler
        {
            public static List<string> Events { get; } = new List<string>();
            public static IntPtr MarshaledNativeData { get; set; }

            public void CleanUpManagedData(object ManagedObj)
            {
                Events.Add("Called CleanUpManagedData");
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                Assert.Equal(MarshaledNativeData, pNativeData);
                Marshal.ZeroFreeCoTaskMemAnsi(pNativeData);

                Events.Add("Called CleanUpNativeData");
            }

            public int GetNativeDataSize()
            {
                Events.Add("Called GetNativeDataSize");
                return 0;
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                Events.Add("Called MarshalManagedToNative");
                MarshaledNativeData = Marshal.StringToCoTaskMemAnsi((string)ManagedObj);
                return MarshaledNativeData;
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                Events.Add("Called MarshalNativeToManaged");
                return pNativeData.ToInt32().ToString();
            }

            public static ICustomMarshaler GetInstance(string cookie)
            {
                Assert.Empty(cookie);
                Events.Add("Called GetInstance");
                return new OrderTrackingCustomMarshaler();
            }
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(OrderTrackingCustomMarshaler))]
        public static extern string OrderTrackingMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(OrderTrackingCustomMarshaler))] string str);

        public class OverridingCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { Marshal.ZeroFreeCoTaskMemAnsi(pNativeData); }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => Marshal.StringToCoTaskMemAnsi("2");
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new OverridingCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BothTypeRefAndTypeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.Tests.ICustomMarshalerTests+OverridingCustomMarshaler", MarshalTypeRef = typeof(StringForwardingCustomMarshaler))] string str);

        public class CookieTrackingCustomMarshaler : ICustomMarshaler
        {
            public static string Cookie { get; set; }

            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { Marshal.ZeroFreeCoTaskMemAnsi(pNativeData); }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => Marshal.StringToCoTaskMemAnsi((string)ManagedObj);
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie)
            {
                Cookie = cookie;
                return new CookieTrackingCustomMarshaler();
            }
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CustomCookieMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CookieTrackingCustomMarshaler), MarshalCookie = "Cookie")] string str);

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NonCustomMarshalerTypeMethod([MarshalAs(UnmanagedType.LPStr, MarshalTypeRef = typeof(OverridingCustomMarshaler))] string str);

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MarshalerOnValueTypeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringForwardingCustomMarshaler))] int str);

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int MarshalerOnPointerMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringForwardingCustomMarshaler))] int* str);

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NullCustomMarshalerMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = null)] string str);

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NonICustomMarshalerMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(string))] string str);

        public class GenericCustomMarshaler<T> : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new GenericCustomMarshaler<T>();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenGenericICustomMarshalerMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(GenericCustomMarshaler<>))] string str);

        public class NoGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NoGetInstanceCustomMarshaler))] string str);

        public class InstanceGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;
            public ICustomMarshaler GetInstance(string cookie) => new InstanceGetInstanceCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int InstanceGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(InstanceGetInstanceCustomMarshaler))] string str);

        public class NoParameterGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance() => new NoParameterGetInstanceCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoParametersGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NoParameterGetInstanceCustomMarshaler))] string str);

        public class NonStringGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(int x) => new NonStringGetInstanceCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NonStringGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NonStringGetInstanceCustomMarshaler))] string str);

        public class VoidGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static void GetInstance(string cookie) { }
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int VoidGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(VoidGetInstanceCustomMarshaler))] string str);

        public class NullGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => null;
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NullGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(NullGetInstanceCustomMarshaler))] string str);

        public class ThrowingGetInstanceCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => IntPtr.Zero;
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => throw new NotImplementedException();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ThrowingGetInstanceMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ThrowingGetInstanceCustomMarshaler))] string str);

        public class ThrowingMarshalManagedToNativeCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) { }

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => throw new NotImplementedException();
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new ThrowingMarshalManagedToNativeCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ThrowingMarshalManagedToNativeMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ThrowingMarshalManagedToNativeCustomMarshaler))] string str);

        public class ThrowingCleanUpNativeDataCustomMarshaler : ICustomMarshaler
        {
            public void CleanUpManagedData(object ManagedObj) { }
            public void CleanUpNativeData(IntPtr pNativeData) => throw new NotImplementedException();

            public int GetNativeDataSize() => IntPtr.Size;

            public IntPtr MarshalManagedToNative(object ManagedObj) => Marshal.StringToCoTaskMemAnsi((string)ManagedObj);
            public object MarshalNativeToManaged(IntPtr pNativeData) => null;

            public static ICustomMarshaler GetInstance(string cookie) => new ThrowingMarshalManagedToNativeCustomMarshaler();
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ThrowingCleanUpNativeDataMethod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ThrowingCleanUpNativeDataCustomMarshaler))] string str);

        public struct StructWithCustomMarshalerField
        {
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringForwardingCustomMarshaler))]
            public string Field;
        }

        [DllImport(LibcLibrary, EntryPoint = "atoi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int StructWithCustomMarshalerFieldMethod(StructWithCustomMarshalerField c);
    }
}
