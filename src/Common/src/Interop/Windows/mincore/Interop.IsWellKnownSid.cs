// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class mincore
    {
        [DllImport(Interop.Libraries.SecurityBase, EntryPoint = "IsWellKnownSid", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int IsWellKnownSid(byte[] sid, int type);
    }
}
