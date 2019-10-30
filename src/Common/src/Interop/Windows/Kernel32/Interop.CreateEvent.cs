// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal partial class Interop
{
    internal partial class Kernel32
    {
        [DllImport(Libraries.Kernel32, EntryPoint = "CreateEventW", SetLastError = true, CharSet = CharSet.Unicode, BestFitMapping = false)]
        internal static extern SafeWaitHandle CreateEvent(
            ref SECURITY_ATTRIBUTES lpSecurityAttributes,
            bool isManualReset,
            bool initialState,
            string name);
    }
}
