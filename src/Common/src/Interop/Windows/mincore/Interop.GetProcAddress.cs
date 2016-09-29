// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.LibraryLoader, CharSet = CharSet.Ansi, BestFitMapping = false)]
        public static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, string lpProcName);
    }
}
