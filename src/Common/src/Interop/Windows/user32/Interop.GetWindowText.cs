// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class User32
    {
        [DllImport(Libraries.User32, EntryPoint = "GetWindowTextW")]
        public static extern int GetWindowText(IntPtr hWnd, [Out]StringBuilder lpString, int nMaxCount);
    }
}
