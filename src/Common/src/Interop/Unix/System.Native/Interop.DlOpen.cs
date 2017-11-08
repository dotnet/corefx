// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [Flags]
        internal enum DlOpenFlags : int
        {
            RTLD_LAZY = 1,
            RTLD_NOW = 2
        }

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_DlOpen")]
        internal static extern IntPtr DlOpen(string fileName, DlOpenFlags flag);
    }
}
