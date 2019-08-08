// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class User32
    {
        [DllImport(Libraries.User32, SetLastError = true, EntryPoint = "LoadStringW", CharSet = CharSet.Unicode)]
        internal static extern unsafe int LoadString(SafeLibraryHandle hInstance, uint uID, char* lpBuffer, int cchBufferMax);
    }
}
