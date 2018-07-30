// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    [SkipOnTargetFramework(TargetFrameworkMonikers.Uap, "Marshalling between VARIANT and Object is not supported in AppX")]
    public class GetObjectForNativeVariantTests
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Record
        {
            private IntPtr _record;
            private IntPtr _recordInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct UnionTypes
        {
            [FieldOffset(0)] internal SByte _i1;
            [FieldOffset(0)] internal Int16 _i2;
            [FieldOffset(0)] internal Int32 _i4;
            [FieldOffset(0)] internal Int64 _i8;
            [FieldOffset(0)] internal Byte _ui1;
            [FieldOffset(0)] internal UInt16 _ui2;
            [FieldOffset(0)] internal UInt32 _ui4;
            [FieldOffset(0)] internal UInt64 _ui8;
            [FieldOffset(0)] internal Int32 _int;
            [FieldOffset(0)] internal UInt32 _uint;
            [FieldOffset(0)] internal Single _r4;
            [FieldOffset(0)] internal Double _r8;
            [FieldOffset(0)] internal Int64 _cy;
            [FieldOffset(0)] internal double _date;
            [FieldOffset(0)] internal IntPtr _bstr;
            [FieldOffset(0)] internal IntPtr _unknown;
            [FieldOffset(0)] internal IntPtr _dispatch;
            [FieldOffset(0)] internal IntPtr _pvarVal;
            [FieldOffset(0)] internal IntPtr _byref;
            [FieldOffset(0)] internal Record _record;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TypeUnion
        {
            public ushort vt;
            public ushort wReserved1;
            public ushort wReserved2;
            public ushort wReserved3;
            public UnionTypes _unionTypes;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Variant
        {
            [FieldOffset(0)] public TypeUnion m_Variant;
            [FieldOffset(0)] public decimal m_decimal;
        }

#pragma warning disable 618

        [DllImport(@"oleaut32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern Int32 VariantClear(IntPtr pvarg);

        private static void DeleteVariant(IntPtr pVariant)
        {
            VariantClear(pVariant);
            Marshal.FreeHGlobal(pVariant);
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void DecimalType()
        {
            Variant v = new Variant();
            IntPtr pNative = Marshal.AllocHGlobal(Marshal.SizeOf(v));
            try
            {
                Marshal.GetNativeVariantForObject(3.14m, pNative);
                decimal d = Marshal.GetObjectForNativeVariant<decimal>(pNative);
                Assert.Equal(3.14m, d);
            }
            finally
            {
                DeleteVariant(pNative);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void PrimitiveType()
        {
            Variant v = new Variant();
            IntPtr pNative = Marshal.AllocHGlobal(Marshal.SizeOf(v));
            try
            {
                Marshal.GetNativeVariantForObject<ushort>(99, pNative);
                ushort actual = Marshal.GetObjectForNativeVariant<ushort>(pNative);
                Assert.Equal(99, actual);
            }
            finally
            {
                DeleteVariant(pNative);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void StringType()
        {
            Variant v = new Variant();
            IntPtr pNative = Marshal.AllocHGlobal(Marshal.SizeOf(v));
            try
            {
                Marshal.GetNativeVariantForObject<string>("99", pNative);
                string actual = Marshal.GetObjectForNativeVariant<string>(pNative);
                Assert.Equal("99", actual);
            }
            finally
            {
                DeleteVariant(pNative);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void DoubleType()
        {
            Variant v = new Variant();
            IntPtr pNative = Marshal.AllocHGlobal(Marshal.SizeOf(v));
            try
            {
                Marshal.GetNativeVariantForObject<double>(3.14, pNative);
                double actual = Marshal.GetObjectForNativeVariant<double>(pNative);
                Assert.Equal(3.14, actual);
            }
            finally
            {
                DeleteVariant(pNative);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void IUnknownType()
        {
            Variant v = new Variant();
            object obj = new object();

            // Technically we don't need this - but this is necessary for forcing
            // COM initialization
            IntPtr pUnk = Marshal.GetIUnknownForObject(obj);
            IntPtr pNative = Marshal.AllocHGlobal(Marshal.SizeOf(v));
            try
            {
                Marshal.GetNativeVariantForObject<object>(obj, pNative);
                object actual = Marshal.GetObjectForNativeVariant<object>(pNative);
                Assert.Equal(obj, actual);
            }
            finally
            {
                Marshal.Release(pUnk);
                DeleteVariant(pNative);
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void GetObjectForNativeVariant_Unix_ThrowsPlatformNotSupportedException()
        {
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetObjectForNativeVariant(IntPtr.Zero));
            Assert.Throws<PlatformNotSupportedException>(() => Marshal.GetObjectForNativeVariant<int>(IntPtr.Zero));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public static void GetObjectForNativeVariant_ZeroPointer_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("pSrcNativeVariant", () => Marshal.GetObjectForNativeVariant(IntPtr.Zero));
            AssertExtensions.Throws<ArgumentNullException>("pSrcNativeVariant", () => Marshal.GetObjectForNativeVariant<int>(IntPtr.Zero));
        }
#pragma warning restore 618
    }
}
