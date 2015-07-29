// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        private const uint MOVEFILE_COPY_ALLOWED = 2;

        [DllImport(Libraries.CoreFile_L2, EntryPoint = "MoveFileExW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        private static extern bool MoveFileEx(string src, string dst, uint flags);

        internal static bool MoveFile(string src, string dst, bool copyCrossVolume = true)
        {
            return MoveFileEx(src, dst, (copyCrossVolume ? MOVEFILE_COPY_ALLOWED : 0));
        }
    }
}
