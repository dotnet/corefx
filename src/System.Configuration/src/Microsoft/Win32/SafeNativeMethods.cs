//------------------------------------------------------------------------------
// <copyright file="SafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Win32 {
    using System.Runtime.InteropServices;
    using System;
    using System.Security.Permissions;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Microsoft.Win32.SafeHandles;    

    [ 
    System.Security.SuppressUnmanagedCodeSecurityAttribute()
    ]
    internal static class SafeNativeMethods {
#if NOPERF
        [DllImport(ExternDll.Kernel32, SetLastError=true)]
        internal static extern bool QueryPerformanceCounter(out long value);
        
        [DllImport(ExternDll.Kernel32, SetLastError=true)]
        internal static extern bool QueryPerformanceFrequency(out long value);
#endif
    }
}
