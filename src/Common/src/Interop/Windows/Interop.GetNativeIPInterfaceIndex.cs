// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        public static uint GetNativeIPInterfaceIndex(string name) => 0; //TODO: um.. where should I put `if_nametoindex` for Windows (available since Vista)
    }
}
