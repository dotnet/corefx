// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class IpHlpApi
    {
        // Available since Windows Vista and Windows Server 2008
        [DllImport(Interop.Libraries.IpHlpApi, SetLastError = true)]
        internal extern static uint if_nametoindex(string name);
    }
}
