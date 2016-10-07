// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace System.Runtime.CompilerServices
{
    public class UnsafeTests
    {
        [Fact]
        public static unsafe void ReadInt32()
        {
            int expected = 10;
            void* address = Unsafe.AsPointer(ref expected);
            int ret = Unsafe.Read<int>(address);
            Assert.Equal(expected, ret);
        }

        [Fact]
        public static unsafe void WriteInt32()
        {
            int value = 10;
            int* address = (int*)Unsafe.AsPointer(ref value);
            int expected = 20;
            Unsafe.Write(address, expected);

            Assert.Equal(expected, value);
            Assert.Equal(expected, *address);
            Assert.Equal(expected, Unsafe.Read<int>(address));
        }

        [Fact]
        public static unsafe void WriteBytesIntoInt32()
        {
            int value = 20;
            int* intAddress = (int*)Unsafe.AsPointer(ref value);
            byte* byteAddress = (byte*)intAddress;
            for (int i = 0; i < 4; i++)
            {
                Unsafe.Write(byteAddress + i, (byte)i);
            }

            Assert.Equal(0, Unsafe.Read<byte>(byteAddress));
            Assert.Equal(1, Unsafe.Read<byte>(byteAddress + 1));
            Assert.Equal(2, Unsafe.Read<byte>(byteAddress + 2));
            Assert.Equal(3, Unsafe.Read<byte>(byteAddress + 3));

            Byte4 b4 = Unsafe.Read<Byte4>(byteAddress);
            Assert.Equal(0, b4.B0);
            Assert.Equal(1, b4.B1);
            Assert.Equal(2, b4.B2);
            Assert.Equal(3, b4.B3);

            int expected = (b4.B3 << 24) + (b4.B2 << 16) + (b4.B1 << 8) + (b4.B0);
            Assert.Equal(expected, value);
        }

        [Fact]
        public static unsafe void LongIntoCompoundStruct()
        {
            long value = 1234567891011121314L;
            long* longAddress = (long*)Unsafe.AsPointer(ref value);
            Byte4Short2 b4s2 = Unsafe.Read<Byte4Short2>(longAddress);
            Assert.Equal(162, b4s2.B0);
            Assert.Equal(48, b4s2.B1);
            Assert.Equal(210, b4s2.B2);
            Assert.Equal(178, b4s2.B3);
            Assert.Equal(4340, b4s2.S4);
            Assert.Equal(4386, b4s2.S6);

            b4s2.B0 = 1;
            b4s2.B1 = 1;
            b4s2.B2 = 1;
            b4s2.B3 = 1;
            b4s2.S4 = 1;
            b4s2.S6 = 1;
            Unsafe.Write(longAddress, b4s2);

            long expected = 281479288520961;
            Assert.Equal(expected, value);
            Assert.Equal(expected, Unsafe.Read<long>(longAddress));
        }

        [Fact]
        public static unsafe void ReadWriteDoublePointer()
        {
            int value1 = 10;
            int value2 = 20;
            int* valueAddress = (int*)Unsafe.AsPointer(ref value1);
            int** valueAddressPtr = &valueAddress;
            Unsafe.Write(valueAddressPtr, new IntPtr(&value2));

            Assert.Equal(20, *(*valueAddressPtr));
            Assert.Equal(20, Unsafe.Read<int>(valueAddress));
            Assert.Equal(new IntPtr(valueAddress), Unsafe.Read<IntPtr>(valueAddressPtr));
            Assert.Equal(20, Unsafe.Read<int>(Unsafe.Read<IntPtr>(valueAddressPtr).ToPointer()));
        }

        [Fact]
        public static unsafe void CopyToRef()
        {
            int value = 10;
            int destination = -1;
            Unsafe.Copy(ref destination, Unsafe.AsPointer(ref value));
            Assert.Equal(10, destination);
            Assert.Equal(10, value);

            int destination2 = -1;
            Unsafe.Copy(ref destination2, &value);
            Assert.Equal(10, destination2);
            Assert.Equal(10, value);
        }

        [Fact]
        public static unsafe void CopyToVoidPtr()
        {
            int value = 10;
            int destination = -1;
            Unsafe.Copy(Unsafe.AsPointer(ref destination), ref value);
            Assert.Equal(10, destination);
            Assert.Equal(10, value);

            int destination2 = -1;
            Unsafe.Copy(&destination2, ref value);
            Assert.Equal(10, destination2);
            Assert.Equal(10, value);
        }

        [Theory]
        [MemberData(nameof(SizeOfData))]
        public static unsafe void SizeOf<T>(int expected, T valueUnused)
        {
            // valueUnused is only present to enable Xunit to call the correct generic overload.
            Assert.Equal(expected, Unsafe.SizeOf<T>());
        }

        public static IEnumerable<object[]> SizeOfData()
        {
            yield return new object[] { 1, new sbyte() };
            yield return new object[] { 1, new byte() };
            yield return new object[] { 2, new short() };
            yield return new object[] { 2, new ushort() };
            yield return new object[] { 4, new int() };
            yield return new object[] { 4, new uint() };
            yield return new object[] { 8, new long() };
            yield return new object[] { 8, new ulong() };
            yield return new object[] { 4, new float() };
            yield return new object[] { 8, new double() };
            yield return new object[] { 4, new Byte4() };
            yield return new object[] { 8, new Byte4Short2() };
            yield return new object[] { 512, new Byte512() };
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockStack(int numBytes, byte value)
        {
            byte* stackPtr = stackalloc byte[numBytes];
            Unsafe.InitBlock(stackPtr, value, (uint)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(stackPtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUnmanaged(int numBytes, byte value)
        {
            IntPtr allocatedMemory = Marshal.AllocCoTaskMem(numBytes);
            byte* bytePtr = (byte*)allocatedMemory.ToPointer();
            Unsafe.InitBlock(bytePtr, value, (uint)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(bytePtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUIntPtrStack(int numBytes, byte value)
        {
            byte* stackPtr = stackalloc byte[numBytes];
            Unsafe.InitBlock(stackPtr, value, (UIntPtr)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(stackPtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUIntPtrUnmanaged(int numBytes, byte value)
        {
            IntPtr allocatedMemory = Marshal.AllocCoTaskMem(numBytes);
            byte* bytePtr = (byte*)allocatedMemory.ToPointer();
            Unsafe.InitBlock(bytePtr, value, (UIntPtr)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(bytePtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUnalignedStack(int numBytes, byte value)
        {
            byte* stackPtr = stackalloc byte[numBytes + 1];
            stackPtr += 1; // +1 = make unaligned
            Unsafe.InitBlockUnaligned(stackPtr, value, (uint)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(stackPtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUnalignedUnmanaged(int numBytes, byte value)
        {
            IntPtr allocatedMemory = Marshal.AllocCoTaskMem(numBytes + 1);
            byte* bytePtr = (byte*)allocatedMemory.ToPointer() + 1; // +1 = make unaligned
            Unsafe.InitBlockUnaligned(bytePtr, value, (uint)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(bytePtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUnalignedUIntPtrStack(int numBytes, byte value)
        {
            byte* stackPtr = stackalloc byte[numBytes + 1];
            stackPtr += 1; // +1 = make unaligned
            Unsafe.InitBlockUnaligned(stackPtr, value, (UIntPtr)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(stackPtr[i], value);
            }
        }

        [Theory]
        [MemberData(nameof(InitBlockData))]
        public static unsafe void InitBlockUnalignedUIntPtrUnmanaged(int numBytes, byte value)
        {
            IntPtr allocatedMemory = Marshal.AllocCoTaskMem(numBytes + 1);
            byte* bytePtr = (byte*)allocatedMemory.ToPointer() + 1; // +1 = make unaligned
            Unsafe.InitBlockUnaligned(bytePtr, value, (UIntPtr)numBytes);
            for (int i = 0; i < numBytes; i++)
            {
                Assert.Equal(bytePtr[i], value);
            }
        }

        public static IEnumerable<object[]> InitBlockData()
        {
            yield return new object[] { 0, 1 };
            yield return new object[] { 1, 1 };
            yield return new object[] { 10, 0 };
            yield return new object[] { 10, 2 };
            yield return new object[] { 10, 255 };
            yield return new object[] { 10000, 255 };
        }

        [Theory]
        [MemberData(nameof(CopyBlockData))]
        public static unsafe void CopyBlock(int numBytes)
        {
            byte* source = stackalloc byte[numBytes];
            byte* destination = stackalloc byte[numBytes];

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                source[i] = value;
            }

            Unsafe.CopyBlock(destination, source, (uint)numBytes);

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                Assert.Equal(value, destination[i]);
                Assert.Equal(source[i], destination[i]);
            }
        }

        [Theory]
        [MemberData(nameof(CopyBlockData))]
        public static unsafe void CopyBlockUIntPtr(int numBytes)
        {
            byte* source = stackalloc byte[numBytes];
            byte* destination = stackalloc byte[numBytes];

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                source[i] = value;
            }

            Unsafe.CopyBlock(destination, source, (UIntPtr)numBytes);

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                Assert.Equal(value, destination[i]);
                Assert.Equal(source[i], destination[i]);
            }
        }

        [Theory]
        [MemberData(nameof(CopyBlockData))]
        public static unsafe void CopyBlockUnaligned(int numBytes)
        {
            byte* source = stackalloc byte[numBytes + 1];
            byte* destination = stackalloc byte[numBytes + 1];
            source += 1;      // +1 = make unaligned
            destination += 1; // +1 = make unaligned

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                source[i] = value;
            }

            Unsafe.CopyBlockUnaligned(destination, source, (uint)numBytes);

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                Assert.Equal(value, destination[i]);
                Assert.Equal(source[i], destination[i]);
            }
        }


        [Theory]
        [MemberData(nameof(CopyBlockData))]
        public static unsafe void CopyBlockUnalignedUIntPtr(int numBytes)
        {
            byte* source = stackalloc byte[numBytes + 1];
            byte* destination = stackalloc byte[numBytes + 1];
            source += 1;      // +1 = make unaligned
            destination += 1; // +1 = make unaligned

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                source[i] = value;
            }

            Unsafe.CopyBlockUnaligned(destination, source, (UIntPtr)numBytes);

            for (int i = 0; i < numBytes; i++)
            {
                byte value = (byte)(i % 255);
                Assert.Equal(value, destination[i]);
                Assert.Equal(source[i], destination[i]);
            }
        }

        public static IEnumerable<object[]> CopyBlockData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 1 };
            yield return new object[] { 10 };
            yield return new object[] { 100 };
            yield return new object[] { 100000 };
        }

        [Fact]
        public static void As()
        {
            object o = "Hello";
            Assert.Equal("Hello", Unsafe.As<string>(o));
        }

        [Fact]
        public static void DangerousAs()
        {
            // Verify that As does not perform type checks
            object o = new Object();
            Assert.IsType(typeof(Object), Unsafe.As<string>(o));
        }

        // Active Issue: https://github.com/dotnet/coreclr/issues/6505
        // These tests require C# compiler with support for ref returns and locals
#if false
        [Fact]
        public unsafe static void AsRef()
        {
            byte[] b = new byte[4] { 0x42, 0x42, 0x42, 0x42 };
            fixed (byte * p = b)
            {
                ref int r = ref Unsafe.AsRef<int>(p);
                Assert.Equal(0x42424242, r);

                r = 0x0EF00EF0;
                Assert.Equal(0xFE, b[0] | b[1] | b[2] | b[3]);
            }
        }

        [Fact]
        public static void RefAs()
        {
            byte[] b = new byte[4] { 0x42, 0x42, 0x42, 0x42 };

            ref int r = ref Unsafe.As<byte, int>(ref b[0]);
            Assert.Equal(0x42424242, r);

            r = 0x0EF00EF0;
            Assert.Equal(0xFE, b[0] | b[1] | b[2] | b[3]);
        }

        [Fact]
        public static void RefAdd()
        {
            int[] a = new int[] { 0x123, 0x234, 0x345, 0x456 };

            ref int r1 = ref Unsafe.Add(ref a[0], 1);
            Assert.Equal(0x234, r1);

            ref int r2 = ref Unsafe.Add(ref r1, 2);
            Assert.Equal(0x456, r2);

            ref int r3 = ref Unsafe.Add(ref r2, -3);
            Assert.Equal(0x123, r3);
        }

        [Fact]
        public static void RefAddIntPtr()
        {
            int[] a = new int[] { 0x123, 0x234, 0x345, 0x456 };

            ref int r1 = ref Unsafe.Add(ref a[0], (IntPtr)1);
            Assert.Equal(0x234, r1);

            ref int r2 = ref Unsafe.Add(ref r1, (IntPtr)2);
            Assert.Equal(0x456, r2);

            ref int r3 = ref Unsafe.Add(ref r2, (IntPtr)-3);
            Assert.Equal(0x123, r3);
        }

        [Fact]
        public static void RefAddByteOffset()
        {
            byte[] a = new byte[] { 0x12, 0x34, 0x56, 0x78 };

            ref int r1 = ref Unsafe.AddByteOffset(ref a[0], (IntPtr)1);
            Assert.Equal(0x34, r1);

            ref int r2 = ref Unsafe.AddByteOffset(ref r1, (IntPtr)2);
            Assert.Equal(0x78, r2);

            ref int r3 = ref Unsafe.AddByteOffset(ref r2, (IntPtr) - 3);
            Assert.Equal(0x12, r3);
        }

        [Fact]
        public static void RefSubtract()
        {
            string[] a = new string[] { "abc", "def", "ghi", "jkl" };

            ref string r1 = ref Unsafe.Subtract(ref a[0], -2);
            Assert.Equal("ghi", r1);

            ref string r2 = ref Unsafe.Subtract(ref r1, -1);
            Assert.Equal("jkl", r2);

            ref string r3 = ref Unsafe.Subtract(ref r2, 3);
            Assert.Equal("abc", r3);
        }

        [Fact]
        public static void RefSubtractIntPtr()
        {
            string[] a = new string[] { "abc", "def", "ghi", "jkl" };

            ref string r1 = ref Unsafe.Subtract(ref a[0], (IntPtr)-2);
            Assert.Equal("ghi", r1);

            ref string r2 = ref Unsafe.Subtract(ref r1, (IntPtr)-1);
            Assert.Equal("jkl", r2);

            ref string r3 = ref Unsafe.Subtract(ref r2, (IntPtr)3);
            Assert.Equal("abc", r3);
        }

        [Fact]
        public static void RefSubtractByteOffset()
        {
            byte[] a = new byte[] { 0x12, 0x34, 0x56, 0x78 };

            ref int r1 = ref Unsafe.SubtractByteOffset(ref a[0], (IntPtr)-1);
            Assert.Equal(0x34, r1);

            ref int r2 = ref Unsafe.SubtractByteOffset(ref r1, (IntPtr)-2);
            Assert.Equal(0x78, r2);

            ref int r3 = ref Unsafe.SubtractByteOffset(ref r2, (IntPtr)3);
            Assert.Equal(0x12, r3);
        }

        [Fact]
        public static void RefAreSame()
        {
            long[] a = new long[2];

            Assert.True(Unsafe.AreSame(ref a[0], ref a[0]));
            Assert.False(Unsafe.AreSame(ref a[0], ref a[1]));
        }
#endif
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Byte4
    {
        [FieldOffset(0)]
        public byte B0;
        [FieldOffset(1)]
        public byte B1;
        [FieldOffset(2)]
        public byte B2;
        [FieldOffset(3)]
        public byte B3;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Byte4Short2
    {
        [FieldOffset(0)]
        public byte B0;
        [FieldOffset(1)]
        public byte B1;
        [FieldOffset(2)]
        public byte B2;
        [FieldOffset(3)]
        public byte B3;
        [FieldOffset(4)]
        public short S4;
        [FieldOffset(6)]
        public short S6;
    }

    public unsafe struct Byte512
    {
        public fixed byte Bytes[512];
    }
}
