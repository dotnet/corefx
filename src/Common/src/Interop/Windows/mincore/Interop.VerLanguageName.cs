// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Text;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.Localization, CharSet = CharSet.Unicode, EntryPoint = "VerLanguageNameW")]
        internal static extern int VerLanguageName(uint langID, [Out] StringBuilder lpBuffer, uint nSize);
    }
}
