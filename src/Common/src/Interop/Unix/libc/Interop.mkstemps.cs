// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class libc
    {
        [DllImport(Libraries.Libc, SetLastError = true, EntryPoint = "mkstemps64")]
        internal static extern int mkstemps(byte[] template, int suffixlen);
    }
}
