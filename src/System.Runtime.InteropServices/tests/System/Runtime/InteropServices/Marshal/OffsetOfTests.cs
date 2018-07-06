// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class OffsetOfTests
    {
        [Fact]
        public static void NullParameter()
        {
            Assert.Throws<ArgumentNullException>(() => Marshal.OffsetOf(null, null));
            Assert.Throws<ArgumentNullException>(() => Marshal.OffsetOf(new object().GetType(), null));
            Assert.Throws<ArgumentNullException>(() => Marshal.OffsetOf(null, "abcd"));
        }

        [Fact]
        public void NonExistField()
        {
            Assert.Throws<ArgumentException>(() => Marshal.OffsetOf(typeof(NonExistField), "NonExistField"));
        }

        [Fact]
        public void NoLayoutClass()
        {
            Assert.Throws<ArgumentException>(() => Marshal.OffsetOf(typeof(NoLayoutPoint), "x"));
        }

        [Fact]
        public void StructField()
        {
            Assert.Equal(new IntPtr(4), Marshal.OffsetOf(typeof(SomeStruct), "var"));
        }

        [Fact]
        public void ClassExplicitField()
        {
            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(typeof(MySystemTime), "wYear"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(typeof(MySystemTime), "wHour"));
            Assert.Equal(new IntPtr(14), Marshal.OffsetOf(typeof(MySystemTime), "wMilliseconds"));
        }

        [Fact]
        public void ClassSequentialField()
        {
            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(typeof(MyPoint), "x"));
            Assert.Equal(new IntPtr(4), Marshal.OffsetOf(typeof(MyPoint), "y"));
        }

        [Fact]
        public void TestExplicitLayout()
        {
            Type t = typeof(ExplicitLayoutTest);
            Assert.Equal(56, Marshal.SizeOf(t));
            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(t, "m_short1"));
            Assert.Equal(new IntPtr(2), Marshal.OffsetOf(t, "m_short2"));

            Assert.Equal(new IntPtr(4), Marshal.OffsetOf(t, "union1_byte1"));
            Assert.Equal(new IntPtr(5), Marshal.OffsetOf(t, "union1_byte2"));
            Assert.Equal(new IntPtr(6), Marshal.OffsetOf(t, "union1_short1"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(t, "union1_int1"));
            Assert.Equal(new IntPtr(12), Marshal.OffsetOf(t, "union1_int2"));
            Assert.Equal(new IntPtr(16), Marshal.OffsetOf(t, "union1_double1"));

            Assert.Equal(new IntPtr(4), Marshal.OffsetOf(t, "union2_ushort1"));
            Assert.Equal(new IntPtr(6), Marshal.OffsetOf(t, "union2_ushort2"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(t, "union3_int1"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(t, "union3_decimal1"));

            Assert.Equal(new IntPtr(24), Marshal.OffsetOf(t, "m_ushort1"));
            Assert.Equal(new IntPtr(32), Marshal.OffsetOf(t, "m_decimal1"));
            Assert.Equal(new IntPtr(48), Marshal.OffsetOf(t, "m_char1"));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void TestFieldAlignment_Windows()
        {
            Type t = typeof(FieldAlignmentTest);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || (RuntimeInformation.ProcessArchitecture != Architecture.X86))
            {
                Assert.Equal(80, Marshal.SizeOf(t));
            }
            else
            {
                Assert.Equal(72, Marshal.SizeOf(t));
            }

            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(t, "m_byte1"));
            Assert.Equal(new IntPtr(2), Marshal.OffsetOf(t, "m_short1"));
            Assert.Equal(new IntPtr(4), Marshal.OffsetOf(t, "m_short2"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(t, "m_int1"));
            Assert.Equal(new IntPtr(12), Marshal.OffsetOf(t, "m_byte2"));
            Assert.Equal(new IntPtr(16), Marshal.OffsetOf(t, "m_int2"));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || (RuntimeInformation.ProcessArchitecture != Architecture.X86))
            {
                Assert.Equal(new IntPtr(24), Marshal.OffsetOf(t, "m_double1"));
                Assert.Equal(new IntPtr(32), Marshal.OffsetOf(t, "m_char1"));
                Assert.Equal(new IntPtr(33), Marshal.OffsetOf(t, "m_char2"));
                Assert.Equal(new IntPtr(34), Marshal.OffsetOf(t, "m_char3"));
                Assert.Equal(new IntPtr(40), Marshal.OffsetOf(t, "m_double2"));
                Assert.Equal(new IntPtr(48), Marshal.OffsetOf(t, "m_byte3"));
                Assert.Equal(new IntPtr(49), Marshal.OffsetOf(t, "m_byte4"));
                Assert.Equal(new IntPtr(56), Marshal.OffsetOf(t, "m_decimal1"));
                Assert.Equal(new IntPtr(72), Marshal.OffsetOf(t, "m_char4"));
            }
            else
            {
                Assert.Equal(new IntPtr(20), Marshal.OffsetOf(t, "m_double1"));
                Assert.Equal(new IntPtr(28), Marshal.OffsetOf(t, "m_char1"));
                Assert.Equal(new IntPtr(29), Marshal.OffsetOf(t, "m_char2"));
                Assert.Equal(new IntPtr(30), Marshal.OffsetOf(t, "m_char3"));
                Assert.Equal(new IntPtr(32), Marshal.OffsetOf(t, "m_double2"));
                Assert.Equal(new IntPtr(40), Marshal.OffsetOf(t, "m_byte3"));
                Assert.Equal(new IntPtr(41), Marshal.OffsetOf(t, "m_byte4"));
                Assert.Equal(new IntPtr(48), Marshal.OffsetOf(t, "m_decimal1"));
                Assert.Equal(new IntPtr(64), Marshal.OffsetOf(t, "m_char4"));
            }
        }

        [Fact]
        public void TestFieldAlignment_Decimal()
        {
            Type t = typeof(FieldAlignmentTest_Decimal);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || (RuntimeInformation.ProcessArchitecture != Architecture.X86))
            {
                Assert.Equal(96, Marshal.SizeOf(t));
            }
            else
            {
                Assert.Equal(88, Marshal.SizeOf(t));
            }

            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(t, "b"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(t, "p"));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || (RuntimeInformation.ProcessArchitecture != Architecture.X86))
            {
                Assert.Equal(new IntPtr(88), Marshal.OffsetOf(t, "s"));
            }
            else
            {
                Assert.Equal(new IntPtr(80), Marshal.OffsetOf(t, "s"));
            }
        }

        [Fact]
        public void TestFieldAlignment_Guid()
        {
            Type t = typeof(FieldAlignmentTest_Guid);
            Assert.Equal(24, Marshal.SizeOf(t));

            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(t, "b"));
            Assert.Equal(new IntPtr(4), Marshal.OffsetOf(t, "g"));
            Assert.Equal(new IntPtr(20), Marshal.OffsetOf(t, "s"));
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.Windows)]
        public void TestFieldAlignment_Variant()
        {
            Type t = typeof(FieldAlignmentTest_Variant);

            Assert.Equal(new IntPtr(0), Marshal.OffsetOf(t, "b"));
            Assert.Equal(new IntPtr(8), Marshal.OffsetOf(t, "v"));

            if (IntPtr.Size == 4)
            {
                Assert.Equal(new IntPtr(24), Marshal.OffsetOf(t, "s"));
                Assert.Equal(32, Marshal.SizeOf(t));
            }
            else if (IntPtr.Size == 8)
            {
                Assert.Equal(new IntPtr(32), Marshal.OffsetOf(t, "s"));
                Assert.Equal(40, Marshal.SizeOf(t));
            }
            else
            {
                Assert.True(false, string.Format("Unexpected value '{0}' for IntPtr.Size", IntPtr.Size));
            }
        }

        [Fact]
        public void NonGenericOffEqualsGenericOffset()
        {
            IntPtr nonGenericOffsetCall = Marshal.OffsetOf(typeof(SomeStruct), "var");
            IntPtr genericOffsetCall = Marshal.OffsetOf<SomeStruct>("var");
            Assert.Equal(nonGenericOffsetCall, genericOffsetCall);

            Assert.Throws<ArgumentException>(() => Marshal.OffsetOf(typeof(StructWithFxdLPSTRSAFld), "Arr"));
        }
    }
#pragma warning disable 169, 649, 618
    [StructLayout(LayoutKind.Sequential)]
    public struct StructWithFxdLPSTRSAFld
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.LPStr, SizeConst = 0)]
        public String[] Arr;
    }

    public struct SomeStruct
    {
        public bool p;
        public int var;
    }

    [StructLayout(LayoutKind.Explicit)]
    public class MySystemTime
    {
        [FieldOffset(0)]
        public ushort wYear;
        [FieldOffset(2)]
        public ushort wMonth;
        [FieldOffset(4)]
        public ushort wDayOfWeek;
        [FieldOffset(6)]
        public ushort wDay;
        [FieldOffset(8)]
        public ushort wHour;
        [FieldOffset(10)]
        public ushort wMinute;
        [FieldOffset(12)]
        public ushort wSecond;
        [FieldOffset(14)]
        public ushort wMilliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MyPoint
    {
        public int x;
        public int y;
    }

    public class NoLayoutPoint
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NonExistField
    {

    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct ExplicitLayoutTest
    {
        [FieldOffset(0)]
        public short m_short1; // 2 bytes
        [FieldOffset(2)]
        public short m_short2; // 2 bytes

        [FieldOffset(4)]
        public byte union1_byte1; // 1 byte
        [FieldOffset(5)]
        public byte union1_byte2; // 1 byte
        [FieldOffset(6)]
        public short union1_short1; // 2 bytes
        [FieldOffset(8)]
        public Int32 union1_int1; // 4 bytes
        [FieldOffset(12)]
        public Int32 union1_int2; // 4 bytes
        [FieldOffset(16)]
        public double union1_double1; // 8 bytes

        [FieldOffset(4)]
        public ushort union2_ushort1; // 2 bytes
        [FieldOffset(6)]
        public ushort union2_ushort2; // 2 bytes
        [FieldOffset(8)]
        public Int32 union3_int1; // 4 bytes
        [FieldOffset(8)]
        public decimal union3_decimal1; // 16 bytes

        [FieldOffset(24)]
        public ushort m_ushort1; // 2 bytes
                                 // 6 bytes of padding

        [FieldOffset(32)]
        public decimal m_decimal1; // 16 bytes

        [FieldOffset(48)]
        public char m_char1; // 1 byte
                             // 7 bytes of padding
    }

    internal struct FieldAlignmentTest
    {
        public byte m_byte1; // 1 byte
                             // 1 byte of padding

        public short m_short1; // 2 bytes
        public short m_short2; // 2 bytes
                               // 2 bytes of padding

        public Int32 m_int1; // 4 bytes
        public byte m_byte2; // 1 byte
                             // 3 bytes of padding

        public Int32 m_int2; // 4 bytes
                             // 4 bytes of padding (0 bytes on x86/Unix according System V ABI as double 4-byte aligned)

        public double m_double1; // 8 bytes
        public char m_char1; // 1 byte
        public char m_char2; // 1 byte
        public char m_char3; // 1 byte
                             // 5 bytes of padding (1 byte on x86/Unix according System V ABI as double 4-byte aligned)

        public double m_double2; // 8 bytes
        public byte m_byte3; // 1 byte
        public byte m_byte4; // 1 byte
                             // 6 bytes of padding

        public decimal m_decimal1; // 16 bytes
        public char m_char4; // 1 byte
                             // 7 bytes of padding
    }
    struct FieldAlignmentTest_Decimal
    {
        public byte b; // 1 byte
                       // 7 bytes of padding

        // The largest field in below struct is decimal (16 bytes wide).
        // However, alignment requirement for the below struct should be only  8 bytes (not 16).
        // This is because unlike fields of other types well known to mcg (like long, char etc.)
        // which need to be aligned according to their byte size, decimal is really a struct
        // with 8 byte alignment requirement.
        public FieldAlignmentTest p; // 80 bytes (72 bytes on x86/Unix)

        public short s; // 2 bytes
                        // 6 bytes of padding
    }

    struct FieldAlignmentTest_Guid
    {
        public byte b; // 1 byte
                       // 3 bytes of padding

        // Guid is really a struct with 4 byte alignment requirement (which is less than its byte size of 16 bytes).
        public Guid g; // 16 bytes

        public short s; // 2 bytes
                        // 2 bytes of padding
    }

    struct FieldAlignmentTest_Variant
    {
        public byte b; // 1 byte
                       // 7 bytes of padding

        // Using [MarshalAs(UnmanagedType.Struct)] means that the Variant type will be used for field 'v' on native side.
        // Variant is really a struct with 8 byte alignment requirement (which is less than its byte size of 24 / 16 bytes).
        [MarshalAs(UnmanagedType.Struct)]
        public object v; // 16 bytes on 32-bit, 24 bytes on 64-bit

        public short s; // 2 bytes
                        // 6 bytes of padding
    };
#pragma warning restore 169, 649, 618
}
