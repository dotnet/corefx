// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    internal struct BlittableStruct
    {
        internal int _a;
        internal int _b;
        internal byte _c;
        internal short _d;
        internal IntPtr _p;
    }

    internal struct StructWithReferenceTypes
    {
        internal IntPtr _ptr;
        internal string _str;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        internal int[] _byValArr;
    }
#pragma warning disable 618
    public class ObjectTests
    {
        [Fact]
        public void NullValueArguments_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => { Marshal.WriteByte(null, 0, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.WriteInt16(null, 0, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.WriteInt32(null, 0, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.WriteInt64(null, 0, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.WriteIntPtr(null, 0, IntPtr.Zero); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.ReadByte(null, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.ReadInt16(null, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.ReadInt32(null, 0); });
            Assert.Throws<ArgumentNullException>(() => { Marshal.ReadIntPtr(null, 0); });
        }

        [Fact]
        public void TestBlittableStruct()
        {
            BlittableStruct blittableStruct = new BlittableStruct();
            blittableStruct._a = 200;
            blittableStruct._b = 300;
            blittableStruct._c = 10;
            blittableStruct._d = 123;
            blittableStruct._p = new IntPtr(100);

            object boxedBlittableStruct = (object)blittableStruct;

            int offsetOfB = Marshal.OffsetOf<BlittableStruct>("_b").ToInt32();
            int offsetOfC = Marshal.OffsetOf<BlittableStruct>("_c").ToInt32();
            int offsetOfD = Marshal.OffsetOf<BlittableStruct>("_d").ToInt32();
            int offsetOfP = Marshal.OffsetOf<BlittableStruct>("_p").ToInt32();

            Assert.Equal(Marshal.ReadInt32(boxedBlittableStruct, 0), 200);
            Assert.Equal(Marshal.ReadInt32(boxedBlittableStruct, offsetOfB), 300);
            Assert.Equal(Marshal.ReadByte(boxedBlittableStruct, offsetOfC), 10);
            Assert.Equal(Marshal.ReadInt16(boxedBlittableStruct, offsetOfD), 123);
            Assert.Equal(Marshal.ReadIntPtr(boxedBlittableStruct, offsetOfP), new IntPtr(100));

            Marshal.WriteInt32(boxedBlittableStruct, 0, 300);
            Marshal.WriteInt32(boxedBlittableStruct, offsetOfB, 400);
            Marshal.WriteByte(boxedBlittableStruct, offsetOfC, 20);
            Marshal.WriteInt16(boxedBlittableStruct, offsetOfD, 144);

            Marshal.WriteIntPtr(boxedBlittableStruct, offsetOfP, new IntPtr(500));

            Assert.Equal(((BlittableStruct)boxedBlittableStruct)._a, 300);
            Assert.Equal(((BlittableStruct)boxedBlittableStruct)._b, 400);
            Assert.Equal(((BlittableStruct)boxedBlittableStruct)._c, 20);
            Assert.Equal(((BlittableStruct)boxedBlittableStruct)._d, 144);
            Assert.Equal(((BlittableStruct)boxedBlittableStruct)._p, new IntPtr(500));
        }

        [Fact]
        public void TestStructWithReferenceType()
        {
            StructWithReferenceTypes structWithReferenceTypes = new StructWithReferenceTypes();
            structWithReferenceTypes._ptr = new IntPtr(100);
            structWithReferenceTypes._str = "ABC";
            structWithReferenceTypes._byValArr = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            object boxedStruct = (object)structWithReferenceTypes;

            int offsetOfStr = Marshal.OffsetOf<StructWithReferenceTypes>("_str").ToInt32();
            int offsetOfByValArr = Marshal.OffsetOf<StructWithReferenceTypes>("_byValArr").ToInt32();

            Assert.Equal(Marshal.ReadInt32(boxedStruct, 0), 100);
            Assert.NotEqual(Marshal.ReadIntPtr(boxedStruct, offsetOfStr), IntPtr.Zero);
            Assert.Equal(Marshal.ReadInt32(boxedStruct, offsetOfByValArr + sizeof(int) * 2), 3);

            Marshal.WriteInt32(boxedStruct, 0, 200);
            Marshal.WriteInt32(boxedStruct, offsetOfByValArr + sizeof(int) * 9, 100);

            Assert.Equal(((StructWithReferenceTypes)boxedStruct)._ptr, new IntPtr(200));
            Assert.Equal(((StructWithReferenceTypes)boxedStruct)._byValArr[9], 100);
            Assert.Equal(((StructWithReferenceTypes)boxedStruct)._byValArr[8], 9);
            Assert.Equal(((StructWithReferenceTypes)boxedStruct)._str, "ABC");
        }
    }
#pragma warning restore 618
}
