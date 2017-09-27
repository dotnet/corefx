// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Versioning;

namespace Microsoft.Win32.SafeHandles
{
    internal sealed class SafeEventLogReadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeEventLogReadHandle() : base(true) { }

#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors.
        [DllImport(Interop.Libraries.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        internal static extern SafeEventLogReadHandle OpenEventLog(string UNCServerName, string sourceName);
#pragma warning restore BCL0015

#pragma warning disable BCL0015 // Disable Pinvoke analyzer errors. 
        [DllImport(Interop.Libraries.Advapi32, SetLastError = true)]
        private static extern bool CloseEventLog(IntPtr hEventLog);
#pragma warning restore BCL0015

        override protected bool ReleaseHandle()
        {
            return CloseEventLog(handle);
        }
    }
}


