// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.CoreFile_L1, SetLastError = true)]
        unsafe internal static extern int ReadFile(
            IntPtr handle,
            byte* bytes,
            int numBytesToRead,
            out int numBytesRead,
            IntPtr mustBeZero);
    }
}
