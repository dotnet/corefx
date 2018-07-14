// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ZeroFreeCoTaskMemUnicodeTests
    {
        [Fact]
        public void ZeroFreeCoTaskMemUnicode_ValidPointer_Success()
        {
            IntPtr ptr = Marshal.StringToCoTaskMemUni("hello");
            Marshal.ZeroFreeCoTaskMemUnicode(ptr);
        }

        [Fact]
        public void ZeroFreeCoTaskMemUnicode_Zero_Nop()
        {
            Marshal.ZeroFreeCoTaskMemUnicode(IntPtr.Zero);
        }
    }
}
