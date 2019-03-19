// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Sys
    {
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetTimestampResolution", BestFitMapping = false, ExactSpelling = true)]
        internal static extern unsafe long GetTimestampResolution();

        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_GetTimestamp", BestFitMapping = false, ExactSpelling = true)]
        internal static extern unsafe long GetTimestamp();
    }
}
