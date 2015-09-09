﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class mincore
    {
        /// <summary>
        /// WARNING: This method does not implicitly handle long paths. Use GetLongPathName.
        /// </summary>
        [DllImport(Libraries.CoreFile_L1, EntryPoint = "GetLongPathNameW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false, ExactSpelling = false)]
        private static extern int GetLongPathNamePrivate(string path, [Out]StringBuilder longPathBuffer, int bufferLength);

        internal static int GetLongPathName(string path, [Out]StringBuilder longPathBuffer, int bufferLength)
        {
            path = PathInternal.EnsureExtendedPrefixOverMaxPath(path);
            return GetLongPathNamePrivate(path, longPathBuffer, bufferLength);
        }
    }
}
