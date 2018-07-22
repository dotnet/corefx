// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Kernel32
    {
        internal const uint MUI_PREFERRED_UI_LANGUAGES = 0x10;

        [DllImport(Libraries.Kernel32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        internal static extern bool GetFileMUIPath(uint flags, string filePath, [Out] StringBuilder language, ref int languageLength, [Out] StringBuilder fileMuiPath, ref int fileMuiPathLength, ref long enumerator);
    }
}
