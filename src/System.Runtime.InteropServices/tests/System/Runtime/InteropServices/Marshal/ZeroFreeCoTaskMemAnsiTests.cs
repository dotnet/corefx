// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ZeroFreeCoTaskMemAnsiTests
    {
        [Fact]
        public void ZeroFreeCoTaskMemAnsi_ValidPointer_Success()
        {
            IntPtr ptr = Marshal.StringToCoTaskMemAnsi("hello");
            Marshal.ZeroFreeCoTaskMemAnsi(ptr);
        }

        [Fact]
        public void ZeroFreeCoTaskMemAnsi_Zero_Nop()
        {
            Marshal.ZeroFreeCoTaskMemAnsi(IntPtr.Zero);
        }
    }
}
