// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

internal partial class Interop
{
    internal partial class NtDll
    {
        [DllImport(Libraries.NtDll, ExactSpelling=true)]
        private static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX lpVersionInformation);

        private static unsafe Tuple<uint, uint, uint, string> RtlGetVersionInternal()
        {
            var osvi = new RTL_OSVERSIONINFOEX();
            osvi.dwOSVersionInfoSize = (uint)sizeof(RTL_OSVERSIONINFOEX);
            
            if (RtlGetVersion(ref osvi) == 0)
            {
                return osvi.szCSDVersion[0] != '\0' ?
                    new Tuple<uint, uint, uint, string>(osvi.dwMajorVersion, osvi.dwMinorVersion, osvi.dwBuildNumber, new string(&(osvi.szCSDVersion[0]))) :
                    new Tuple<uint, uint, uint, string>(osvi.dwMajorVersion, osvi.dwMinorVersion, osvi.dwBuildNumber, null);
            }
            else
            {
                return null;
            }
        } 

        internal static string RtlGetVersion()
        {
            var sb = new StringBuilder();
            sb.Append("Microsoft Windows");
            Tuple<uint, uint, uint, string> info = RtlGetVersionInternal();
            if (info != null)
            {
                sb.AppendFormat(" {0}.{1}.{2}", info.Item1, info.Item2, info.Item3);
                if (info.Item4 != null)
                {
                    sb.AppendFormat(" {0}", info.Item4);
                }
            }

            return sb.ToString();
        }

        internal static uint GetWindowsVersion()
        {
            Tuple<uint, uint, uint, string> info = RtlGetVersionInternal();
            return info.Item1;
        }

        internal static uint GetWindowsMinorVersion()
        {
            Tuple<uint, uint, uint, string> info = RtlGetVersionInternal();
            return info.Item2;
        }

        internal static uint GetWindowsBuildNumber()
        {
            Tuple<uint, uint, uint, string> info = RtlGetVersionInternal();
            return info.Item3;
        }
    }
}
