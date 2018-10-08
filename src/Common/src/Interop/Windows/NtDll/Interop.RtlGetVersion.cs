// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;

internal partial class Interop
{
    internal partial class NtDll
    {
        [DllImport(Libraries.NtDll, ExactSpelling=true)]
        private static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX lpVersionInformation);

        internal static unsafe string RtlGetVersion()
        {
            var osvi = new RTL_OSVERSIONINFOEX();
            osvi.dwOSVersionInfoSize = (uint)sizeof(RTL_OSVERSIONINFOEX);
            const string version = "Microsoft Windows";
            if (RtlGetVersion(ref osvi) == 0)
            {
                return $"{version} {osvi.dwMajorVersion}.{osvi.dwMinorVersion}.{osvi.dwBuildNumber}" +
                       $"{(string.IsNullOrWhitespace(osvi.szCSDVersion?[0]) ? string.Empty : new string(&(osvi.szCSDVersion[0])))}";
            }
            else
            {
                return version;
            }
        }
    }
}
