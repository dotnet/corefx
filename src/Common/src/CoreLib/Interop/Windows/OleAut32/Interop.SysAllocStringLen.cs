// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;

internal partial class Interop
{
    internal partial class OleAut32
    {
        [DllImport(Libraries.OleAut32, CharSet = CharSet.Unicode)]
        internal static extern SafeBSTRHandle SysAllocStringLen(IntPtr src, uint len);

        [DllImport(Libraries.OleAut32, CharSet = CharSet.Unicode)]
        internal static extern IntPtr SysAllocStringLen(string src, int len);
    }
}
