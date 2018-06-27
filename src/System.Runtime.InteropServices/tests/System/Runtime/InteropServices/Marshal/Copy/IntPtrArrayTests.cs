// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class IntPtrArrayTests
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
        public void NullValueArguments_ThrowsArgumentNullException(IntPtr[] TestArray)
        {
            IntPtr[] array = null;
            Assert.Throws<ArgumentNullException>("destination", () => Marshal.Copy(TestArray, 0, IntPtr.Zero, 0));
            Assert.Throws<ArgumentNullException>("source", () => Marshal.Copy(array, 0, new IntPtr(1), 0));
            Assert.Throws<ArgumentNullException>("destination", () => Marshal.Copy(new IntPtr(1), array, 0, 0));
            Assert.Throws<ArgumentNullException>("source", () => Marshal.Copy(IntPtr.Zero, TestArray, 0, 0));
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void OutOfRangeArguments_ThrowsArgumentOutOfRangeException(IntPtr[] TestArray)
        {
            int sizeOfArray = IntPtr.Size * TestArray.Length;
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);

            Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(TestArray, 0, ptr, TestArray.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(TestArray, TestArray.Length + 1, ptr, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(TestArray, 2, ptr, TestArray.Length));

            Marshal.FreeCoTaskMem(ptr);
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void CopyRoundTrip_MatchesOriginalArray(IntPtr[] TestArray)
        {
            int sizeOfArray = IntPtr.Size * TestArray.Length;
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);

            Marshal.Copy(TestArray, 0, ptr, TestArray.Length);

            IntPtr[] array1 = new IntPtr[TestArray.Length];
            Marshal.Copy(ptr, array1, 0, TestArray.Length);
            Assert.True(CommonHelper.IsArrayEqual(TestArray, array1), "Failed copy round trip test, original array and round trip copied arrays do not match.");

            Marshal.Copy(TestArray, 2, ptr, TestArray.Length - 4);
            IntPtr[] array2 = new IntPtr[TestArray.Length];
            Marshal.Copy(ptr, array2, 2, TestArray.Length - 4);
            Assert.True(CommonHelper.IsSubArrayEqual(TestArray, array2, 2, TestArray.Length - 4), "Failed copy round trip test, original array and round trip partially copied arrays do not match.");

            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
