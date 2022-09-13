// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class NetSecurityNative
    {
        [DllImport(Interop.Libraries.NetSecurityNative, EntryPoint = "NetSecurityNative_EnsureGssInitialized")]
        private static extern int EnsureGssInitialized();

        // This constructor is added to address the issue with net6 regarding 
        // Shim gss api on Linux to delay loading libgssapi_krb5.so
        // issue https://github.com/dotnet/SqlClient/issues/1390
        // dotnet runtime issue https://github.com/dotnet/runtime/pull/55037
        static NetSecurityNative()
        {
            if (Environment.Version.Major >= 6)
            {
                EnsureGssInitialized();
            }
        }
    }
}
