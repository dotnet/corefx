// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class ZeroFreeGlobalAllocAnsiTests
    {
        [Fact]
        public void ZeroFreeGlobalAllocAnsi_ValidPointer_Success()
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi("hello");
            Marshal.ZeroFreeGlobalAllocAnsi(ptr);
        }

        [Fact]
        public void ZeroFreeGlobalAllocAnsi_Zero_Nop()
        {
            Marshal.ZeroFreeGlobalAllocAnsi(IntPtr.Zero);
        }
    }
}
