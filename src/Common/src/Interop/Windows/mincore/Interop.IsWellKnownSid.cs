// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
