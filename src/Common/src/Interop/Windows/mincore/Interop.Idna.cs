// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        //
        //  Idn APIs
        //

        [DllImport("api-ms-win-core-localization-l1-2-0.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int IdnToAscii(
                                        uint dwFlags,
                                        string lpUnicodeCharStr,
                                        int cchUnicodeChar,
                                        [System.Runtime.InteropServices.OutAttribute()]

                                        char[] lpASCIICharStr,
                                        int cchASCIIChar);

        [DllImport("api-ms-win-core-localization-l1-2-0.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int IdnToUnicode(
                                        uint dwFlags,
                                        string lpASCIICharStr,
                                        int cchASCIIChar,
                                        [System.Runtime.InteropServices.OutAttribute()]

                                        char[] lpUnicodeCharStr,
                                        int cchUnicodeChar);

        internal const int IDN_ALLOW_UNASSIGNED = 0x1;
        internal const int IDN_USE_STD3_ASCII_RULES = 0x2;
    }
}
