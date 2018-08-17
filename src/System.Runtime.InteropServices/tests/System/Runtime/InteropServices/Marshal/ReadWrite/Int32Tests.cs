// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Reflection.Emit;
using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete

namespace System.Runtime.InteropServices.Tests
{
    public partial class Int32Tests
    {
        [Theory]
        [InlineData(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, int.MaxValue })]
        public void WriteInt32_Pointer_Roundtrips(int[] values)
        {
            int sizeOfArray = Marshal.SizeOf(values[0]) * values.Length;

            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            try
            {
                Marshal.WriteInt32(ptr, values[0]);

                for (int i = 1; i < values.Length; i++)
                {
                    Marshal.WriteInt32(ptr, i * Marshal.SizeOf(values[0]), values[i]);
                }

                int value = Marshal.ReadInt32(ptr);
                Assert.Equal(values[0], value);

                for (int i = 1; i < values.Length; i++)
                {
                    value = Marshal.ReadInt32(ptr, i * Marshal.SizeOf(values[0]));
                    Assert.Equal(values[i], value);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Fact]
        public void WriteInt32_BlittableObject_Roundtrips()
        {
            int offset1 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value1)).ToInt32();
            int offset2 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value2)).ToInt32();

            object structure = new BlittableStruct
            {
                value1 = 10,
                value2 = 20
            };

            Marshal.WriteInt32(structure, offset1, 11);
            Marshal.WriteInt32(structure, offset2, 21);

            Assert.Equal(11, ((BlittableStruct)structure).value1);
            Assert.Equal(21, ((BlittableStruct)structure).value2);
            Assert.Equal(11, Marshal.ReadInt32(structure, offset1));
            Assert.Equal(21, Marshal.ReadInt32(structure, offset2));
        }

        [Fact]
        public void WriteInt32_StructWithReferenceTypes_ReturnsExpected()
        {
            int pointerOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.pointerValue)).ToInt32();
            int stringOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.stringValue)).ToInt32();
            int arrayOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.byValueArray)).ToInt32();

            object structure = new StructWithReferenceTypes
            {
                pointerValue = (IntPtr)100,
                stringValue = "ABC",
                byValueArray = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            };

            Marshal.WriteInt32(structure, pointerOffset, 200);
            Marshal.WriteInt32(structure, arrayOffset + sizeof(int) * 9, 100);

            Assert.Equal((IntPtr)200, ((StructWithReferenceTypes)structure).pointerValue);
            Assert.Equal("ABC", ((StructWithReferenceTypes)structure).stringValue);
            Assert.Equal(new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 100 }, ((StructWithReferenceTypes)structure).byValueArray);
            Assert.Equal(200, Marshal.ReadInt32(structure, pointerOffset));
            Assert.NotEqual(0, Marshal.ReadInt32(structure, stringOffset));
            Assert.Equal(100, Marshal.ReadInt32(structure, arrayOffset + sizeof(int) * 9));
        }

        [Fact]
        public void ReadInt32_BlittableObject_ReturnsExpected()
        {
            int offset1 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value1)).ToInt32();
            int offset2 = Marshal.OffsetOf<BlittableStruct>(nameof(BlittableStruct.value2)).ToInt32();

            object structure = new BlittableStruct
            {
                value1 = 10,
                value2 = 20
            };

            Assert.Equal(10, Marshal.ReadInt32(structure, offset1));
            Assert.Equal(20, Marshal.ReadInt32(structure, offset2));
        }

        [Fact]
        public void ReadInt32_StructWithReferenceTypes_ReturnsExpected()
        {
            int pointerOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.pointerValue)).ToInt32();
            int stringOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.stringValue)).ToInt32();
            int arrayOffset = Marshal.OffsetOf<StructWithReferenceTypes>(nameof(StructWithReferenceTypes.byValueArray)).ToInt32();

            object structure = new StructWithReferenceTypes
            {
                pointerValue = (IntPtr)100,
                stringValue = "ABC",
                byValueArray = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            };

            Assert.Equal(100, Marshal.ReadInt32(structure, pointerOffset));
            Assert.NotEqual(0, Marshal.ReadInt32(structure, stringOffset));
            Assert.Equal(3, Marshal.ReadInt32(structure, arrayOffset + sizeof(int) * 2));
        }

        [Fact]
        public void ReadInt32_ZeroPoint_ThrowsException()
        {
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.ReadInt32(IntPtr.Zero));
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.ReadInt32(IntPtr.Zero, 2));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Exception is wrapped in a TargetInvocationException in the .NET Framework.")]
        public void ReadInt32_NullObject_ThrowsAccessViolationException()
        {
            Assert.Throws<AccessViolationException>(() => Marshal.ReadInt32(null, 2));
        }

        [Fact]
        public void WriteInt32_ZeroPointer_ThrowsException()
        {
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.WriteInt32(IntPtr.Zero, 0));
            AssertExtensions.ThrowsAny<AccessViolationException, NullReferenceException>(() => Marshal.WriteInt32(IntPtr.Zero, 2, 0));
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework, "Exception is wrapped in a TargetInvocationException in the .NET Framework.")]
        public void WriteInt32_NullObject_ThrowsAccessViolationException()
        {
            Assert.Throws<AccessViolationException>(() => Marshal.WriteInt32(null, 2, 0));
        }

        public struct BlittableStruct
        {
            public int value1;
            public int padding;
            public int value2;
        }

        public struct StructWithReferenceTypes
        {
            public IntPtr pointerValue;
            public string stringValue;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public int[] byValueArray;
        }
    }
}

#pragma warning restore CS0618 // Type or member is obsolete
