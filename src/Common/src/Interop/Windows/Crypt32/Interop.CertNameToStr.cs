﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Crypt32
    {
        [DllImport(Libraries.Crypt32, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "CertNameToStrW")]
        internal static extern unsafe int CertNameToStr(
            int dwCertEncodingType,
            void* pName,
            int dwStrType,
            char* psz,
            int csz);
    }
}
