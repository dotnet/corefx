// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class IntPtrTests
    {
        public static readonly object[][] ArrayData =
        {
            new object[] { GenerateIntPtrArray() }
        };

        public static IntPtr[] GenerateIntPtrArray()
        {
            IntPtr[] testArray = new IntPtr[10];
            for (int i = 0; i < testArray.Length; i++)
                testArray[i] = new IntPtr(i);
            return testArray;
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void NullValue_ThrowsException(IntPtr[] TestValues)
        {
            IntPtr value;

            try
            {
                value = Marshal.ReadIntPtr(IntPtr.Zero);
            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }

            try
            {
                value = Marshal.ReadIntPtr(IntPtr.Zero, 2);
            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }

            try
            {
                Marshal.WriteIntPtr(IntPtr.Zero, TestValues[0]);

            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }

            try
            {
                Marshal.WriteIntPtr(IntPtr.Zero, 2, TestValues[0]);
            }
            catch (Exception e)
            {
                //ProjectN throws NullReferenceException
                Assert.True(e.GetType().FullName == "System.AccessViolationException" || e.GetType().FullName == "System.NullReferenceException", e.GetType().FullName + " is not expected.");
            }
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void ReadWriteRoundTrip(IntPtr[] TestValues)
        {
            int sizeOfArray = Marshal.SizeOf(TestValues[0]) * TestValues.Length;

            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            Marshal.WriteIntPtr(ptr, TestValues[0]);

            for (int i = 1; i < TestValues.Length; i++)
            {
                Marshal.WriteIntPtr(ptr, i * Marshal.SizeOf(TestValues[0]), TestValues[i]);
            }

            IntPtr value = Marshal.ReadIntPtr(ptr);
            Assert.True(value.Equals(TestValues[0]), "Failed round trip ReadWrite test.");

            for (int i = 1; i < TestValues.Length; i++)
            {
                value = Marshal.ReadIntPtr(ptr, i * Marshal.SizeOf(TestValues[0]));
                Assert.True(value.Equals(TestValues[i]), "Failed round trip ReadWrite test.");
            }
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
