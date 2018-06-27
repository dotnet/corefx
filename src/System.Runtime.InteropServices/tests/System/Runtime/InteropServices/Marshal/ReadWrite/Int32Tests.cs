// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class Int32Tests
    {
        public static readonly object[][] ArrayData =
        {
            new object[] { new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, int.MaxValue } }
        };

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void NullValue_ThrowsException(int[] TestValues)
        {
            int value;

            try
            {
                value = Marshal.ReadInt32(IntPtr.Zero);
            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }

            try
            {
                value = Marshal.ReadInt32(IntPtr.Zero, 2);
            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }

            try
            {
                Marshal.WriteInt32(IntPtr.Zero, TestValues[0]);

            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }

            try
            {
                Marshal.WriteInt32(IntPtr.Zero, 2, TestValues[0]);
            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void ReadWriteRoundTrip(int[] TestValues)
        {
            int sizeOfArray = Marshal.SizeOf(TestValues[0]) * TestValues.Length;

            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            Marshal.WriteInt32(ptr, TestValues[0]);

            for (int i = 1; i < TestValues.Length; i++)
            {
                Marshal.WriteInt32(ptr, i * Marshal.SizeOf(TestValues[0]), TestValues[i]);
            }

            int value = Marshal.ReadInt32(ptr);
            Assert.True(value.Equals(TestValues[0]), "Failed round trip ReadWrite test.");

            for (int i = 1; i < TestValues.Length; i++)
            {
                value = Marshal.ReadInt32(ptr, i * Marshal.SizeOf(TestValues[0]));
                Assert.True(value.Equals(TestValues[i]), "Failed round trip ReadWrite test.");
            }
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
