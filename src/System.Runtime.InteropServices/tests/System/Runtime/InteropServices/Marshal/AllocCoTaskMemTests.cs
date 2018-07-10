// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class AllocCoTaskMemTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void AllocCoTaskMem_Int32_ReadableWritable(int size)
        {
            IntPtr p = Marshal.AllocCoTaskMem(size);
            Assert.NotEqual(IntPtr.Zero, p);
            try
            {
                for (int i = 0; i < size; i++)
                {
                    Marshal.WriteByte(p + i, (byte)i);
                }

                for (int i = 0; i < size; i++)
                {
                    Assert.Equal((byte)i, Marshal.ReadByte(p + i));
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(p);
            }
        }
    }
}
