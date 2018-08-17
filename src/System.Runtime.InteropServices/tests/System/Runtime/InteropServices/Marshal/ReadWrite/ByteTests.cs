// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;
using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete

namespace System.Runtime.InteropServices.Tests
{
    public partial class ByteTests
    {
        [Theory]
        [InlineData(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, byte.MaxValue })]
        public void WriteByte_Pointer_Roundtrips(byte[] values)
        {
            int sizeOfArray = Marshal.SizeOf(values[0]) * values.Length;

            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            try
            {
                Marshal.WriteByte(ptr, values[0]);

                for (int i = 1; i < values.Length; i++)
                {
                    Marshal.WriteByte(ptr, i * Marshal.SizeOf(values[0]), values[i]);
                }

                byte value = Marshal.ReadByte(ptr);
                Assert.Equal(values[0], value);

                for (int i = 1; i < values.Length; i++)
                {
                    value = Marshal.ReadByte(ptr, i * Marshal.SizeOf(values[0]));
                    Assert.Equal(values[i], value);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Fact]
        public void WriteByte_BlittableObject_Roundtrips()
        {
            int offset1 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value1)).ToInt32();
            int offset2 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value2)).ToInt32();

            object structure = new BlittableStruct
            {
                value1 = 10,
                value2 = 20
            };

            Marshal.WriteByte(structure, offset1, 11);
            Marshal.WriteByte(structure, offset2, 21);

            Assert.Equal(11, ((BlittableStruct)structure).value1);
            Assert.Equal(21, ((BlittableStruct)structure).value2);
            Assert.Equal(11, Marshal.ReadByte(structure, offset1));
            Assert.Equal(21, Marshal.ReadByte(structure, offset2));
        }

        [Fact]
        public void WriteByte_StructWithReferenceTypes_ReturnsExpected()
        {
            int pointerOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.pointerValue)).ToInt32();
            int stringOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.stringValue)).ToInt32();
            int arrayOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.byValueArray)).ToInt32();

            object structure = new StructWithReferenceTypes
            {
                pointerValue = (IntPtr)100,
                stringValue = "ABC",
                byValueArray = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            };

            Marshal.WriteByte(structure, pointerOffset, 200);
            Marshal.WriteByte(structure, arrayOffset + sizeof(byte) * 9, 100);

            Assert.Equal((IntPtr)200, ((StructWithReferenceTypes)structure).pointerValue);
            Assert.Equal("ABC", ((StructWithReferenceTypes)structure).stringValue);
            Assert.Equal(new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 100 }, ((StructWithReferenceTypes)structure).byValueArray);
            Assert.Equal(200, Marshal.ReadByte(structure, pointerOffset));
            Assert.Equal(100, Marshal.ReadByte(structure, arrayOffset + sizeof(byte) * 9));
        }

        [Fact]
        public void ReadByte_BlittableObject_ReturnsExpected()
        {
            int offset1 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value1)).ToInt32();
            int offset2 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value2)).ToInt32();

            object structure = new BlittableStruct
            {
                value1 = 10,
                value2 = 20
            };

            Assert.Equal(10, Marshal.ReadByte(structure, offset1));
            Assert.Equal(20, Marshal.ReadByte(structure, offset2));
        }

        [Fact]
        public void ReadByte_StructWithReferenceTypes_ReturnsExpected()
        {
            int pointerOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.pointerValue)).ToInt32();
            int stringOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.stringValue)).ToInt32();
            int arrayOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.byValueArray)).ToInt32();

            object structure = new StructWithReferenceTypes
            {
                pointerValue = (IntPtr)100,
                stringValue = "ABC",
                byValueArray = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            };

            Assert.Equal(100, Marshal.ReadByte(structure, pointerOffset));
            Assert.Equal(3, Marshal.ReadByte(structure, arrayOffset + sizeof(byte) * 2));
        }

        [Fact]
        public void ReadByte_ZeroPointer_ThrowsException()
        {
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.ReadByte(IntPtr.Zero));
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.ReadByte(IntPtr.Zero, 2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Exception is wrapped in a TargetInvocationException in the .NET Framework.")]
        public void ReadByte_NullObject_ThrowsAccessViolationException()
        {
            Assert.Throws<AccessViolationException>(() => Marshal.ReadByte(null, 2));
        }

        [Fact]
        public void WriteByte_ZeroPointer_ThrowsException()
        {
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.WriteByte(IntPtr.Zero, 0));
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.WriteByte(IntPtr.Zero, 2, 0));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Exception is wrapped in a TargetInvocationException in the .NET Framework.")]
        public void WriteByte_NullObject_ThrowsAccessViolationException()
        {
            Assert.Throws<AccessViolationException>(() => Marshal.WriteByte(null, 2, 0));
        }

        public struct BlittableStruct
        {
            public byte value1;
            public int padding;
            public byte value2;
        }

        public struct StructWithReferenceTypes
        {
            public IntPtr pointerValue;
            public string stringValue;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] byValueArray;
        }
    }
}

#pragma warning restore CS0618 // Type or member is obsolete
