// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.CoreFile_L2, CharSet = CharSet.Unicode, BestFitMapping = false)]
        internal static extern int CopyFile2(string pwszExistingFileName, string pwszNewFileName, ref COPYFILE2_EXTENDED_PARAMETERS pExtendedParameters);

        internal struct COPYFILE2_EXTENDED_PARAMETERS
        {
            internal uint dwSize;
            internal uint dwCopyFlags;
            internal IntPtr pfCancel;
            internal IntPtr pProgressRoutine;
            internal IntPtr pvCallbackContext;
        }
    }
}
