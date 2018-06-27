// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class MarshalClassTests
    {
#pragma warning disable 649
        //definition of structure that will be used in testing of structs with Fixed BSTR Safearray fields
        internal struct Variant
        {
            public ushort vt;
            public ushort wReserved1;
            public ushort wReserved2;
            public ushort wReserved3;
            public IntPtr bstrVal;
            public IntPtr pRecInfo;
        }
#pragma warning restore 649

        [StructLayout(LayoutKind.Auto)]
        public struct SomeTestStruct_Auto
        {
            public int i;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StructWithFxdLPSTRSAFld
        {
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.LPStr, SizeConst = 0)]
            public String[] Arr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SomeTestStruct
        {
            public int i;
            //[MarshalAs(UnmanagedType.BStr)]
            public String s;
        }

        public enum TestEnum
        {
            red,
            green,
            blue
        }

        public struct TestStructWithEnumArray
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public TestEnum[] ArrayOfEnum;
        }

        public IntPtr ip;
        public SomeTestStruct someTs = new SomeTestStruct();
        public Exception ex = null;

        [STAThread]
        [Fact]
        public void StructWithEnumArray()
        {
            TestStructWithEnumArray s = new TestStructWithEnumArray();
            s.ArrayOfEnum = new TestEnum[3];
            s.ArrayOfEnum[0] = TestEnum.red;
            s.ArrayOfEnum[1] = TestEnum.green;
            s.ArrayOfEnum[2] = TestEnum.blue;
            int retsize = Marshal.SizeOf(s.GetType());
            Assert.Equal(12, retsize);

            retsize = 0;
            retsize = Marshal.SizeOf(typeof(TestStructWithEnumArray));
            int genericRetsize = Marshal.SizeOf<TestStructWithEnumArray>();
            Assert.True(genericRetsize == retsize, "Generic and non generic versions of the API did not return the same size!");
            Assert.Equal(12, retsize);
        }

        [STAThread]
        [Fact]
        public void StructureToPtr()
        {
            ip = IntPtr.Zero;
            Assert.Throws<ArgumentNullException>("ptr", () => Marshal.StructureToPtr<SomeTestStruct>(someTs, ip, true));

            ip = new IntPtr(123);
            Assert.Throws<ArgumentNullException>("structure", () => Marshal.StructureToPtr<Object>(null, ip, true));

            ip = Marshal.AllocHGlobal(Marshal.SizeOf(someTs));
            someTs.s = "something";
            ex = Record.Exception(() => Marshal.StructureToPtr(someTs, ip, false));
            Assert.Null(ex);
            ex = Record.Exception(() => Marshal.StructureToPtr(someTs, ip, true));
            Assert.Null(ex);
        }

        [STAThread]
        [Fact]
        public void PtrToStructure()
        {
            ip = IntPtr.Zero;
            Assert.Throws<ArgumentNullException>("ptr", () => Marshal.PtrToStructure(ip, someTs));

            ip = new IntPtr(123);
            Assert.Throws<ArgumentNullException>("structureType", () => Marshal.PtrToStructure(ip, null));

            ip = new IntPtr(123);
            Assert.Throws<ArgumentException>(() => Marshal.PtrToStructure<SomeTestStruct>(ip, someTs));
        }

        [STAThread]
        [Fact]
        public void DestroyStructure()
        {
            ip = IntPtr.Zero;
            Assert.Throws<ArgumentNullException>("ptr", () => Marshal.DestroyStructure<SomeTestStruct>(ip));

            ip = new IntPtr(123);
            Assert.Throws<ArgumentNullException>("structureType", () => Marshal.DestroyStructure(ip, null));
        
            SomeTestStruct_Auto someTs_Auto = new SomeTestStruct_Auto();
            Assert.Throws<ArgumentException>(() => Marshal.DestroyStructure(ip, someTs_Auto.GetType()));

            ip = Marshal.AllocHGlobal(Marshal.SizeOf(someTs));
            someTs.s = null;
            ex = Record.Exception(() => Marshal.StructureToPtr(someTs, ip, false));
            Assert.Null(ex);
            ex = Record.Exception(() => Marshal.DestroyStructure<SomeTestStruct>(ip));
            Assert.Null(ex);
        }

        [STAThread]
        [Fact]
        public void SizeOf()
        {
            Assert.Throws<ArgumentNullException>("t", () => Marshal.SizeOf(null));
            Assert.Throws<ArgumentException>(() => Marshal.SizeOf(typeof(StructWithFxdLPSTRSAFld)));
            ex = Record.Exception(() => Marshal.SizeOf(someTs.GetType()));
            Assert.Null(ex);
        }

        [STAThread]
        [Fact]
        public void UnsafeAddrOfPinnedArrayElement()
        {
            Assert.Throws<ArgumentNullException>("arr", () => Marshal.UnsafeAddrOfPinnedArrayElement<Object>(null, 123));
        }

        [STAThread]
        [Fact]
        public void OffsetOf()
        {
            IntPtr nonGenericOffsetCall = Marshal.OffsetOf(typeof(SomeTestStruct), "i");
            IntPtr genericOffsetCall = Marshal.OffsetOf<SomeTestStruct>("i");
            Assert.True(nonGenericOffsetCall == genericOffsetCall, "Generic and non generic versions of the API did not return the same offset");

            Assert.Throws<ArgumentException>(() => Marshal.OffsetOf(typeof(StructWithFxdLPSTRSAFld), "Arr"));
        }

        [STAThread]
        [Fact]
        public void PtrToStringAnsi()
        {
            Assert.Throws<ArgumentNullException>("ptr", () => Marshal.PtrToStringAnsi(IntPtr.Zero, 123));
            Assert.Throws<ArgumentException>(() => Marshal.PtrToStringAnsi(new IntPtr(123), -77));
        }

        [STAThread]
        [Fact]
        public void PtrToStringUni()
        {
            Assert.Throws<ArgumentException>(() => Marshal.PtrToStringUni(new IntPtr(123), -77));
        }

        [STAThread]
        [Fact]
        public void Copy()
        {
            byte[] barr = null;
            Assert.Throws<ArgumentNullException>("source", () => Marshal.Copy(barr, 0, new IntPtr(123), 10));

            byte[] barray = new byte[2];
            Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(barray, 100, new IntPtr(123), 2));
        }
    }
}
