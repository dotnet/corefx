// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use CreateDirectory.
        /// </summary>
        [DllImport(Libraries.CoreFile_L1, EntryPoint = "CreateDirectoryW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        private static extern bool CreateDirectoryPrivate(string path, ref SECURITY_ATTRIBUTES lpSecurityAttributes);

        internal static bool CreateDirectory(string path, ref SECURITY_ATTRIBUTES lpSecurityAttributes)
        {
            // We always want to add for CreateDirectory to get around the legacy 248 character limitation
            path = PathInternal.EnsureExtendedPrefix(path);
            return CreateDirectoryPrivate(path, ref lpSecurityAttributes);
        }
    }
}
