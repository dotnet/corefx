// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ZeroFreeGlobalAllocUnicodeTests
    {
        [Fact]
        public void ZeroFreeGlobalAllocUnicode_ValidPointer_Success()
        {
            IntPtr ptr = Marshal.StringToHGlobalUni("hello");
            Marshal.ZeroFreeGlobalAllocUnicode(ptr);
        }

        [Fact]
        public void ZeroFreeGlobalAllocUnicode_Zero_Nop()
        {
            Marshal.ZeroFreeGlobalAllocUnicode(IntPtr.Zero);
        }
    }
}
