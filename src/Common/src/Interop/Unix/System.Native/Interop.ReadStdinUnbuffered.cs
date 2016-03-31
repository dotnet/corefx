// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_ReadStdin", SetLastError = true)]
        internal unsafe static extern int ReadStdin(byte* buffer, int bufferSize);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_InitializeConsoleBeforeRead")]
        internal unsafe static extern void InitializeConsoleBeforeRead(byte minChars = 1, byte decisecondsTimeout = 0);

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_UninitializeConsoleAfterRead")]
        internal unsafe static extern void UninitializeConsoleAfterRead();
    }
}
