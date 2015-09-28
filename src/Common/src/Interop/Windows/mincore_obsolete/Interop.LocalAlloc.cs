// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    internal static partial class mincore_obsolete
    {
        internal const int LMEM_FIXED = 0;

        [DllImport(Interop.Libraries.Heap, SetLastError = true)]
        internal static extern SafeLocalAllocHandle LocalAlloc(int uFlags, UIntPtr sizetdwBytes);
    }
}
