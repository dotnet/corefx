// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {        
        [DllImport(Libraries.SecurityBase, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(
            SafeTokenHandle TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGE NewState,
            uint BufferLength,
            ref TOKEN_PRIVILEGE PreviousState,
            ref uint ReturnLength);
    }
}
