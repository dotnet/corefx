// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class Int16ArrayTests
    {
        [Theory]
        [InlineData(new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
        public void CopyTo_Roundtrip_MatchesOriginalInput(short[] values)
        {
            int sizeOfArray = sizeof(short) * values.Length;
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            try
            {
                Marshal.Copy(values, 0, ptr, values.Length);

                short[] array1 = new short[values.Length];
                Marshal.Copy(ptr, array1, 0, values.Length);
                Assert.Equal<short>(values, array1);

                Marshal.Copy(values, 2, ptr, values.Length - 4);
                short[] array2 = new short[values.Length];
                Marshal.Copy(ptr, array2, 2, values.Length - 4);
                Assert.Equal<short>(values.AsSpan(2, values.Length - 4).ToArray(), array2.AsSpan(2, values.Length - 4).ToArray());
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }


        [Fact]
        public void CopyTo_NullDestination_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("destination", () => Marshal.Copy(new short[10], 0, IntPtr.Zero, 0));
            AssertExtensions.Throws<ArgumentNullException>("destination", () => Marshal.Copy(new IntPtr(1), (short[])null, 0, 0));
        }

        [Fact]
        public void CopyTo_NullSource_ThrowsArgumentNullException()
        {
            AssertExtensions.Throws<ArgumentNullException>("source", () => Marshal.Copy((short[])null, 0, new IntPtr(1), 0));
            AssertExtensions.Throws<ArgumentNullException>("source", () => Marshal.Copy(IntPtr.Zero, new short[10], 0, 0));
        }

        [Fact]
        public void CopyTo_NegativeStartIndex_ThrowsArgumentOutOfRangeException()
        {
            short[] array = new short[10];
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeof(short) * array.Length);
            try
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => Marshal.Copy(array, -1, ptr, 10));
                AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => Marshal.Copy(ptr, array, -1, 10));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Fact]
        public void CopyTo_NegativeLength_ThrowsArgumentOutOfRangeException()
        {
            short[] array = new short[10];
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeof(short) * array.Length);
            try
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => Marshal.Copy(array, 0, ptr, -1));
                AssertExtensions.Throws<ArgumentOutOfRangeException>("length", () => Marshal.Copy(ptr, array, 0, -1));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Theory]
        [InlineData(0, 11)]
        [InlineData(11, 1)]
        [InlineData(2, 10)]
        public void CopyTo_InvalidStartIndexLength_ThrowsArgumentOutOfRangeException(int startIndex, int length)
        {
            short[] array = new short[10];
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeof(short) * array.Length);
            try
            {
                AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => Marshal.Copy(array, startIndex, ptr, length));
                AssertExtensions.Throws<ArgumentOutOfRangeException>(null, () => Marshal.Copy(ptr, array, startIndex, length));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }
    }
}
