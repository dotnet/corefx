// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.CoreFile_L2, EntryPoint = "ReplaceFileW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        private static extern bool ReplaceFilePrivate(
            string replacedFileName, string replacementFileName, string backupFileName,
            int dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved);

        internal static bool ReplaceFile(
            string replacedFileName, string replacementFileName, string backupFileName,
            int dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved)
        {
            replacedFileName = PathInternal.EnsureExtendedPrefixOverMaxPath(replacedFileName);
            replacementFileName = PathInternal.EnsureExtendedPrefixOverMaxPath(replacementFileName);
            if (backupFileName != null)
            {
                backupFileName = PathInternal.EnsureExtendedPrefixOverMaxPath(backupFileName);
            }

            return ReplaceFilePrivate(
                replacedFileName, replacementFileName, backupFileName,
                dwReplaceFlags, lpExclude, lpReserved);
        }

        internal const int REPLACEFILE_WRITE_THROUGH = 0x1;
        internal const int REPLACEFILE_IGNORE_MERGE_ERRORS = 0x2;
    }
}
