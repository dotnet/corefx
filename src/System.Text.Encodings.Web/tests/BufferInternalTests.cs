// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Text.Encodings.Web.Tests
{
    public class BufferInternalTests
    {
        [Fact]
        public unsafe void CopyOverlappingNotOK()
        {
            byte[] array = new byte[] { 1, 2, 3, 4 };

            fixed (byte* pArray = array)
            {
                void* pArrayPlusOne = pArray + 1;
                BufferInternal.MemoryCopy(pArray, pArrayPlusOne, array.Length - 1, array.Length - 1);
            }

            Assert.Equal(new byte[] { 1, 1, 2, 3 }, array);
        }

        [Fact]
        public unsafe void CopyOverlappingNotOKByOne()
        {
            byte[] array = new byte[] { 1, 2 };

            fixed (byte* pArray = array)
            {
                void* pArrayPlusOne = pArray + 1;
                BufferInternal.MemoryCopy(pArray, pArrayPlusOne, array.Length - 1, array.Length - 1);
            }

            Assert.Equal(new byte[] { 1, 1 }, array);
        }

        [Fact]
        public unsafe void CopyOverlappingOK()
        {
            byte[] array = new byte[] { 1, 2, 3, 4 };

            fixed (byte* pArray = array)
            {
                void* pArrayPlusOne = pArray + 1;
                BufferInternal.MemoryCopy(pArrayPlusOne, pArray, array.Length - 1, array.Length - 1);
            }
            Assert.Equal(new byte[] { 2, 3, 4, 4 }, array);
        }
    }
}
