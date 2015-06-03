// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class libc
    {
        [DllImport(Libraries.Libc)]
        internal static extern int fnmatch(string pattern, string str, FnmatchFlags flags);

        [Flags]
        internal enum FnmatchFlags
        {
            None = 0
        }
    }
}
