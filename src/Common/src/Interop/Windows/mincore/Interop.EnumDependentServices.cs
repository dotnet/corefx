// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class mincore
    {
        [DllImport(Libraries.ServiceCore, CharSet = CharSet.Unicode, SetLastError = true)]
        internal extern static bool EnumDependentServices(
            IntPtr serviceHandle,
            int serviceState,
            IntPtr bufferOfENUM_SERVICE_STATUS,
            int bufSize,
            ref int bytesNeeded,
            ref int numEnumerated);
    }
}
