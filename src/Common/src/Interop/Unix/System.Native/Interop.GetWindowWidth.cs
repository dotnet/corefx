// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct WinSize
        {
            internal ushort Row;
            internal ushort Col;
            internal ushort XPixel;
            internal ushort YPixel;
        };

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetWindowSize", SetLastError = true)]
        internal static extern int GetWindowSize(out WinSize winSize);
    }
}
